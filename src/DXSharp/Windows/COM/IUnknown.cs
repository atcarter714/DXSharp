using System.Runtime.InteropServices ;
namespace DXSharp.Windows.COM ;


// -----------------------------------------------------------------
// COM Interfaces:
// -----------------------------------------------------------------

//! Import native COM IUnknown interface:
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
 ComImport, Guid("00000000-0000-0000-C000-000000000046"),]
public interface IUnknown {
	[PreserveSig] uint AddRef( ) ;
	[PreserveSig] uint Release( ) ;
	[PreserveSig] int  QueryInterface( ref Guid riid, out nint ppvObject ) ;
} ;

// -----------------------------------------------------------------