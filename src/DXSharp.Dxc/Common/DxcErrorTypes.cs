#region Using Directives
using System.Runtime.InteropServices ;
using DXSharp.Windows ;
#endregion
namespace DXSharp.Dxc ;


/// <summary>A general-purpose exception thrown by DXSharp.Dxc when an error occurs.</summary>
public class DxcErrorException: DXSharpException {
	public DxcErrorException( string? message = null ): 
		base( message ?? $"{nameof(DxcErrorException)}" ) { }
	
	public DxcErrorException( string message, Exception innerException ): 
		base( message, innerException ) { }
} ;


/// <summary>An exception thrown by DXSharp.Dxc when a COM error occurs.</summary>
public class DxcComError: DirectXComError {
	public DxcComError( string? message = null ):
		base( message ?? $"{nameof(DxcComError)}" ) { }
	
	public DxcComError( HResult hr, string message, COMException? innerException = null ):
		base( hr, message, innerException ) { }
} ;