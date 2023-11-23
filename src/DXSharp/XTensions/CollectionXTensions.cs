#region Using Directives
using System ;
using System.Buffers ;
using System.Linq ;
using System.Collections ;
using System.Collections.Generic ;
using System.Numerics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

#endregion
namespace DXSharp.XTensions ;


/// <summary>
/// Provides extension methods, utilities, helpers
/// and add-ons for .NET collections types.
/// </summary>
public static class CollectionXTensions {
	public const int DEFAULT_CACHE_ARRAY_SIZE
#if CACHE_SIZE_SMALL
		= 0x10 ;
#elif CACHE_SIZE_NORMAL
		= 0x40 ;
#elif CACHE_SIZE_LARGE
		= 0x100 ;
#elif CACHE_SIZE_MAX
		= 0x400 ;
#else
		= 0x40 ;
#endif
	
	static readonly Dictionary< Type, Array > _oneItemArrays  = new( ) ;
	static readonly Dictionary< Type, Array > _tempArrayCache = new( ) ;
	internal static IReadOnlyList< Type > _oneItemArrayTypes => new List< Type >( _oneItemArrays.Keys ) ;
	internal static IReadOnlyList< Type > _tempArrayTypes => new List< Type >( _tempArrayCache.Keys ) ;
	
	
	
	/// <summary>Gets a cached array of a single item of type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The type of element in the array.</typeparam>
	/// <returns>A cached array of a single item of type <typeparamref name="T"/>.</returns>
	/// <remarks>
	/// The array is from a cache of single-item arrays to recycle and skip extra memory allocations,
	/// especially in frequently-accessed sections of code (i.e., the "hot path") of a real-time
	/// application (such as a game or 3D application).<para/>
	/// [<b>WARNING:</b> Do not check for a saved value in the returned array, as these are for temporary
	/// use only and may be overwritten at any time and are not synchronized between threads.]<para/>
	/// (<b>NOTE:</b> If you are sure no other thread could be accessing the array memory simultaneously, you may clear,
	/// overwrite, or otherwise modify the array's contents to use it for momentary storage of a single
	/// value where an array or collection is required for something.)<para/>
	/// A single-item array storing an item of the type <typeparamref name="T"/> is created and cached the
	/// first time one is requested. Subsequent requests for a single-item array of the same type will fetch
	/// the cached array instead of creating a new one.
	/// </remarks>
	public static T[ ] GetSingleItemArrayCache< T >( ) {
		if( _oneItemArrays.TryGetValue( typeof(T), out var array ) )
			return (T[ ])array ;
		
		var newArray = new T[ 1 ] ;
		_oneItemArrays.Add( typeof(T), newArray ) ;
		return newArray ;
	}

	/// <summary>Gets a temporary array of the specified length.</summary>
	/// <typeparam name="T">The type of element in the array.</typeparam>
	/// <param name="minSize">
	/// The minimum required length of the cached array to get. Unless otherwise specified, the default
	/// array length is defined as <see cref="DEFAULT_CACHE_ARRAY_SIZE"/> (builds can change this value).
	/// If the temporary cached array is not of sufficient size, it will get resized to the minimum length.
	/// </param>
	/// <returns>A temporary array of at least the specified length (or greater).</returns>
	/// <remarks>
	/// The array is from a cache of temporary arrays to recycle and skip extra memory allocations,
	/// especially in frequently-accessed sections of code (i.e., the "hot path") of a real-time
	/// application (such as a game or 3D application).<para/>
	/// [<b>WARNING:</b> Do not check for a saved value in the returned array, as these are for temporary
	/// use only and may be overwritten at any time and are not synchronized between threads.]<para/>
	/// (<b>NOTE:</b> If you are sure no other thread could be accessing the array memory simultaneously, you may clear,
	/// overwrite, or otherwise modify the array's contents to use it for momentary storage of a single
	/// value where an array or collection is required for something.)<para/>
	/// A temporary array of the specified length is created and cached the first time one is requested.
	/// Subsequent requests for a temporary array of the same length will fetch the cached array instead
	/// of creating a new one.
	/// </remarks>
	public static T[ ] GetTempArrayCache< T >( int minSize = -1 ) {
		bool sizeSpecified = ( minSize > -1 ) ;
		
		int length = sizeSpecified
						 ? minSize : DEFAULT_CACHE_ARRAY_SIZE ;
		
		// If the array is already cached, return it:
		if( _tempArrayCache.TryGetValue( typeof(T), out var _array ) ) {
			// If the array is large enough, return it:
			if( sizeSpecified && _array.Length >= minSize )
				return ( T[ ] )_array ;
			
			// Otherwise, resize the array and return it:
			var newArray = new T[ length ] ;
			_tempArrayCache[ typeof(T) ] = newArray ;
			return newArray ;
		}
		
		// Otherwise, create a new array and cache it:
		var array = new T[ length ] ;
		_tempArrayCache.Add( typeof(T), array ) ;
		return array ;
	}
	
	
	
