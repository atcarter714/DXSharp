#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.InteropServices.ComTypes ;
using System.Runtime.InteropServices.Marshalling ;

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
	
	[UnmanagedCallConv( CallConvs = new[ ] {
		typeof( CallConvStdcall ),
		typeof( CallConvThiscall ),
		typeof( CallConvMemberFunction ),
	} )]
	[PreserveSig] uint AddRef( ) ;
	
	[UnmanagedCallConv( CallConvs = new[ ] {
		typeof( CallConvStdcall ),
		typeof( CallConvMemberFunction ),
	} )]
	[PreserveSig] uint Release( ) ;
	
	[UnmanagedCallConv( CallConvs = new[ ] {
		typeof( CallConvStdcall ),
		typeof( CallConvMemberFunction ),
	} )]
	[PreserveSig] HResult QueryInterface( ref Guid riid,
										  [MarshalAs(UnmanagedType.IUnknown)] 
											out nint ppvObject ) ;
} ;

// -----------------------------------------------------------------


