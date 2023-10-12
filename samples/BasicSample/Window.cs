using DXSharp.Windows ;

namespace BasicSample ;


public class Window: RenderForm {
	
	protected override void OnResize( EventArgs e ) {
		base.OnResize( e ) ;
		this.Invalidate( ) ;
	}
	
}