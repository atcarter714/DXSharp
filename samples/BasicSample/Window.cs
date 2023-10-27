using DXSharp.DXGI ;
using DXSharp.Windows ;

namespace BasicSample ;


public class Window: RenderForm {
	ISwapChain? _swapChain ;
	
	public Window( ) { }
	
	protected override void OnResize( EventArgs e ) {
		
		base.OnResize( e ) ;
		this.Invalidate( ) ;
	}
	
}