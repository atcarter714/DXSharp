#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12RootSignature) )]
public interface IRootSignature: IDeviceChild,
								 IComObjectRef< ID3D12RootSignature >,
								 IUnknownWrapper< ID3D12RootSignature > {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12RootSignature > ComPointer { get ; }
	new ID3D12RootSignature? COMObject => ComPointer?.Interface ;
	ID3D12RootSignature? IComObjectRef< ID3D12RootSignature >.COMObject => COMObject ;
	ComPtr< ID3D12RootSignature >? IUnknownWrapper< ID3D12RootSignature >.ComPointer => ComPointer ;
	// ==================================================================================
} ;