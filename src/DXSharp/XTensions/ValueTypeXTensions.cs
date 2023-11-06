#region MyRegion
using System ;
using System.Diagnostics.CodeAnalysis ;
using System.Numerics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
#endregion
namespace DXSharp.XTensions ;


/// <summary>
/// Provides extension methods for value type
/// ("primitive" or "struct") types.
/// </summary>
public static class ValueTypeXTensions {
	
	/// <summary>
	/// Interprets this value as a span of bytes.
	/// </summary>
	/// <param name="value">This (unmanaged) value instance.</param>
	/// <typeparam name="T">The type of (unmanaged) value type.</typeparam>
	/// <returns>A span of bytes that represents this value in binary form.</returns>
	public static Span< byte > AsBytes< T >( this ref T value ) where T : unmanaged => 
		MemoryMarshal.AsBytes( MemoryMarshal.CreateSpan( ref value, 1 ) ) ;
	
	public static byte[ ] GetBytes< T >( this ref T value ) where T : unmanaged => value.AsBytes( ).ToArray( ) ;
	
	public static ref T AsRef< T >( this scoped ref T value ) where T : unmanaged => ref Unsafe.AsRef( value ) ;
	
	public static ref TOut Reinterpret< TIn, TOut >( this ref TIn value ) where TIn  : unmanaged
																		  where TOut : unmanaged {
		unsafe { fixed ( TIn* ptr = &value ) {
			return ref Unsafe.AsRef< TOut >( (void*)ptr ) ;
		}}
	}
	
	public static TOut[ ] ReinterpretArray< TIn, TOut >( this TIn[ ] values ) where TIn  : unmanaged
																		 where TOut : unmanaged {
		var result = Unsafe.As< TOut[] >( values ) ;
		return result ;
	}
	
	public static Span<TOut> ReinterpretSpan< TIn, TOut >( this Span< TIn > values ) where TIn  : unmanaged
																				 where TOut : unmanaged {
		var result = MemoryMarshal.Cast< TIn, TOut >( values ) ;
		return result ;
	}
} ;