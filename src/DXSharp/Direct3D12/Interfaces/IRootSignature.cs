using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

[Wrapper( typeof( ID3D12RootSignature ) )]
public interface IRootSignature: IDeviceChild,
								 IComObjectRef< ID3D12RootSignature >,
								 IUnknownWrapper< ID3D12RootSignature > {
	
	
	new Type ComType => typeof( ID3D12RootSignature ) ;
	new Guid InterfaceGUID => typeof( ID3D12RootSignature ).GUID ;
	new ID3D12RootSignature? COMObject => ComPointer?.Interface ;
	new ID3D12RootSignature? ComObject => ComPointer?.Interface ;
	new ComPtr< ID3D12RootSignature >? ComPointer { get ; }

} ;