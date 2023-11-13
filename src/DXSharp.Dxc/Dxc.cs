#region Using Directives
using System ;
using System.Reflection ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.System.Com ;
using Windows.Win32.Graphics.Direct3D.Dxc ;
using Windows.Win32.Graphics.DXCore ;
using cswinDxc = Windows.Win32.Graphics.Direct3D.Dxc ;

using DXSharp.Windows ;
#endregion
namespace DXSharp.Dxc ;


/// <summary>
/// Exposes methods that create Direct3D shader compiler objects.
/// </summary>
public static partial class Dxc {
	const string DxcLibraryName = "dxcompiler.dll", DxilLibraryName = "dxil.dll" ;
	internal static nint DxcDll, DxilDll ;
	static Dxc( ) {
		bool dxcLoaded  = NativeLibrary.TryLoad( DxcLibraryName, out nint dxcompilerDll ) ;
		bool dxilLoaded = NativeLibrary.TryLoad( DxilLibraryName, out nint dxilDll ) ;
		
		if( !dxcLoaded ) {
			throw new DllNotFoundException( $"{nameof(Dxc)} :: Unable to load dxcompiler.dll" ) ;
		}
		DxcDll = dxcompilerDll ;

		if( !dxilLoaded ) {
			throw new DllNotFoundException( $"{nameof(Dxc)} :: Unable to load dxil.dll" ) ;
		}
		DxilDll = dxilDll ;
	}
	// -----------------------------------------------------------------------------------------------------------------
	
	public static HResult CreateInstance( in Guid clsid, in Guid iid, out object ppv ) {
		unsafe {
			fixed ( Guid* clsidPtr = &clsid, iidPtr = &iid ) {
				HResult hr = PInvoke.DxcCreateInstance( clsidPtr, iidPtr,
														out ppv ) ;
				return hr ;
			}
		}
	}
	
	public static HResult CreateInstance2( IMalloc pMalloc, in Guid clsid, in Guid iid, out object ppv ) {
		unsafe {
			fixed ( Guid* clsidPtr = &clsid, iidPtr = &iid ) {
				HResult hr = PInvoke.DxcCreateInstance2( pMalloc, clsidPtr, iidPtr,
														 out ppv ) ;
				return hr ;
			}
		}
	}
	
	
	// -----------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// <para>Exposes the CLSIDs of the Direct3D shader compiler objects.</para>
	/// <para>
	/// These are the "magic" <see cref="Guid"/> values defined in the <i>dxcapi.h</i> header file
	/// which identify a particular COM class.
	/// </para>
	/// </summary>
	/// <remarks>
	/// <para>For more information about COM class identifiers, see:</para>
	/// <a href="https://learn.microsoft.com/en-us/windows/win32/com/com-class-objects-and-clsids">COM Class Objects and CLSIDs</a>
	/// </remarks>
	public static class CLSIDs {
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcCompiler"/> type.</summary>
		public static readonly Guid Compiler = new( 0x73e22d93, 0xe6ce, 0x47f3, 0xb5, 0xbf, 
													   0xf0, 0x66, 0x4f, 0x39, 0xc1, 0xb0) ;
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcLinker"/> type.</summary>
		public static readonly Guid Linker = new( 0xef6a8087, 0xb0ea, 0x4d56, 0x9e, 0x45, 
													 0xd0, 0x7e, 0x1a, 0x8b, 0x78, 0x06) ;
		
		
		public static readonly Guid DxcDiaDataSource = new( 0xcd1f6b73, 0x2ab0, 0x484d, 0x8e, 
															0xdc, 0xeb, 0xe7, 0xa4, 0x3c, 0xa0, 0x9f) ;
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcAssembler"/> type.</summary>
		public static readonly Guid DxcCompilerArgs = new( 0x3e56ae82, 0x224d, 0x470f, 0xa1, 0xa1, 
														   0xfe, 0x30, 0x16, 0xee, 0x9f, 0x9d) ;
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcLibrary"/> type.</summary>
		public static readonly Guid DxcLibrary = new( 0x6245d6af, 0x66e0, 0x48fd, 0x80, 0xb4, 
													  0x4d, 0x27, 0x17, 0x96, 0x74, 0x8c) ;
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcUtils"/> type.</summary>
		public static readonly Guid DxcUtils = DxcLibrary; //! Alias for CLSID_DxcLibrary
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcValidator"/> type.</summary>
		public static readonly Guid DxcValidator = new( 0x8ca3e215, 0xf728, 0x4cf3, 0x8c, 0xdd, 
														0x88, 0xaf, 0x91, 0x75, 0x87, 0xa1) ;
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcContainerReflection"/> type.</summary>
		public static readonly Guid DxcAssembler = new( 0xd728db68, 0xf903, 0x4f80, 0x94, 0xcd, 
														0xdc, 0xcf, 0x76, 0xec, 0x71, 0x51) ;
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcOptimizer"/> type.</summary>
		public static readonly Guid DxcContainerReflection = new( 0xb9f54489, 0x55b8, 0x400c, 0xba, 0x3a, 
																  0x16, 0x75, 0xe4, 0x72, 0x8b, 0x91) ;
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcOptimizer"/> type.</summary>
		public static readonly Guid DxcOptimizer = new( 0xae2cd79f, 0xcc22, 0x453f, 0x9b, 0x6b, 
														0xb1, 0x24, 0xe7, 0xa5, 0x20, 0x4c) ;
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcContainerBuilder"/> type.</summary>
		public static readonly Guid DxcContainerBuilder = new( 0x94134294, 0x411f, 0x4574, 0xb4, 0xd0, 
															   0x87, 0x41, 0xe2, 0x52, 0x40, 0xd2) ;
		
