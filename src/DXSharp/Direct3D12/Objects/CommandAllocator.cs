using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12.Objects ;


public class CommandAllocator: Pageable,
							   ICommandAllocator,
							   IInstantiable< CommandAllocator > {
	static CommandAllocator IInstantiable< CommandAllocator >.Instantiate( ) => new( ) ;
	static IDXCOMObject IInstantiable.Instantiate( ) => new CommandAllocator( ) ;
	
	public new ID3D12CommandAllocator? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12CommandAllocator >? ComPointer { get ; protected set ; }
	
	internal CommandAllocator( ) { }
	internal CommandAllocator( ComPtr< ID3D12CommandAllocator > comPtr ) => ComPointer = comPtr ;
	internal CommandAllocator( nint address ) => ComPointer = new( address ) ;
	internal CommandAllocator( ID3D12CommandAllocator comObject ) => ComPointer = new( comObject ) ;
	
	
} ;