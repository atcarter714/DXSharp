using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public interface IStateObject: IPageable,
							   IComObjectRef< ID3D12StateObject >,
							   IUnknownWrapper< ID3D12StateObject > {
	new static Type ComType => typeof(ID3D12StateObject) ;
	new static Guid InterfaceGUID => typeof(ID3D12StateObject).GUID ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12StateObject).GUID ;
	

	new ComPtr< ID3D12StateObject >? ComPointer { get ; }
	new ID3D12StateObject? COMObject => ComPointer?.Interface ;

	ComPtr< ID3D12Pageable >? IPageable.ComPointer => new( COMObject! ) ;
	
} ;