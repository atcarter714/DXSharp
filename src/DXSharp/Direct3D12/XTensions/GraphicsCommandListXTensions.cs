using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12.XTensions ;


public static class GraphicsCommandListXTensions {
	
	static Rect[ ]   _scissorRectCache = new Rect[ 1 ] ;
	static float[ ] _clearColorCache  = new float[ 4 ] ;
	
	static unsafe void _setTempClearColor( ColorF color ) {
		fixed( float* pColor = _clearColorCache ) {
			pColor[ 0 ] = color.R ;
			pColor[ 1 ] = color.G ;
			pColor[ 2 ] = color.B ;
			pColor[ 3 ] = color.A ;
		}
	}
	
	
	public static void ClearRenderTargetView( this IGraphicsCommandList graphicsCommandList,
											  CPUDescriptorHandle        RenderTargetView, 
											  ColorF                     ColorRGBA,
											  Span< Rect >               pRects = default ) {
		_setTempClearColor( ColorRGBA ) ;
		graphicsCommandList.ClearRenderTargetView( RenderTargetView, _clearColorCache,
												   (uint)pRects.Length, pRects ) ;
	}

	public static void RSSetScissorRects( this IGraphicsCommandList graphicsCommandList, in Rect rect ) {
		var commandList = (IComObjectRef< ID3D12GraphicsCommandList >)graphicsCommandList ;
		ArgumentNullException.ThrowIfNull( graphicsCommandList, nameof(graphicsCommandList) ) ;
		ObjectDisposedException.ThrowIf( commandList.ComObject is null, typeof(ID3D12GraphicsCommandList) ) ;
		
		_scissorRectCache[ 0 ] = rect ;
		unsafe {
			fixed( Rect* pScissorRects = &_scissorRectCache[ 0 ] )
				commandList.ComObject.RSSetScissorRects( 1, (RECT *)pScissorRects ) ;
		}
	}
} ;