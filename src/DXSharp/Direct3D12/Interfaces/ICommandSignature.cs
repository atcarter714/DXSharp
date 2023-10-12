using Windows.Win32.Graphics.Direct3D12 ;
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12CommandSignature))]
public interface ICommandSignature:
	IPageable< ID3D12CommandSignature > { }