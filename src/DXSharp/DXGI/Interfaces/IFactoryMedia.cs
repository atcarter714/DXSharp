#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.Win32 ;
using HResult = DXSharp.Windows.HResult ;
#endregion
namespace DXSharp.DXGI ;


[SupportedOSPlatform( "windows8.1" )]
[ProxyFor( typeof( IDXGIFactoryMedia ) )]
public interface IFactoryMedia: IComIID, IInstantiable {

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
	void CreateSwapChainForCompositionSurfaceHandle( IDXCOMObject pDevice,
													 Win32Handle hSurface,
													 in SwapChainDescription1 pDesc,
													 [Optional] IOutput? pRestrictToOutput,
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
	void CreateDecodeSwapChainForCompositionSurfaceHandle( IDXCOMObject pDevice,
														   Win32Handle hSurface,
														   in DecodeSwapChainDescription pDesc,
														   IResource pYuvDecodeBuffers,
														   [Optional] IOutput? pRestrictToOutput,
														   out IDecodeSwapChain?  ppSwapChain ) ;
	
	
	// ---------------------------------------------------------------------------
	static Type ComType => typeof( IDXGIFactoryMedia ) ;
	static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIFactoryMedia ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new FactoryMedia( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new FactoryMedia( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new FactoryMedia( (pComObj as IDXGIFactoryMedia)! ) ;
	// ===========================================================================
} ;


