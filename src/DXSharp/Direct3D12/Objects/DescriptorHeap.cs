using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public class DescriptorHeap: Pageable, IDescriptorHeap {
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12DescriptorHeap) ;
	public new static Guid InterfaceGUID => typeof(ID3D12DescriptorHeap).GUID ;
	// ==================================================================================
	
	public new ComPtr< ID3D12DescriptorHeap >? ComPointer { get ; protected set ; }
	public new ID3D12DescriptorHeap? COMObject => ComPointer?.Interface ;
	
	internal DescriptorHeap( ) { }
	internal DescriptorHeap( ComPtr< ID3D12DescriptorHeap > ptr ) => ComPointer = ptr ;
	internal DescriptorHeap( nint ptr ) => ComPointer = new( ptr ) ;
	public DescriptorHeap( ID3D12DescriptorHeap ptr ) => ComPointer = new( ptr ) ;

	
	public DescriptorHeapDescription GetDesc( ) {
		ID3D12DescriptorHeap heap = COMObject ?? throw new ObjectDisposedException( nameof( DescriptorHeap ) ) ;
		var guid = typeof( ID3D12DescriptorHeap ).GUID ;
		Marshal.QueryInterface( ComPointer.BaseAddress, ref guid, out var ptr ) ;
		ComPtr<ID3D12DescriptorHeap> dheap = new( ptr ) ;
		var ddd = dheap.Interface.GetDesc(  ) ;
		
		heap.SetName( "DescHeap" ) ;
		return heap.GetDesc( ) ;
	}
} ;