#region Using Directives

using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning;
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
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	[SuppressMessage( "Interoperability", "CA1416:Validate platform compatibility" )] 
	internal static readonly ReadOnlyDictionary< Guid, Func<IDXGIOutput, IInstantiable> > _outputCreationFunctions =
		new( new Dictionary< Guid, Func<IDXGIOutput, IInstantiable> > {
			{ IOutput.IID, ( pComObj ) => new Output( pComObj ) },
			{ IOutput1.IID, ( pComObj ) => new Output1( (pComObj as IDXGIOutput1)! ) },
			{ IOutput2.IID, ( pComObj ) => new Output2( (pComObj as IDXGIOutput2)! ) },
			{ IOutput3.IID, ( pComObj ) => new Output3( (pComObj as IDXGIOutput3)! ) },
			{ IOutput4.IID, ( pComObj ) => new Output4( (pComObj as IDXGIOutput4)! ) },
			{ IOutput5.IID, ( pComObj ) => new Output5( (pComObj as IDXGIOutput5)! ) },
			{ IOutput6.IID, ( pComObj ) => new Output6( (pComObj as IDXGIOutput6)! ) },
		} ) ;
	// ---------------------------------------------------------------------------------
	
	/// <inheritdoc cref="IDXGIOutput.GetDesc"/>
	void GetDescription( out OutputDescription pDescription ) ;
	
	/// <inheritdoc cref="IDXGIOutput.GetDisplayModeList"/>
	void GetDisplayModeList( Format enumFormat,
							 EnumModesFlags flags,
							 out uint pNumModes,
							 out Span< ModeDescription > pDescription ) ;

	/// <inheritdoc cref="IDXGIOutput.FindClosestMatchingMode"/>
	void FindClosestMatchingMode( in  ModeDescription pModeToMatch, 
								  out ModeDescription pClosestMatch,
								  IUnknownWrapper     pConcernedDevice ) ;

	/// <inheritdoc cref="IDXGIOutput.WaitForVBlank"/>
	void WaitForVBlank( ) ;
	
	/// <inheritdoc cref="IDXGIOutput.TakeOwnership"/>
	void TakeOwnership( IUnknownWrapper pDevice, bool exclusive ) ;
	
	/// <inheritdoc cref="IDXGIOutput.ReleaseOwnership"/>
	void ReleaseOwnership( ) ;
	
	/// <inheritdoc cref="IDXGIOutput.GetGammaControlCapabilities"/>
	void GetGammaControlCapabilities( out GammaControlCapabilities pGammaCaps ) ;
	
	/// <inheritdoc cref="IDXGIOutput.SetGammaControl"/>
	void SetGammaControl( in GammaControl pGammaData ) ;
	
	/// <inheritdoc cref="IDXGIOutput.GetGammaControl"/>
	void GetGammaControl( out GammaControl pGammaData ) ;
	
	/// <inheritdoc cref="IDXGIOutput.SetDisplaySurface"/>
	void SetDisplaySurface< T >( T pScanoutSurface ) where T: ISurface, IInstantiable ;
	
	/// <inheritdoc cref="IDXGIOutput.GetDisplaySurfaceData"/>
	void GetDisplaySurfaceData( ISurface pDestination ) ;
	
	/// <inheritdoc cref="IDXGIOutput.GetFrameStatistics"/>
	void GetFrameStatistics( out FrameStatistics pStats ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGIOutput ) ;
	public new static Guid IID => (ComType.GUID) ;
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
[SupportedOSPlatform( "windows8.0" )]
[ProxyFor(typeof(IDXGIOutput1))]
public interface IOutput1: IOutput {
	// ---------------------------------------------------------------------------------
	/// <inheritdoc cref="IDXGIOutput1.GetDisplayModeList1"/>
	void GetDisplayModeList1( Format enumFormat,
							  EnumModesFlags flags,
							  out uint pNumModes,
							  out Span< ModeDescription1 > pDescription ) ;
	
	/// <inheritdoc cref="IDXGIOutput1.FindClosestMatchingMode1"/>
	void FindClosestMatchingMode1( in  ModeDescription1 pModeToMatch, 
								   out ModeDescription1 pClosestMatch,
								   Direct3D12.IDevice   pConcernedDevice ) ;
	
	/// <inheritdoc cref="IDXGIOutput1.GetDisplaySurfaceData1"/>
	void GetDisplaySurfaceData1( IResource pDestination ) ;
	