	public static bool EndsWithAny< T >( this IEnumerable< T > collection, in Span< T > values ) 
																		where T: IEquatable< T > {
		var item = collection.Last( ) ;
		if( values.IndexOf( item ) != -1 ) return true ;
		
		return false ;
	}
	
	public static bool EndsWithAny< T >( this IEnumerable< T > collection, params T[ ] values ) 
																			where T: IEquatable< T > {
		var item = collection.Last( ) ;
		if( values.Contains(item) ) return true ;
		
		return false ;
	}
	
	public static bool EndsWithAny< T >( this IEnumerable< T > collection, Func< T, bool > predicate ) {
		var item = collection.Last( ) ;
		if( predicate(item) ) return true ;
		
		return false ;
	}
	
	public static bool EndsWith< T >( this IEnumerable< T > collection, Func< T, bool > predicate ) {
		var item = collection.Last( ) ;
		return predicate(item) ;
	}
	
	
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static TOut[ ] ReinterpretCast< TIn, TOut >( this TIn[ ] array )  where TIn:  unmanaged 
																				  where TOut: unmanaged {
		var asTOutRef = Unsafe.As< TOut[] >( array ) ;
		return asTOutRef ;
	}
	
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Span< TOut > ReinterpretSpan< TIn, TOut >( this Span< TIn > values ) where TIn  : unmanaged
		where TOut : unmanaged {
		var result = MemoryMarshal.Cast< TIn, TOut >( values ) ;
		return result ;
	}
	
	public static MemoryHandle AllocatePinned< T >( this T[ ] array ) where T: unmanaged {
		Memory< T > memory = array ;
		return memory.Pin( ) ;
	}
	
	public static T[ ] FromPointer< T >( this nint pointer, int length ) where T: unmanaged {
#if DEBUG || DEV_BUILD
		if( !pointer.IsValid() ) throw new ArgumentException( nameof(pointer) ) ;
		if( length < 1 ) throw new ArgumentOutOfRangeException( nameof(length) ) ;
#endif
		
		unsafe {
			// Get reference to the first element in the buffer:
			var rFirst = Unsafe.AsRef< T >( *( (T *)pointer ) ) ;
			
			// Create a managed array over the memory pointed to by the pointer
			// without allocating more memory, copying or otherwise duplicating it ...
			return Unsafe.As< T[ ] >( rFirst ) ;
		}
	}
	
	
	public static void AddRange< T >( this List< T > list, params T[ ]? items ) {
		ArgumentNullException.ThrowIfNull( list, nameof(list) ) ;
		ArgumentNullException.ThrowIfNull( items, nameof(items) ) ;
		
		list.AddRange( items.AsEnumerable() ) ;
	}
	
	
	public static int IndexOf< K, V >( this IDictionary<K, V> list, V item ) 
										where V: IEqualityOperators< V, V, bool > {
		int index = 0 ;
		foreach ( var entry in list ) {
			if( entry.Value != item ) {
				++index ;
				continue ;
			}
			return index ;
		}
		
		return -1 ;
	}
	
	
	public static int IndexOfReference< K, V >( this IDictionary<K, V> list, V item ) where V: class {
		int index = 0 ;
		foreach ( var entry in list ) {
			if( !ReferenceEquals( entry.Value, item ) ) {
				++index ;
				continue ;
			}
			return index ;
		}
		
		return -1 ;
	}
} ;