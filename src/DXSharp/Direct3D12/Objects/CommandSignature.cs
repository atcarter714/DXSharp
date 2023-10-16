#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12CommandSignature))]
public class CommandSignature: Pageable, 
							   ICommandSignature,
							   IInstantiable< CommandSignature > {
	// -------------------------------------------------------------------------------------------------------
	public new ID3D12CommandSignature? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12CommandSignature >? ComPointer { get ; protected set ; }
	// -------------------------------------------------------------------------------------------------------
	
	
	internal CommandSignature( ) { }
	internal CommandSignature( nint childAddr ): base( childAddr ) { }
	internal CommandSignature( ID3D12Pageable child ): base( child ) { }
	internal CommandSignature( ComPtr< IUnknown > childPtr ): base( childPtr ) { }
	
	
	// -------------------------------------------------------------------------------------------------------
	static IDXCOMObject IInstantiable.Instantiate( ) => new CommandSignature( ) ;
	static CommandSignature? IInstantiable< CommandSignature >.Instantiate( IntPtr ptr ) => new( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate( IntPtr pComObj ) => new CommandSignature( pComObj ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => 
		new CommandSignature( (ID3D12CommandSignature)pComObj! ) ;
	// =======================================================================================================
} ;