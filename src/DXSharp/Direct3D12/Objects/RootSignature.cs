using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public class RootSignature: DeviceChild, IRootSignature {
	public new static Type ComType => typeof( ID3D12RootSignature ) ;
	public new static Guid InterfaceGUID => typeof( ID3D12RootSignature ).GUID ;
	
	// ------------------------------------------------------------------------------------------
	public new ID3D12RootSignature? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12RootSignature >? ComPointer { get ; protected set ; }
	// ------------------------------------------------------------------------------------------
	
	internal RootSignature( ) { }
	internal RootSignature( nint ptr ) => ComPointer = new( ptr ) ;
	internal RootSignature( ComPtr< ID3D12RootSignature > comObject ) => ComPointer = comObject ;
	internal RootSignature( ID3D12RootSignature? comObject ) => ComPointer = new( comObject! ) ;
} ;