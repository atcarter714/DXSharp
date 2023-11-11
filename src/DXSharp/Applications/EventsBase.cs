namespace DXSharp.Applications ;


public delegate void UserResizeEventHandler( object? sender, UserResizeEventArgs e ) ;
public delegate void DPIChangedEventHandler( object sender, DPIChangedEventArgs e ) ;


public class UserResizeEventArgs: EventArgs {
	public Size NewSize { get ; }
	public UserResizeEventArgs( in Size newSize ) => NewSize = newSize ;
} ;


public class DPIChangedEventArgs: EventArgs {
	public float NewDPI { get ; }
	public Rect NewRect { get ; }
	public Point NewPosition { get ; }
	
	public DPIChangedEventArgs( float newDPI = 0f ) => NewDPI = newDPI ;
	public DPIChangedEventArgs( in Rect newRect ) => NewRect = newRect ;
	public DPIChangedEventArgs( in Point newPosition ) => NewPosition = newPosition ;
	public DPIChangedEventArgs( float newDPI, in Rect newRect, in Point newPosition = default ) {
		NewDPI = newDPI ;
		NewRect = newRect ;
		NewPosition = newPosition ;
	}
} ;