#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


public class Device: Object,
					 IDevice,
					 IInstantiable< Device > {
	
	public new ID3D12Device? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device >? ComPointer { get ; protected set ; }
	

	internal Device( ) { }
	internal Device( ComPtr< ID3D12Device > comPtr ) => ComPointer = comPtr ;
	internal Device( nint address ) => ComPointer = new( address ) ;
	internal Device( ID3D12Device comObject ) => ComPointer = new( comObject ) ;
	
	// -------------------------------------------------------------------------------------------------------
	static IDXCOMObject IInstantiable.Instantiate( ) => new Device( ) ;
	static Device? IInstantiable< Device >.Instantiate( IntPtr ptr ) => new( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate( IntPtr pComObj ) => new Device( pComObj ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => 
		new Device( (ID3D12Device)pComObj! ) ;
	// =======================================================================================================
} ;