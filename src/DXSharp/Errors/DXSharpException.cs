#region Using Directives
using System.Runtime.InteropServices ;
using System.Runtime.Serialization ;
using Windows.Security.ExchangeActiveSyncProvisioning ;
using Windows.Win32 ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;

#endregion
namespace DXSharp ;


// --------------------------------------------------------------------
// Exception Helper Types ::
// --------------------------------------------------------------------

/// <summary>Attribute which marks DXSharp exception/error types with additional information.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class DXSharpErrorAttribute: Attribute {
	// -----------------------------------------------------------------
	public static readonly Type[ ] DefaultOriginators = { typeof(object), } ;
	public const string DefaultDescription = "A DXSharpException error with no description.",
						DefaultCause       = "No cause was specified." ;
	// -----------------------------------------------------------------
	
	
	/// <summary>Short description of the error.</summary>
	public string? Description { get ; init ; }
	/// <summary>Types that causes the error.</summary>
	public Type[ ]? Originators { get ; init ; }
	/// <summary>Short description of the cause of the error.</summary>
	public string? Cause { get ; init ; }

	// -----------------------------------------------------------------
	public DXSharpErrorAttribute( ) {
		this.Description = string.Empty ;
		this.Originators = DefaultOriginators ;
		this.Cause       = string.Empty ;
	}
	public DXSharpErrorAttribute( string? description = null,
										  string? cause = null,
											params Type[ ]? originators ) {
		this.Description = description ;
		this.Originators = originators ;
		this.Cause       = cause ;
	}
	// =================================================================
} ;



// --------------------------------------------------------------------
// Exception Base Classes ::
// --------------------------------------------------------------------

[DXSharpError( "DXSharpException is the base class for all DXSharp errors." )]
public class DXSharpException: Exception {
	public DXSharpException( ): base( ) { }
	public DXSharpException( string message ): base( message ) { }
	public DXSharpException( string message, Exception inner ): base( message, inner ) { }
	public DXSharpException( SerializationInfo info, StreamingContext context ): 
		base( info, context ) { }
} ;


[DXSharpError( "DirectXComError is the base class for all DirectX COM errors.",
			   "When a COM API call or operation fails.", 
			   typeof(IUnknown), typeof(COMUtility),
			   typeof(D3D12), typeof(DXGIFunctions) )]
public class DirectXComError: DXSharpException {
	public COMException? COMError { get ; init ; }
	public HResult? APICallReturnValue { get ; init ; }

	public DirectXComError( HResult hResult, string? message = null ): 
									base( message ?? hResult.ToString() ) {
		APICallReturnValue = hResult ;
		if ( message is not null )
			base.Data.Add( "Message", message ) ;
	}
	public DirectXComError( HResult hResult, string? message = null, COMException? comError = null ): base( message ?? hResult.ToString() ) {
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


[DXSharpError( "Win32InteropException is the base class for Win32 interop errors.",
			   "When a Windows API call or operation fails.", 
			   typeof(PInvoke), typeof(InteropUtils), typeof(COMUtility), 
			   typeof(D3D12), typeof(DXGIFunctions) )]
public class Win32InteropException: DXSharpException {
	public HResult? Win32Error { get ; init ; }
	public bool IsWin32Error => Win32Error is not null ;
	
	public Guid? IID { get ; init ; }
	public bool IsCOMError => IID is not null ;
	
	
	public Win32InteropException( HResult? hResult = default,
								  string? message = null, Guid? iid = null ): 
									base( message ?? $"[ HR: {hResult?.ToString() ?? "???"}, " +
										  $"IID: {iid?.ToString() ?? "???"} ]" ) {
		Win32Error = hResult ;
		IID        = iid ;
	}
	
	public Win32InteropException( string? message = null, 
								  HResult? hResult = default, 
								  Guid? iid = null ):
									base( message ?? $"[ HR: {hResult?.ToString() ?? "???"}, " +
										  $"IID: {iid?.ToString() ?? "???"} ]" ) {
		Win32Error = hResult ;
		IID        = iid ;
	}
	
	public Win32InteropException( SerializationInfo info, 
								  StreamingContext context,
								  HResult hResult, Guid? iid = null ): 
									base( info, context ) {
		Win32Error = hResult ; IID = iid ;
	}
} ;