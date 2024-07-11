namespace DXSharp.Framework.Threading ;

public abstract class ThreadSystemBase {
	public Thread MainThread { get ; }
	
	protected ThreadSystemBase( ) {
		MainThread = Thread.CurrentThread ;
	}
	
	
} ;