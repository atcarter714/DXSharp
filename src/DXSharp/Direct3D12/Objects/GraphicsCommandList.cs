using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public class GraphicsCommandList: CommandList, 
								  IGraphicsCommandList {
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList ) ;
	public new static Guid InterfaceGUID => typeof(ID3D12GraphicsCommandList).GUID ;
	public new ComPtr< ID3D12GraphicsCommandList >? ComPointer { get ; protected set ; }
	
	internal GraphicsCommandList( ) { }
	internal GraphicsCommandList( nint ptr ) => ComPointer = new(ptr) ;
	internal GraphicsCommandList( ComPtr< ID3D12GraphicsCommandList > comPointer ) => ComPointer = comPointer ;
	internal GraphicsCommandList( ID3D12GraphicsCommandList obj ) => ComPointer = new(obj) ;
} ;