using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;


[Wrapper( typeof( ID3D12CommandSignature ) )]
public interface ICommandSignature: IPageable,
									IComObjectRef< ID3D12CommandSignature >,
									IUnknownWrapper< ID3D12CommandSignature > {
	static Guid IUnknownWrapper< ID3D12CommandSignature >.InterfaceGUID => 
		typeof(ID3D12CommandSignature).GUID ;
	
	new Type ComType => typeof( ID3D12CommandSignature ) ;
	new ComPtr< ID3D12CommandSignature >? ComPointer { get ; }
	new Guid InterfaceGUID => typeof( ID3D12CommandSignature ).GUID ;
	new ID3D12CommandSignature? COMObject => ComPointer?.Interface ;
	new ID3D12CommandSignature? ComObject => ComPointer?.Interface ;

} ;