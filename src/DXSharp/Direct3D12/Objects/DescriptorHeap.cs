#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
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
	
} ;