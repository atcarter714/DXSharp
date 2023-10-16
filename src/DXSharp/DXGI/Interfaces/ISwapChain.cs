#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using System.Runtime.Versioning ;
using Windows.Win32.Foundation ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


// -----------------------------------------------
// Version: IDXGISwapChain
// -----------------------------------------------

[ProxyFor(typeof(IDXGISwapChain))]
public interface ISwapChain: IDeviceSubObject,
							 IComObjectRef< IDXGISwapChain >,
							 IUnknownWrapper< IDXGISwapChain >,
							 IInstantiable {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGISwapChain >? ComPointer { get ; }
	new IDXGISwapChain? COMObject => ComPointer?.Interface ;
	IDXGISwapChain? IComObjectRef< IDXGISwapChain >.COMObject => COMObject ;

	static IDXCOMObject IInstantiable.Instantiate( ) => new SwapChain( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new SwapChain( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new SwapChain( (IDXGISwapChain)pComObj! ) ;
	// ==================================================================================
	
	
	void GetDesc( out SwapChainDescription pDesc ) ;
	void Present( uint syncInterval, PresentFlags flags ) ;
	void GetBuffer( uint buffer, out Direct3D12.IResource? pSurface ) ;
	void SetFullscreenState( bool fullscreen, in IOutput? pTarget ) ;
	void GetFullscreenState( out bool pFullscreen, out IOutput? ppTarget ) ;
	
		
	void ResizeBuffers( uint bufferCount, uint width, uint height,
						Format newFormat, SwapChainFlags swapChainFlags ) ;
	
	void ResizeTarget( in ModeDescription newTargetParameters ) ;
	
	uint GetLastPresentCount( ) ;
	IOutput? GetContainingOutput( ) ;
	void GetFrameStatistics( out FrameStatistics pStats ) ;
	
} ;


// -----------------------------------------------
// Version: IDXGISwapChain1
// -----------------------------------------------

public interface ISwapChain1: ISwapChain,
							  IComObjectRef< IDXGISwapChain1 >,
							  IUnknownWrapper< IDXGISwapChain1 > {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGISwapChain1 > ComPointer { get ; }
	new IDXGISwapChain1? COMObject => ComPointer?.Interface ;
	IDXGISwapChain1? IComObjectRef< IDXGISwapChain1 >.COMObject => COMObject ;

	static IDXCOMObject IInstantiable.Instantiate( ) => new SwapChain1( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new SwapChain1( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new SwapChain1( (IDXGISwapChain1)pComObj! ) ;
	// ==================================================================================
	
	
	void GetDesc1( out SwapChainDescription1 pDesc ) ;
	void GetFullscreenDesc( out SwapChainFullscreenDescription pDesc ) ;
	
	void GetHwnd( out HWND pHwnd ) ;
	void GetCoreWindow( Guid riid, out IUnknown? ppUnk ) ;
	
	void Present1( uint syncInterval, PresentFlags flags,
				   in PresentParameters pPresentParameters ) ;
	
	bool IsTemporaryMonoSupported( ) ;
	
	void GetRestrictToOutput( out IOutput ppRestrictToOutput ) ;
	
	void SetBackgroundColor( in RGBA pColor ) ;
	void GetBackgroundColor( out RGBA pColor ) ;
	
	void SetRotation( ModeRotation rotation ) ;
	void GetRotation( out ModeRotation pRotation ) ;
} ;

// -----------------------------------------------
// Version: IDXGISwapChain2
// -----------------------------------------------


public interface ISwapChain2: ISwapChain1,
							  IComObjectRef< IDXGISwapChain2 >,
							  IUnknownWrapper< IDXGISwapChain2 > {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGISwapChain2 > ComPointer { get ; }
	new IDXGISwapChain2? COMObject => ComPointer?.Interface ;
	IDXGISwapChain2? IComObjectRef< IDXGISwapChain2 >.COMObject => COMObject ;

	static IDXCOMObject IInstantiable.Instantiate( ) => new SwapChain2( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new SwapChain2( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new SwapChain2( (IDXGISwapChain2)pComObj! ) ;
	// ==================================================================================

	
	
} ;


[SupportedOSPlatform("windows10.0.10240")]
public interface ISwapChain3: ISwapChain2,
							  IComObjectRef< IDXGISwapChain3 >,
							  IUnknownWrapper< IDXGISwapChain3 > {
	
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGISwapChain3 > ComPointer { get ; }
	new IDXGISwapChain3? COMObject => ComPointer?.Interface ;
	IDXGISwapChain3? IComObjectRef< IDXGISwapChain3 >.COMObject => COMObject ;

	static IDXCOMObject IInstantiable.Instantiate( ) => new SwapChain3( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new SwapChain3( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new SwapChain3( (IDXGISwapChain3)pComObj! ) ;
	// ==================================================================================
} ;



/// <summary>
/// Extends <see cref="ISwapChain3"/> with an additional method to set video metadata.
/// </summary>
/// <remarks>
/// <b>WARNING</b>: <para/>
/// It is no longer recommended for apps to explicitly set HDR metadata on their swap chain using SetHDRMetaData.
/// Windows does not guarantee that swap chain metadata is sent to the monitor, and monitors do not handle HDR
/// metadata consistently. Therefore it's recommended that apps always tone-map content into the range reported
/// by the monitor. For more details on how to write apps that react dynamically to monitor capabilities, see
/// <a href="https://docs.microsoft.com/windows/win32/direct3darticles/high-dynamic-range">
/// Using DirectX with high dynamic range displays and Advanced Color.
/// </a>
/// </remarks>
[ProxyFor(typeof(IDXGISwapChain4))]
public interface ISwapChain4: ISwapChain3,
							  IComObjectRef< IDXGISwapChain4 >,
							  IUnknownWrapper< IDXGISwapChain4 > {
	new ComPtr< IDXGISwapChain4 > ComPointer { get ; }
	

	/// <summary>This method sets High Dynamic Range (HDR) and Wide Color Gamut (WCG) header metadata.</summary>
	/// <param name="Type">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_hdr_metadata_type">DXGI_HDR_METADATA_TYPE</a></b> Specifies one member of the  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_hdr_metadata_type">DXGI_HDR_METADATA_TYPE</a> enum.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Size">
	/// <para>Type: <b>UINT</b> Specifies the size of <i>pMetaData</i>, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pMetaData">
	/// <para>Type: <b>void*</b> Specifies a void pointer that references the metadata, if it exists. Refer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ns-dxgi1_5-dxgi_hdr_metadata_hdr10">DXGI_HDR_METADATA_HDR10</a> structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method sets metadata to enable a monitor's output to be adjusted depending on its capabilities. However it does not change how pixel values are interpreted by Windows or monitors. To adjust the color space of the swap chain, use [**SetColorSpace1**](..\dxgi1_4\nf-dxgi1_4-idxgiswapchain3-setcolorspace1.md) instead. Applications should not rely on the metadata being sent to the monitor as the the metadata may be ignored. Monitors do not consistently process HDR metadata, resulting in varied appearance of your content across different monitors. In order to ensure more consistent output across a range of monitors, devices, and use cases, it is recommended to not use **SetHDRMetaData** and to instead tone-map content into the gamut and luminance range supported by the monitor. See [IDXGIOutput6::GetDesc1](../dxgi1_6/nf-dxgi1_6-idxgioutput6-getdesc1.md) to retrieve the monitor's supported gamut and luminance range. Monitors adhering to the VESA DisplayHDR standard will automatically perform a form of clipping for content outside of the monitor's supported gamut and luminance range. For more details on how to write apps that react dynamically to monitor capabilities, see [Using DirectX with high dynamic range displays and Advanced Color](/windows/win32/direct3darticles/high-dynamic-range).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	unsafe void SetHDRMetaData( DXGI_HDR_METADATA_TYPE Type, 
								uint Size,
								[Optional] void* pMetaData ) ;
} ;



/// <summary>Specifies the header metadata type.</summary>
/// <remarks>
/// This enum is used by the
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata">SetHDRMetaData</a> method.
/// </remarks>
[EquivalentOf( typeof( DXGI_HDR_METADATA_TYPE ) )]
public enum HDRMetaDataType {
	/// <summary>Indicates there is no header metadata.</summary>
	NONE = 0,

	/// <summary>Indicates the header metadata is held by a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ns-dxgi1_5-dxgi_hdr_metadata_hdr10">DXGI_HDR_METADATA_HDR10</a> structure.</summary>
	HDR10 = 1,

	HDR10PLUS = 2,
} ;
	

