#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


public class CommandList: Pageable, ICommandList {
	public new static Type ComType => typeof( ID3D12CommandList ) ;
	public new static Guid InterfaceGUID => typeof( ID3D12CommandList ).GUID ;
	
	// ------------------------------------------------------------------------------------------
	public new ID3D12CommandList? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12CommandList >? ComPointer { get ; protected set ; }
	// ------------------------------------------------------------------------------------------
	
	internal CommandList( ) { }
	internal CommandList( nint ptr ) => ComPointer = new( ptr ) ;
	internal CommandList( ComPtr< ID3D12CommandList > comObject ) => ComPointer = comObject ;
	internal CommandList( ID3D12CommandList? comObject ) => ComPointer = new( comObject! ) ;
	
	
	public static IDXCOMObject Instantiate( ) => new CommandList( ) ;
	public static IDXCOMObject Instantiate( IntPtr pComObj ) => new CommandList( pComObj ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => new CommandList( pComObj as ID3D12CommandList ) ;
} ;