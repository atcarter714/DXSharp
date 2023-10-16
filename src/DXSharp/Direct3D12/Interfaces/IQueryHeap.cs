#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12QueryHeap) )]
public interface IQueryHeap: IPageable,
						IComObjectRef< ID3D12QueryHeap >,
						IUnknownWrapper< ID3D12QueryHeap > {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12QueryHeap > ComPointer { get ; }
	new ID3D12QueryHeap? COMObject => ComPointer?.Interface ;
	ID3D12QueryHeap? IComObjectRef< ID3D12QueryHeap >.COMObject => COMObject ;
	ComPtr< ID3D12QueryHeap >? IUnknownWrapper< ID3D12QueryHeap >.ComPointer => ComPointer ;
	
	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof(ID3D12QueryHeap) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12QueryHeap).GUID ;
	// ==================================================================================
} ;