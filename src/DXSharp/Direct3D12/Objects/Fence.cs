using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Fence))]
public class Fence: Pageable, 
					IFence,
					IInstantiable< Fence > {
	// -----------------------------------------------------------------------------------------------
	public new ID3D12Fence? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Fence >? ComPointer { get ; protected set ; }
	// -----------------------------------------------------------------------------------------------
	
	internal Fence( ) { }
	internal Fence( ComPtr< ID3D12Fence > comPtr ) => ComPointer = comPtr ;
	internal Fence( nint address ) => ComPointer = new( address ) ;
	internal Fence( ID3D12Fence comObject ) => ComPointer = new( comObject ) ;
	
	// -----------------------------------------------------------------------------------------------
	static IDXCOMObject IInstantiable.Instantiate( ) => new Fence( ) ;
	static IDXCOMObject IInstantiable.Instantiate( IntPtr pComObj ) => new Fence( pComObj ) ;
	static Fence? IInstantiable< Fence >.Instantiate( IntPtr ptr ) => new( ptr ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom pComObj ) 
		where ICom: IUnknown? => new Fence( (ID3D12Fence) pComObj! ) ;
	// ===============================================================================================
} ;