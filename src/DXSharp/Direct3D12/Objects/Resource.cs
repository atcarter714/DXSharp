#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Resource))]
public class Resource: Pageable,
					   IResource,
					   IInstantiable< Resource > {
	
	public new ID3D12Resource? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Resource >? ComPointer { get ;  protected set ; }
	
	internal Resource( ) { }
	internal Resource( ComPtr<ID3D12Resource> ptr ) => ComPointer = ptr ;
	internal Resource( ID3D12Resource _interface ) => ComPointer = new( _interface ) ;
	internal Resource( nint address ) => ComPointer = new( address ) ;
	
	
	// -----------------------------------------------------------------------------------------------------------------
	
	static IInstantiable IInstantiable. Instantiate( )                => new Resource( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Resource( pComObj ) ;
	public static IInstantiable Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => 
			pComObj is not null ? ( new Resource( (ID3D12Resource)pComObj 
							?? throw new InvalidCastException($"Cannot cast {nameof(pComObj)} " +
									$"({pComObj.GetType().Name}) to {nameof(IDXGIResource)}!") ))
				: throw new ArgumentNullException( nameof(pComObj) ) ;
	
	static Resource? IInstantiable< Resource >.Instantiate( nint ptr ) => 
		(ptr is 0x0000) ? null : new( ptr ) ;
	static TResource Instantiate< TResource >( nint ptr ) => 
		(TResource)( (IResource)new Resource( ptr ) ) ;
	// =================================================================================================================
} ;