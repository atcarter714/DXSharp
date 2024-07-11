namespace DXSharp.Framework ;

[DXSharpError("An unexpected error occured in DXSharp.Framework.", 
			  "An undetermined exceptional case or failure in the framework..")]
public class FrameworkErrorException: DXSharpException {
	public FrameworkErrorException( ) : base( ) { }
	public FrameworkErrorException( string message ) : base( message ) { }
	public FrameworkErrorException( string message, Exception innerException ) : base( message, innerException ) { }
} ;


[DXSharpError("A DirectX graphics error occured in DXSharp.Framework.", 
			  "An unexpected problem occurred in the DirectX 12 graphics pipeline.")]
public class DirectXGraphicsErrorException: DXSharpException {
	public DirectXGraphicsErrorException( ) : base( ) { }
	public DirectXGraphicsErrorException( string message ) : base( message ) { }
	public DirectXGraphicsErrorException( string message, Exception innerException ) : base( message, innerException ) { }
} ;