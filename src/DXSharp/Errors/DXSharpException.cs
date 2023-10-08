﻿#region Using Directives
using System.Runtime.InteropServices ;
using System.Runtime.Serialization ;
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


public class COMErrorException: DXSharpException {
	public COMException? COMError { get ; init ; }
	public HResult? APICallReturnValue { get ; init ; }
	
	public COMErrorException( COMException? comError = null ) => COMError = comError ;
	public COMErrorException( string message, COMException? comError = null ): 
		base(message) => COMError = comError ;
	public COMErrorException( SerializationInfo info, StreamingContext context, COMException? comError = null ): 
		base(info, context) => COMError = comError ;
} ;