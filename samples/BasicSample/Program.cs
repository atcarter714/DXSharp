// DXSharp Basic Sample:
using BasicSample ;

static class Program
{
	[STAThread]
	static int Main( string[ ] args ) {
		BasicApp app = new( ) ;
		app.Initialize( );
		app.Run( ) ;
		return 0 ;
	}
}