#region Using Directives
using System.Runtime.InteropServices ;
#endregion
namespace DXSharp.Windows.COM ;


// -----------------------------------------------------------------
// COM RCW Interfaces:
// -----------------------------------------------------------------
//! TODO: Figure out why calling any of these methods causes AccessViolationException


[NativeLibrary(null, "IUnknown", "unknwn.h", "COM")]
[ComImport, System.Runtime.InteropServices.Guid("00000000-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ]
public interface IUnknown {
	[PreserveSig] uint AddRef( ) ;
	[PreserveSig] uint Release( ) ;
	[PreserveSig] HResult QueryInterface( ref Guid riid,
										  [MarshalAs(UnmanagedType.IUnknown)] 
											out nint ppvObject ) ;
} ;

// -----------------------------------------------------------------


