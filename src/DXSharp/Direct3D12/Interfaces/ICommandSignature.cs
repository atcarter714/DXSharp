using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12CommandSignature))]
public interface ICommandSignature: IPageable,
									IComObjectRef< ID3D12CommandSignature >,
									IUnknownWrapper< ID3D12CommandSignature >
{ }