#region Using Directives
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


[Wrapper( typeof( IDXGIFactoryMedia ) )]
public interface IFactoryMedia {

	/// <summary>Creates a YUV swap chain for an existing DirectComposition surface handle. (IDXGIFactoryMedia.CreateSwapChainForCompositionSurfaceHandle)</summary>
	/// <param name="pDevice">A pointer to the Direct3D device for the swap chain. This parameter cannot be <b>NULL</b>. Software drivers, like <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/ne-d3dcommon-d3d_driver_type">D3D_DRIVER_TYPE_REFERENCE</a>, are not supported for composition swap chains.</param>
	/// <param name="hSurface">A handle to an existing <a href="https://docs.microsoft.com/windows/desktop/directcomp/reference">DirectComposition</a> surface. This parameter cannot be <b>NULL</b>.</param>
	/// <param name="pDesc">A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">DXGI_SWAP_CHAIN_DESC1</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>.</param>
	/// <param name="pRestrictToOutput">
	/// <para>A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgioutput">IDXGIOutput</a> interface for the swap chain to restrict content to. If the swap chain is moved to a different output, the content is black. You can optionally set this parameter to an output target that uses <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-present">DXGI_PRESENT_RESTRICT_TO_OUTPUT</a> to restrict the content on this output. If the swap chain is moved to a different output, the content is black. You must also pass the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-present">DXGI_PRESENT_RESTRICT_TO_OUTPUT</a> flag in a present call to force the content to appear blacked out on any other output. If you want to restrict the content to a different output, you must create a new swap chain. However, you can conditionally restrict content based on the <b>DXGI_PRESENT_RESTRICT_TO_OUTPUT</b> flag. Set this parameter to <b>NULL</b> if you don't want to restrict content to an output target.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgifactorymedia-createswapchainforcompositionsurfacehandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppSwapChain">A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgiswapchain1">IDXGISwapChain1</a> interface for the swap chain that this method creates.</param>
	/// <returns>
	/// <para><b>CreateSwapChainForCompositionSurfaceHandle</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgifactorymedia-createswapchainforcompositionsurfacehandle">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void CreateSwapChainForCompositionSurfaceHandle( [MarshalAs( UnmanagedType.IUnknown )] object pDevice,
															Win32Handle hSurface,
															SwapChainDescription1* pDesc,
															IOutput pRestrictToOutput,
															out ISwapChain1 ppSwapChain ) ;

	/// <summary>Creates a YUV swap chain for an existing DirectComposition surface handle. (IDXGIFactoryMedia.CreateDecodeSwapChainForCompositionSurfaceHandle)</summary>
	/// <param name="pDevice">
	/// <para>A pointer to the Direct3D device for the swap chain. This parameter cannot be <b>NULL</b>. Software drivers, like <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/ne-d3dcommon-d3d_driver_type">D3D_DRIVER_TYPE_REFERENCE</a>, are not supported for composition swap chains.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgifactorymedia-createdecodeswapchainforcompositionsurfacehandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="hSurface">A handle to an existing <a href="https://docs.microsoft.com/windows/desktop/directcomp/reference">DirectComposition</a> surface. This parameter cannot be <b>NULL</b>.</param>
	/// <param name="pDesc">
	/// <para>A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_3/ns-dxgi1_3-dxgi_decode_swap_chain_desc">DXGI_DECODE_SWAP_CHAIN_DESC</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgifactorymedia-createdecodeswapchainforcompositionsurfacehandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pYuvDecodeBuffers">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiresource">IDXGIResource</a> interface that represents the resource that contains the info that <b>CreateDecodeSwapChainForCompositionSurfaceHandle</b> decodes.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgifactorymedia-createdecodeswapchainforcompositionsurfacehandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pRestrictToOutput">
	/// <para>A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgioutput">IDXGIOutput</a> interface for the swap chain to restrict content to. If the swap chain is moved to a different output, the content is black. You can optionally set this parameter to an output target that uses <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-present">DXGI_PRESENT_RESTRICT_TO_OUTPUT</a> to restrict the content on this output. If the swap chain is moved to a different output, the content is black.</para>
	/// <para>You must also pass the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-present">DXGI_PRESENT_RESTRICT_TO_OUTPUT</a> flag in a present call to force the content to appear blacked out on any other output. If you want to restrict the content to a different output, you must create a new swap chain. However, you can conditionally restrict content based on the <b>DXGI_PRESENT_RESTRICT_TO_OUTPUT</b> flag.</para>
	/// <para>Set this parameter to <b>NULL</b> if you don't want to restrict content to an output target.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgifactorymedia-createdecodeswapchainforcompositionsurfacehandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppSwapChain">
	/// <para>A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_3/nn-dxgi1_3-idxgidecodeswapchain">IDXGIDecodeSwapChain</a> interface for the swap chain that this method creates.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgifactorymedia-createdecodeswapchainforcompositionsurfacehandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para><b>CreateDecodeSwapChainForCompositionSurfaceHandle</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>The <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiresource">IDXGIResource</a> provided via the <i>pYuvDecodeBuffers</i> parameter must point to at least one subresource, and all subresources must be created with the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ne-d3d11-d3d11_bind_flag">D3D11_BIND_DECODER</a> flag.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgifactorymedia-createdecodeswapchainforcompositionsurfacehandle#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void CreateDecodeSwapChainForCompositionSurfaceHandle( object                      pDevice,
																  Win32Handle                 hSurface,
																  DecodeSwapChainDescription* pDesc,
																  IResource                   pYuvDecodeBuffers,
																  IOutput                     pRestrictToOutput,
																  IDXGIDecodeSwapChain        ppSwapChain ) ;
} ;



[Wrapper( typeof( IDXGIDecodeSwapChain ) )]
public interface IDecodeSwapChain
{
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
	HResult PresentBuffer( uint BufferToPresent, uint SyncInterval, uint Flags ) ;

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
}
