#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12StateObject))]
public interface IStateObject: IPageable, IInstantiable {
	new static Type ComType => typeof(ID3D12StateObject) ;
	
	new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12StateObject).GUID
																 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new StateObject( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new StateObject( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new StateObject( (ID3D12StateObject)pComObj! ) ;
	
	// ==================================================================================
} ;


[Wrapper( typeof( ID3D12StateObject ) )]
internal class StateObject: Pageable, 
							IStateObject, 
							IComObjectRef< ID3D12StateObject >,
							IUnknownWrapper< ID3D12StateObject > {
	ComPtr<ID3D12StateObject>? _comPtr ;
	public new virtual ComPtr< ID3D12StateObject >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12StateObject >( ) ;
	public override ID3D12StateObject? COMObject => ComPointer?.Interface ;

	
	internal StateObject( ) {
		_comPtr = ComResources?.GetPointer< ID3D12StateObject >( ) ;
	}
	internal StateObject( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal StateObject( ID3D12StateObject ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12StateObject).GUID
																 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	public new static Type ComType => typeof(ID3D12StateObject) ;
} ;