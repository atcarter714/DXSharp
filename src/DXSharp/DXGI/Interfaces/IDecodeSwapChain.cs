#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;

using HResult = DXSharp.Windows.HResult ;
#endregion
namespace DXSharp.DXGI ;


[SupportedOSPlatform( "windows8.1" )]
[ProxyFor( typeof( IDXGIDecodeSwapChain ) )]
public interface IDecodeSwapChain: IComIID, IInstantiable {
	/// <summary>Presents a frame on the output adapter.</summary>
	/// <param name="BufferToPresent">An index indicating which member of the subresource array to present.</param>
	/// <param name="SyncInterval">
	/// <para>An integer that specifies how to synchronize presentation of a frame with the vertical blank.</para>
	/// <para>For the bit-block transfer (bitblt) model (<a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT_DISCARD</a> or <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT_SEQUENTIAL</a>), values are: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-presentbuffer#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Flags">
	/// <para>An integer value that contains swap-chain presentation options. These options are defined by the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-present">DXGI_PRESENT</a> constants. The <b>DXGI_PRESENT_USE_DURATION</b> flag must be set if a custom present duration (custom refresh rate) is being used.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-presentbuffer#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>This method returns <b>S_OK</b> on success, or it returns one of the following error codes: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-presentbuffer">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	HResult PresentBuffer( uint BufferToPresent, uint SyncInterval, PresentFlags Flags ) ;
	
	/// <summary>Sets the rectangle that defines the source region for the video processing blit operation.</summary>
	/// <param name="pRect">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">RECT</a> structure that contains the source region to set for the swap chain.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-setsourcerect#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>This method returns S_OK on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-setsourcerect">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetSourceRect( in Rect pRect ) ;

	/// <summary>Sets the rectangle that defines the target region for the video processing blit operation.</summary>
	/// <param name="pRect">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">RECT</a> structure that contains the target region to set for the swap chain.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-settargetrect#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>This method returns S_OK on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-settargetrect">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetTargetRect( in Rect pRect ) ;

	/// <summary>Sets the size of the destination surface to use for the video processing blit operation.</summary>
	/// <param name="Width">The width of the destination size, in pixels.</param>
	/// <param name="Height">The height of the destination size, in pixels.</param>
	/// <returns>This method returns S_OK on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-setdestsize">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetDestSize( uint Width, uint Height ) ;

	/// <summary>Gets the source region that is used for the swap chain.</summary>
	/// <param name="pRect">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">RECT</a> structure that receives the source region for the swap chain.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-getsourcerect#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>This method returns S_OK on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-getsourcerect">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetSourceRect( out Rect pRect ) ;

	/// <summary>Gets the rectangle that defines the target region for the video processing blit operation.</summary>
	/// <param name="pRect">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">RECT</a> structure that receives the target region for the swap chain.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-gettargetrect#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>This method returns S_OK on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-gettargetrect">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetTargetRect( out Rect pRect ) ;

	/// <summary>Gets the size of the destination surface to use for the video processing blit operation.</summary>
	/// <param name="pWidth">A pointer to a variable that receives the width in pixels.</param>
	/// <param name="pHeight">A pointer to a variable that receives the height in pixels.</param>
	/// <returns>This method returns S_OK on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-getdestsize">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetDestSize( out uint pWidth, out uint pHeight ) ;

	/// <summary>Sets the color space used by the swap chain. (IDXGIDecodeSwapChain.SetColorSpace)</summary>
	/// <param name="ColorSpace">A pointer to a combination of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_3/ne-dxgi1_3-dxgi_multiplane_overlay_ycbcr_flags">DXGI_MULTIPLANE_OVERLAY_YCbCr_FLAGS</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies the color space to set for the swap chain.</param>
	/// <returns>This method returns S_OK on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-setcolorspace">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetColorSpace( MultiplaneOverlayYCbCrFlags ColorSpace ) ;

	/// <summary>Gets the color space used by the swap chain.</summary>
	/// <returns>A combination of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_3/ne-dxgi1_3-dxgi_multiplane_overlay_ycbcr_flags">DXGI_MULTIPLANE_OVERLAY_YCbCr_FLAGS</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies the color space for the swap chain.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidecodeswapchain-getcolorspace">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	MultiplaneOverlayYCbCrFlags GetColorSpace( ) ;

	// ---------------------------------------------------------------------------
	static Type ComType => typeof( IDXGIDecodeSwapChain ) ;
	static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIDecodeSwapChain ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new DecodeSwapChain( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new DecodeSwapChain( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new DecodeSwapChain( (pComObj as IDXGIDecodeSwapChain)! ) ;
	// ===========================================================================
} ;