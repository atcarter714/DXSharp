#region Using Directives
using System.Runtime.InteropServices ;
using System.Runtime.Serialization ;
using Windows.Security.ExchangeActiveSyncProvisioning ;
using DXSharp.Windows ;
#endregion


namespace DXSharp ;

public class DXSharpException: Exception {
	public DXSharpException( ): base( ) { }
	public DXSharpException( string message ): base( message ) { }
	public DXSharpException( string message, Exception inner ): base( message, inner ) { }
	public DXSharpException( SerializationInfo info, StreamingContext context ): 
		base( info, context ) { }
} ;


public class DirectXComError: DXSharpException {
	public COMException? COMError { get ; init ; }
	public HResult? APICallReturnValue { get ; init ; }

	public DirectXComError( HResult hResult, string? message = null ): base( message ) {
		APICallReturnValue = hResult ;
		if ( message is not null )
			base.Data.Add( "Message", message ) ;
	}
	public DirectXComError( HResult hResult, string? message = null, COMException? comError = null ): base( message ) {
		APICallReturnValue = hResult ;
		COMError           = comError ;
		if ( message is not null )
			base.Data.Add( "Message", message ) ;
	}
	public DirectXComError( COMException? comError = null ) => COMError = comError ;
	public DirectXComError( string message, COMException? comError = null ): 
		base( message ) => COMError = comError ;
	public DirectXComError( SerializationInfo info, StreamingContext context, COMException? comError = null ): 
		base( info, context ) => COMError = comError ;
} ;