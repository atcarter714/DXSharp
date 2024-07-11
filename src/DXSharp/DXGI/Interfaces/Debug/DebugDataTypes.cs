using System.Runtime.Versioning ;
using Windows.Win32.Graphics.Dxgi ;

namespace DXSharp.DXGI.Debugging ;


/// <summary>Flags used with ReportLiveObjects to specify the amount of info to report about an object's lifetime.</summary>
/// <remarks>
/// <para>Use this enumeration with
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/nf-dxgidebug-idxgidebug-reportliveobjects">IDXGIDebug::ReportLiveObjects</a>.</para>
/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/ne-dxgidebug-dxgi_debug_rlo_flags#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[SupportedOSPlatform( "windows8.0" )]
[Flags, EquivalentOf(typeof(DXGI_DEBUG_RLO_FLAGS))]
public enum DebugRLOFlags {
	None    = 0x00000000,
	/// <summary>A flag that specifies to obtain a summary about an object's lifetime.</summary>
	Summary = 0x00000001,
	/// <summary>A flag that specifies to obtain detailed info about an object's lifetime.</summary>
	Detail = 0x00000002,
	/// <summary>
	/// <para>This flag indicates to ignore objects which have no external refcounts keeping them alive. D3D objects are printed using an external refcount and an internal refcount. Typically, all objects are printed. This flag means ignore the objects whose external refcount is 0, because the application is not responsible for keeping them alive.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/ne-dxgidebug-dxgi_debug_rlo_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	IgnoreInternal = 0x00000004,
	/// <summary>A flag that specifies to obtain both a summary and detailed info about an object's lifetime.</summary>
	All = 0x00000007,
} ;