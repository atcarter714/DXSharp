#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof( ID3D12CommandSignature ) )]
public interface ICommandSignature: IPageable,
									IComObjectRef< ID3D12CommandSignature >,
									IUnknownWrapper< ID3D12CommandSignature > {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12CommandSignature >? ComPointer { get ; }
	new ID3D12CommandSignature? COMObject => ComPointer?.Interface ;
	ID3D12CommandSignature? IComObjectRef< ID3D12CommandSignature >.COMObject => COMObject ;
	ComPtr< ID3D12CommandSignature >? IUnknownWrapper< ID3D12CommandSignature >.ComPointer => ComPointer ;
	// ==================================================================================

	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof(ID3D12CommandSignature) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12CommandSignature).GUID ;
	// ==================================================================================
} ;