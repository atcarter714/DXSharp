using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public class Heap: Pageable, IHeap,
					IInstantiable< Heap > {
	
	public new ID3D12Heap? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Heap >? ComPointer { get ; protected set ; }
	
	static Heap IInstantiable< Heap >.Instantiate( ) => new( ) ;
	static IDXCOMObject IInstantiable.Instantiate( ) => new Heap( ) ;
	
	internal Heap( ) { }
	internal Heap( ID3D12Heap d3D12Heap ) => 
		ComPointer = new( d3D12Heap ) ;
}