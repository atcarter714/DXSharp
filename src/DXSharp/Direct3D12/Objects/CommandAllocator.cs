#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12CommandAllocator))]
public class CommandAllocator: Pageable,
							   ICommandAllocator,
							   IInstantiable< CommandAllocator > {
	
	public new ID3D12CommandAllocator? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12CommandAllocator >? ComPointer { get ; protected set ; }
	
	internal CommandAllocator( ) { }
	internal CommandAllocator( ComPtr< ID3D12CommandAllocator > comPtr ) => ComPointer = comPtr ;
	internal CommandAllocator( nint address ) => ComPointer = new( address ) ;
	internal CommandAllocator( ID3D12CommandAllocator comObject ) => ComPointer = new( comObject ) ;


	// -------------------------------------------------------------------------------------------------------
	public new static Guid InterfaceGUID => typeof(ID3D12CommandAllocator).GUID ;
	public new static Type ComType => typeof(ID3D12CommandAllocator) ;
	// -------------------------------------------------------------------------------------------------------

	public static CommandAllocator Instantiate( ) => new( ) ;
	static IDXCOMObject IInstantiable.Instantiate( ) => new CommandAllocator( ) ;
	static CommandAllocator? IInstantiable< CommandAllocator >.Instantiate( IntPtr ptr ) => new( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate( IntPtr pComObj ) => new CommandAllocator( pComObj ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => 
		new CommandAllocator( (ID3D12CommandAllocator)pComObj! ) ;
	// =======================================================================================================
} ;