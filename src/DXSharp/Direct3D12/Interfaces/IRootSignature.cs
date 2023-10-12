using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12 ;

[Wrapper(typeof(ID3D12RootSignature))]
public interface IRootSignature:
	IDeviceChild< ID3D12RootSignature > { } ;