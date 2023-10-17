using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public class DescriptorHeap: Pageable, IDescriptorHeap {
	public new ComPtr< ID3D12DescriptorHeap >? ComPointer { get ; protected set ; }
	
	internal DescriptorHeap( ) { }
	internal DescriptorHeap( ComPtr< ID3D12DescriptorHeap > ptr ) => ComPointer = ptr ;
	internal DescriptorHeap( nint ptr ) => ComPointer = new( ptr ) ;
	internal DescriptorHeap( ID3D12DescriptorHeap ptr ) => ComPointer = new( ptr ) ;
	
} ;