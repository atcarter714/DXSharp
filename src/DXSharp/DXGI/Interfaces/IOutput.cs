﻿#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;

// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput
// ------------------------------------------------------------------------------------------------
// https://learn.microsoft.com/en-us/windows/desktop/api/DXGI/nn-dxgi-idxgioutput

/// <summary>
/// An <a href="https://learn.microsoft.com/en-us/windows/desktop/api/DXGI/nn-dxgi-idxgioutput">IDXGIOutput interface</a> represents an adapter output (such as a monitor).
/// </summary>
/// <remarks>
/// Learn more about DXGI here: <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/_direct3ddxgi/">DirectX Graphics Infrastructure</a></para>
/// </remarks>
[ProxyFor(typeof(IDXGIOutput))]
public interface IOutput: IObject,
						  IInstantiable {
	// ---------------------------------------------------------------------------------
	
	void GetDescription( out OutputDescription pDescription ) ;
	
	void GetDisplayModeList( Format enumFormat,
							 uint flags,
							 out uint pNumModes,
							 out Span< ModeDescription > pDescription ) ;

	void FindClosestMatchingMode( in  ModeDescription pModeToMatch, 
								  out ModeDescription pClosestMatch,
								  IUnknownWrapper     pConcernedDevice ) ;

	void WaitForVBlank( ) ;
	
	void TakeOwnership( IUnknownWrapper pDevice, bool exclusive ) ;
	void ReleaseOwnership( ) ;
	
	void GetGammaControlCapabilities( out GammaControlCapabilities pGammaCaps ) ;
	void SetGammaControl( in GammaControl pGammaData ) ;
	void GetGammaControl( out GammaControl pGammaData ) ;
	
	void SetDisplaySurface<T>( T pScanoutSurface ) where T : class, ISurface ;
	void GetDisplaySurfaceData( ISurface pDestination ) ;
	
	void GetFrameStatistics( out FrameStatistics pStats ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(IDXGIOutput) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIOutput).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable. Instantiate( ) => new Output( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Output( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Output( (pComObj as IDXGIOutput)! ) ;
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput1
// https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nn-dxgi1_2-idxgioutput1
// ------------------------------------------------------------------------------------------------

/// <summary>
/// An <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nn-dxgi1_2-idxgioutput1">IDXGIOutput1 interface</a> represents an adapter output (such as a monitor).
/// </summary>
/// <remarks>
/// Learn more about DXGI here: <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/_direct3ddxgi/">DirectX Graphics Infrastructure</a></para>
/// </remarks>
[ProxyFor(typeof(IDXGIOutput1))]
public interface IOutput1: IOutput {
	// ---------------------------------------------------------------------------------
	
	void GetDisplayModeList1( Format enumFormat,
							  uint flags,
							  out uint pNumModes,
							  out Span< ModeDescription1 > pDescription ) ;
	
	void FindClosestMatchingMode1( in  ModeDescription1 pModeToMatch, 
								   out ModeDescription1 pClosestMatch,
								   Direct3D12.IDevice   pConcernedDevice ) ;
	
	void GetDisplaySurfaceData1( IResource pDestination ) ;
	
	void DuplicateOutput( IDevice pDevice, out IOutputDuplication? ppOutputDuplication ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(IDXGIOutput1) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIOutput1).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable. Instantiate( ) => new Output1( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Output1( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Output1( (pComObj as IDXGIOutput1)! ) ;
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput2
// https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nn-dxgi1_3-idxgioutput2
// ------------------------------------------------------------------------------------------------

/// <summary>
/// An <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nn-dxgi1_3-idxgioutput2">IDXGIOutput2 interface</a> represents an adapter output (such as a monitor).
/// </summary>
/// <remarks>
/// Learn more about DXGI here: <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/_direct3ddxgi/">DirectX Graphics Infrastructure</a></para>
/// </remarks>
[ProxyFor(typeof(IDXGIOutput2))]
public interface IOutput2: IOutput1,
						   IComObjectRef< IDXGIOutput2 >,
						   IUnknownWrapper< IDXGIOutput2 > {
	// ---------------------------------------------------------------------------------
	
	/// <summary>Queries an adapter output for multiplane overlay support.</summary>
	/// <returns>TRUE if the output adapter is the primary adapter and it supports multiplane overlays, otherwise returns FALSE.</returns>
	/// <remarks>See <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcorewindow">CreateSwapChainForCoreWindow</a> for info on creating a foreground swap chain.</remarks>
	bool SupportsOverlays( ) ;
	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof( IDXGIOutput2 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput2 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput3
// https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nn-dxgi1_3-idxgioutput3
// ------------------------------------------------------------------------------------------------


/// <summary>
/// An <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nn-dxgi1_3-idxgioutput3">IDXGIOutput3 interface</a> represents an adapter output (such as a monitor).
/// </summary>
/// <remarks>
/// Learn more about DXGI here: <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/_direct3ddxgi/">DirectX Graphics Infrastructure</a></para>
/// </remarks>
[ProxyFor(typeof(IDXGIOutput3))]
public interface IOutput3: IOutput2 {
	// ---------------------------------------------------------------------------------
	
	/// <summary>Checks for overlay support.</summary>
	/// <param name="enumFormat">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value for the color format.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgioutput3-checkoverlaysupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pConcernedDevice">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> A pointer to the Direct3D device interface. <b>CheckOverlaySupport</b> returns only support info about this scan-out device.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgioutput3-checkoverlaysupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pFlags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> A pointer to a variable that receives a combination of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_3/ne-dxgi1_3-dxgi_overlay_support_flag">DXGI_OVERLAY_SUPPORT_FLAG</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies options for overlay support.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgioutput3-checkoverlaysupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the error codes described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgioutput3-checkoverlaysupport">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CheckOverlaySupport( Format enumFormat, Direct3D12.IDevice pConcernedDevice, out OverlaySupportFlag pFlags ) ;
	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof( IDXGIOutput3 ) ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput3 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput4
// https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_4/nn-dxgi1_4-idxgioutput4
// ------------------------------------------------------------------------------------------------


/// <summary>
/// An <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_4/nn-dxgi1_4-idxgioutput4">IDXGIOutput4 interface</a> represents an adapter output (such as a monitor).
/// </summary>
/// <remarks>
/// Learn more about DXGI here: <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/_direct3ddxgi/">DirectX Graphics Infrastructure</a></para>
/// </remarks>
[ProxyFor(typeof(IDXGIOutput4))]
public interface IOutput4: IOutput3 {
	/// <summary>Checks for overlay color space support.</summary>
	/// <param name="format">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value for the color format.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgioutput4-checkoverlaycolorspacesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="colorSpace">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ne-dxgicommon-dxgi_color_space_type">DXGI_COLOR_SPACE_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ne-dxgicommon-dxgi_color_space_type">DXGI_COLOR_SPACE_TYPE</a>-typed value that specifies color space type to check overlay support for.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgioutput4-checkoverlaycolorspacesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pConcernedDevice">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> A pointer to the Direct3D device interface. <b>CheckOverlayColorSpaceSupport</b> returns only support info about this scan-out device.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgioutput4-checkoverlaycolorspacesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pFlags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> A pointer to a variable that receives a combination of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/ne-dxgi1_4-dxgi_overlay_color_space_support_flag">DXGI_OVERLAY_COLOR_SPACE_SUPPORT_FLAG</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies options for overlay color space support.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgioutput4-checkoverlaycolorspacesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>S_OK</b> on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgioutput4-checkoverlaycolorspacesupport">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CheckOverlayColorSpaceSupport( Format format,
										ColorSpaceType colorSpace, 
										Direct3D12.IDevice pConcernedDevice, 
										out OverlayColorSpaceSupportFlag pFlags ) ;

	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof( IDXGIOutput4 ) ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput4 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput5
// https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_5/nn-dxgi1_5-idxgioutput5
// ------------------------------------------------------------------------------------------------


/// <summary>
/// An <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_5/nn-dxgi1_5-idxgioutput5">IDXGIOutput5 interface</a> represents an adapter output (such as a monitor).
/// </summary>
/// <remarks>
/// Learn more about DXGI here: <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/_direct3ddxgi/">DirectX Graphics Infrastructure</a></para>
/// </remarks>
[ProxyFor(typeof(IDXGIOutput5))]
public interface IOutput5: IOutput4 {

	/// <summary>Allows specifying a list of supported formats for fullscreen surfaces that can be returned by the IDXGIOutputDuplication object.</summary>
	/// <param name="pDevice">
	/// <para>Type: <b>IUnknown*</b> A pointer to the Direct3D device interface that you can use to process the desktop image. This device must be created from the adapter to which the output is connected.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgioutput5-duplicateoutput1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Flags">
	/// <para>Type: <b>UINT</b> Reserved for future use; must be zero.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgioutput5-duplicateoutput1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="SupportedFormatsCount">
	/// <para>Type: <b>UINT</b> Specifies the number of supported formats.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgioutput5-duplicateoutput1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pSupportedFormats">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>*</b> Specifies an array, of length  <i>SupportedFormatsCount</i> of  <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a> entries.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgioutput5-duplicateoutput1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppOutputDuplication">
	/// <para>Type: <b>IDXGIOutputDuplication**</b> A pointer to a variable that receives the new <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgioutputduplication">IDXGIOutputDuplication</a> interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgioutput5-duplicateoutput1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b></para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method allows directly receiving the original back buffer format used by a running fullscreen application. For comparison, using the original <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutput1-duplicateoutput">DuplicateOutput</a> function always converts the fullscreen surface to a 32-bit BGRA format. In cases where the current fullscreen application is using a different buffer format, a conversion to 32-bit BGRA incurs a performance penalty. Besides the performance benefit of being able to skip format conversion, using <b>DuplicateOutput1</b> also allows receiving the full gamut of colors in cases where a high-color format (such as R10G10B10A2) is being presented.</para>
	/// <para>The <i>pSupportedFormats</i> array should only contain display scan-out formats. See <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/format-support-for-direct3d-11-0-feature-level-hardware">Format Support for Direct3D Feature Level 11.0 Hardware</a> for  required scan-out formats at each feature level. If the current fullscreen buffer format is not contained in the <i>pSupportedFormats</i> array, DXGI will pick one of the supplied formats and convert the fullscreen buffer to that format before returning from <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-acquirenextframe">IDXGIOutputDuplication::AcquireNextFrame</a>. The list of supported formats should always contain DXGI_FORMAT_B8G8R8A8_UNORM, as this is the most common format for the desktop.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgioutput5-duplicateoutput1#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	HResult DuplicateOutput1( Direct3D12.IDevice      pDevice,
							  uint                    Flags,
							  uint                    SupportedFormatsCount,
							  in  Span< Format >      pSupportedFormats,
							  out IOutputDuplication? ppOutputDuplication ) ;

	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof( IDXGIOutput5 ) ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput5 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput6
// https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_6/nn-dxgi1_6-idxgioutput6
// ------------------------------------------------------------------------------------------------


/// <summary>
/// An <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_6/nn-dxgi1_6-idxgioutput6">IDXGIOutput6 interface</a> represents an adapter output (such as a monitor).
/// </summary>
/// <remarks>
/// Learn more about DXGI here: <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/_direct3ddxgi/">DirectX Graphics Infrastructure</a></para>
/// </remarks>
[ProxyFor(typeof(IDXGIOutput6))]
public interface IOutput6: IOutput5 {
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof( IDXGIOutput6 ) ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput6 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;