	/// <inheritdoc cref="IDXGIOutput1.DuplicateOutput"/>
	void DuplicateOutput( IDevice pDevice, out IOutputDuplication? ppOutputDuplication ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGIOutput1 ) ;
	public new static Guid IID => (ComType.GUID) ;
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
public interface IOutput2: IOutput1 {
	// ---------------------------------------------------------------------------------
	
	/// <summary>Queries an adapter output for multiplane overlay support.</summary>
	/// <returns>TRUE if the output adapter is the primary adapter and it supports multiplane overlays, otherwise returns FALSE.</returns>
	/// <remarks>See <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcorewindow">CreateSwapChainForCoreWindow</a> for info on creating a foreground swap chain.</remarks>
	bool SupportsOverlays( ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGIOutput2 ) ;
	public new static Guid IID => (ComType.GUID) ;
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
	static IInstantiable IInstantiable.Instantiate( ) => new Output2( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Output2( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Output2( (pComObj as IDXGIOutput2)! ) ;
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
[SupportedOSPlatform("windows8.1")]
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
	new static Type ComType => typeof( IDXGIOutput3 ) ;
	public new static Guid IID => (ComType.GUID) ;
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
	static IInstantiable IInstantiable.Instantiate( ) => new Output3( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Output3( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Output3( (pComObj as IDXGIOutput3)! ) ;
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
[SupportedOSPlatform("windows10.0.10240")]
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
	new static Type ComType => typeof( IDXGIOutput4 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput4 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	static IInstantiable IInstantiable.Instantiate( ) => new Output4( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Output4( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Output4( (pComObj as IDXGIOutput4)! ) ;
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
[SupportedOSPlatform("windows10.0.10240")]
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
	new static Type ComType => typeof( IDXGIOutput5 ) ;
	public new static Guid IID => (ComType.GUID) ;
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
	static IInstantiable IInstantiable.Instantiate( ) => new Output5( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Output5( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Output5( (pComObj as IDXGIOutput5)! ) ;
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
[SupportedOSPlatform("windows10.0.19041")]
[ProxyFor(typeof(IDXGIOutput6))]
public interface IOutput6: IOutput5 {
	
	/// <summary>Get an extended description of the output that includes color characteristics and connection type.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1">DXGI_OUTPUT_DESC1</a>*</b> A pointer to the output description (see <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1">DXGI_OUTPUT_DESC1</a>).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgioutput6-getdesc1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns a code that indicates success or failure. S_OK if successful, <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>pDesc</i> is passed in as <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para>Some scenarios do not have well-defined values for all fields in this struct. For example, if this IDXGIOutput represents a clone/duplicate set, or if the EDID has missing or invalid data. In these cases, the OS will provide some default values that correspond to a standard SDR display. An output's reported color and luminance characteristics can adjust dynamically while the system is running due to user action or changing ambient conditions. Therefore, apps should periodically query **IDXGIFactory::IsCurrent** and re-create their **IDXGIFactory** if it returns **FALSE**. Then re-query **GetDesc1** from the new factory's equivalent output to retrieve the newest color information. For more details on how to write apps that react dynamically to monitor capabilities, see [Using DirectX with high dynamic range displays and Advanced Color](/windows/win32/direct3darticles/high-dynamic-range). On a high DPI desktop, <b>GetDesc1</b> returns the visualized screen size unless the app is marked high DPI aware. For info about writing DPI-aware Win32 apps, see <a href="https://docs.microsoft.com/windows/desktop/hidpi/high-dpi-desktop-application-development-on-windows">High DPI</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgioutput6-getdesc1#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetDesc1( out OutputDescription1 pDesc ) ;
	
	/// <summary>Notifies applications that hardware stretching is supported.</summary>
	/// <param name="pFlags">
	/// <para>Type: <b>UINT*</b> A bitfield of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_6/ne-dxgi1_6-dxgi_hardware_composition_support_flags">DXGI_HARDWARE_COMPOSITION_SUPPORT_FLAGS</a> enumeration values describing which types of hardware composition are supported. The values are bitwise OR'd together.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgioutput6-checkhardwarecompositionsupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns a code that indicates success or failure.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgioutput6-checkhardwarecompositionsupport">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CheckHardwareCompositionSupport( out HardwareCompositionSupportFlags pFlags ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGIOutput6 ) ;
	public new static Guid IID => (ComType.GUID) ;
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
	static IInstantiable IInstantiable.Instantiate( ) => new Output6( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Output6( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Output6( (pComObj as IDXGIOutput6)! ) ;
	// ==================================================================================
} ;

