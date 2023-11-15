#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12QueryHeap) )]
public interface IQueryHeap: IPageable, IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12QueryHeap, IInstantiable> > _queryHeapCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12QueryHeap, IInstantiable> > {
			{ IQueryHeap.IID, ( pComObj ) => new QueryHeap( pComObj ) },
		} ) ;
	// ---------------------------------------------------------------------------------
	
	new static Type ComType => typeof(ID3D12QueryHeap) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12QueryHeap).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new QueryHeap( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new QueryHeap( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => new QueryHeap( ( obj as ID3D12QueryHeap )! ) ;
	// ==================================================================================
} ;


[Wrapper(typeof(ID3D12QueryHeap))]
internal class QueryHeap: Pageable, 
						  IQueryHeap,
						  IComObjectRef< ID3D12QueryHeap >,
						  IUnknownWrapper< ID3D12QueryHeap > {
	ComPtr< ID3D12QueryHeap >? _comPtr ;
	public new virtual ComPtr< ID3D12QueryHeap >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12QueryHeap >( ) ;
	public override ID3D12QueryHeap? ComObject => ComPointer?.Interface ;

	public QueryHeap( ) {
		_comPtr ??= ComResources?.GetPointer< ID3D12QueryHeap >( ) ;
		if ( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	public QueryHeap( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	public QueryHeap( ID3D12QueryHeap pComObj ) {
		_comPtr = new( pComObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	public QueryHeap( ComPtr< ID3D12QueryHeap > pComObj ) {
		_comPtr = pComObj ;
		_initOrAdd( _comPtr ) ;
	}
	
	public new static Type ComType => typeof(ID3D12QueryHeap) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12QueryHeap).GUID
															   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
} ;