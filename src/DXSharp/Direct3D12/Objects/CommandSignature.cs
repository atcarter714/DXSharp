using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;


public class CommandSignature: Pageable, ICommandSignature {
	public new ID3D12CommandSignature? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12CommandSignature >? ComPointer { get ; protected set ; }
	
	public CommandSignature( nint childAddr ): base( childAddr ) { }
	public CommandSignature( ID3D12Pageable child ): base( child ) { }
	public CommandSignature( ComPtr< IUnknown > childPtr ): base( childPtr ) { }
} ;