		/// <summary>The CLSID of the <see cref="cswinDxc.IDxcPdbUtils"/> type.</summary>
		public static readonly Guid DxcPdbUtils = new( 0x54621dfb, 0xf2ce, 0x457e, 0xae, 0x8c, 
													   0xec, 0x35, 0x5f, 0xae, 0xec, 0x7c) ;
	}
	// =================================================================================================================
} ;


/*
 * DxcCreateInstance:
	Creates a single uninitialized object of the class associated with a specified CLSID.
 
 * DxcCreateInstance2:
	Creates a single uninitialized object of the class associated with a specified CLSID (can be used to create an instance of the compiler with a custom memory allocator).
 */


/*
 DXC_API_IMPORT HRESULT DxcCreateInstance(
										  REFCLSID rclsid,
										  REFIID   riid,
										  LPVOID   *ppv
										) ;

DXC_API_IMPORT HRESULT DxcCreateInstance2(
										  IMalloc  *pMalloc,
										  REFCLSID rclsid,
										  REFIID   riid,
										  LPVOID   *ppv
										) ;
 */


internal static unsafe class DxcInterop {
	const string DxcLibrary = "dxcompiler.dll" ;

	[DllImport(DxcLibrary, EntryPoint = nameof(DxcCreateInstance),
			   CallingConvention = CallingConvention.StdCall, PreserveSig = true)]
	public static extern HResult DxcCreateInstance(
			Guid* rclsid,
			Guid* riid,
			[MarshalAs(UnmanagedType.LPStruct)] out nint ppv
		) ;

	
	[DllImport(DxcLibrary, EntryPoint = nameof(DxcCreateInstance2),
			   CallingConvention = CallingConvention.StdCall, PreserveSig = true)]
	public static extern HResult DxcCreateInstance2(
			[MarshalAs(UnmanagedType.Interface)] nint pMalloc,
			Guid* rclsid,
			Guid* riid,
			[MarshalAs(UnmanagedType.LPStruct)] out nint ppv
		) ;
}
