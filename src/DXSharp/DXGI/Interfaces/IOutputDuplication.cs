#region Using Directives
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Accesses and manipulates a duplicated desktop image.</summary>
/// <remarks>
/// <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nn-dxgi1_2-idxgioutputduplication">IDXGIOutputDuplication</a>
/// </remarks>
public interface IOutputDuplication: IObject,
									 IComObjectRef< IDXGIOutputDuplication >,
									 IUnknownWrapper< IDXGIOutputDuplication >,
									 IInstantiable {
	new ComPtr<IDXGIOutputDuplication>? ComPointer { get ; }
	new IDXGIOutputDuplication? COMObject => ComPointer!.Interface ;


	/// <summary>
	/// Retrieves a description of a duplicated output.
	/// This description specifies the dimensions of
	/// the surface that contains the desktop image.
	/// </summary>
	/// <param name="pDesc">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_desc">DXGI_OUTDUPL_DESC</a> structure that describes the duplicated output. This parameter must not be <b>NULL</b>.</param>
	/// <remarks>
	/// After an application creates an
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgioutputduplication">IDXGIOutputDuplication</a>
	/// interface, it calls <b>GetDesc</b> to retrieve the dimensions of the surface that contains the desktop image.
	/// The format of the desktop image is always
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT_B8G8R8A8_UNORM</a>.
	/// </remarks>
	void GetDesc( out OutputDuplicationDescription pDesc ) ;

	/// <summary>Indicates that the application is ready to process the next desktop image.</summary>
	/// <param name="TimeoutInMilliseconds">
	/// <para>The time-out interval, in milliseconds. This interval specifies the amount of time that this method waits for a new frame before it returns to the caller.  This method returns if the interval elapses, and a new desktop image is not available. For more information about the time-out interval, see Remarks.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-acquirenextframe#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pFrameInfo">A pointer to a memory location that receives the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_frame_info">DXGI_OUTDUPL_FRAME_INFO</a> structure that describes timing and presentation statistics for a frame.</param>
	/// <param name="ppDesktopResource">A pointer to a variable that receives the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiresource">IDXGIResource</a> interface of the surface that contains the desktop bitmap.</param>
	/// <returns>
	/// <para><b>AcquireNextFrame</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>When <b>AcquireNextFrame</b> returns successfully, the calling application can access the desktop image that <b>AcquireNextFrame</b> returns in the variable at <i>ppDesktopResource</i>. If the caller specifies a zero time-out interval in the <i>TimeoutInMilliseconds</i> parameter, <b>AcquireNextFrame</b> verifies whether there is a new desktop image available, returns immediately, and indicates its outcome with the return value.  If the caller specifies an <b>INFINITE</b> time-out interval in the <i>TimeoutInMilliseconds</i> parameter, the time-out interval never elapses. <div class="alert"><b>Note</b>  You cannot cancel the wait that you specified in the <i>TimeoutInMilliseconds</i> parameter. Therefore, if you must periodically check for other conditions (for example, a terminate signal), you should specify a non-<b>INFINITE</b> time-out interval. After the time-out interval elapses, you can check for these other conditions and then call <b>AcquireNextFrame</b> again to wait for the next frame.</div> <div> </div> <b>AcquireNextFrame</b> acquires a new desktop frame when the operating system either updates the desktop bitmap image or changes the shape or position of a hardware pointer.  The new frame that <b>AcquireNextFrame</b> acquires might have only the desktop image updated, only the pointer shape or position updated, or both.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-acquirenextframe#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void AcquireNextFrame( uint TimeoutInMilliseconds,
						   out OutputDuplicationFrameInfo pFrameInfo,
						   out IResource? ppDesktopResource ) ;

	/// <summary>Gets information about dirty rectangles for the current desktop frame.</summary>
	/// <param name="DirtyRectsBufferSize">
	/// <para>The size in bytes of the buffer that the caller passed to the  <i>pDirtyRectsBuffer</i> parameter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframedirtyrects#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDirtyRectsBuffer">
	/// <para>A pointer to an array of <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">RECT</a> structures that identifies the dirty rectangle regions for the desktop frame.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframedirtyrects#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDirtyRectsBufferSizeRequired">
	/// <para>Pointer to a variable that receives the number of bytes that <b>GetFrameDirtyRects</b> needs to store information about dirty regions in the buffer at <i>pDirtyRectsBuffer</i>. For more information about returning the required buffer size, see Remarks.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframedirtyrects#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para><b>GetFrameDirtyRects</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>GetFrameDirtyRects</b> stores a size value in the variable at <i>pDirtyRectsBufferSizeRequired</i>. This  value specifies the number of bytes that <b>GetFrameDirtyRects</b> needs to store information about dirty regions. You can use this value in the following situations to determine the amount of memory to allocate for future buffers that you pass to <i>pDirtyRectsBuffer</i>: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframedirtyrects#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetFrameDirtyRects( uint DirtyRectsBufferSize,
							 out Span< Rect > pDirtyRectsBuffer,
							 out uint pDirtyRectsBufferSizeRequired ) ;

	/// <summary>Gets information about the moved rectangles for the current desktop frame.</summary>
	/// <param name="MoveRectsBufferSize">The size in bytes of the buffer that the caller passed to the  <i>pMoveRectBuffer</i> parameter.</param>
	/// <param name="pMoveRectBuffer">
	/// <para>A pointer to an array of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_move_rect">DXGI_OUTDUPL_MOVE_RECT</a> structures that identifies the moved rectangle regions for the desktop frame.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframemoverects#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pMoveRectsBufferSizeRequired">
	/// <para>Pointer to a variable that receives the number of bytes that <b>GetFrameMoveRects</b> needs to store information about moved regions in the buffer at <i>pMoveRectBuffer</i>. For more information about returning the required buffer size, see Remarks.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframemoverects#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para><b>GetFrameMoveRects</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>GetFrameMoveRects</b> stores a size value in the variable at <i>pMoveRectsBufferSizeRequired</i>. This  value specifies the number of bytes that <b>GetFrameMoveRects</b> needs to store information about moved regions. You can use this value in the following situations to determine the amount of memory to allocate for future buffers that you pass to <i>pMoveRectBuffer</i>: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframemoverects#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetFrameMoveRects( uint MoveRectsBufferSize,
							out Span< OutputDuplicationMoveRect > pMoveRectBuffer,
							out uint pMoveRectsBufferSizeRequired ) ;

	/// <summary>Gets information about the new pointer shape for the current desktop frame.</summary>
	/// <param name="PointerShapeBufferSize">The size in bytes of the buffer that the caller passed to the  <i>pPointerShapeBuffer</i> parameter.</param>
	/// <param name="pPointerShapeBuffer">A pointer to a buffer to which <b>GetFramePointerShape</b> copies and returns pixel data for the new pointer shape.</param>
	/// <param name="pPointerShapeBufferSizeRequired">
	/// <para>Pointer to a variable that receives the number of bytes that <b>GetFramePointerShape</b> needs to store the new pointer shape pixel data in the buffer at <i>pPointerShapeBuffer</i>. For more information about returning the required buffer size, see Remarks.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframepointershape#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pPointerShapeInfo">Pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_pointer_shape_info">DXGI_OUTDUPL_POINTER_SHAPE_INFO</a> structure that receives the pointer shape information.</param>
	/// <returns>
	/// <para><b>GetFramePointerShape</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>GetFramePointerShape</b> stores a size value in the variable at <i>pPointerShapeBufferSizeRequired</i>. This  value specifies the number of bytes that <i>pPointerShapeBufferSizeRequired</i> needs to store the new pointer shape pixel data. You can use the value in the following situations to determine the amount of memory to allocate for future buffers that you pass to <i>pPointerShapeBuffer</i>: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getframepointershape#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetFramePointerShape( uint PointerShapeBufferSize,
							   nint pPointerShapeBuffer,
							   out uint pPointerShapeBufferSizeRequired,
							   out Span< OutputDuplicationPointerShapeInfo > pPointerShapeInfo ) ;

	/// <summary>
	/// Provides the CPU with efficient access to a desktop image if that desktop image is already in system memory.
	/// </summary>
	/// <param name="pLockedRect">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_mapped_rect">DXGI_MAPPED_RECT</a> structure that receives the surface data that the CPU needs to directly access the surface data.</param>
	/// <remarks>You can successfully call <b>MapDesktopSurface</b> if the <b>DesktopImageInSystemMemory</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_desc">DXGI_OUTDUPL_DESC</a> structure is set to <b>TRUE</b>. If <b>DesktopImageInSystemMemory</b> is <b>FALSE</b>, <b>MapDesktopSurface</b> returns DXGI_ERROR_UNSUPPORTED. Call <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-getdesc">IDXGIOutputDuplication::GetDesc</a> to retrieve the <b>DXGI_OUTDUPL_DESC</b> structure.</remarks>
	void MapDesktopSurface( out MappedRect pLockedRect ) ;

	/// <summary>Invalidates the pointer to the desktop image that was retrieved by using IDXGIOutputDuplication::MapDesktopSurface.</summary>
	/// <remarks>
	/// <para>
	/// <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-unmapdesktopsurface">
	/// Learn more about this API from docs.microsoft.com
	/// </a>.
	/// </para>
	/// </remarks>
	void UnMapDesktopSurface( ) ;

	/// <summary>Indicates that the application finished processing the frame.</summary>
	/// <remarks>
	/// <para>The application must release the frame before it acquires the next frame.
	/// After the frame is released, the surface that contains the desktop bitmap becomes
	/// invalid; you will not be able to use the surface in a DirectX graphics operation.
	/// For performance reasons, we recommend that you release the frame just before you
	/// call the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-acquirenextframe">
	/// IDXGIOutputDuplication::AcquireNextFrame
	/// </a>
	/// method to acquire the next frame. When the client does not own the frame, the
	/// operating system copies all desktop updates to the surface. This can result in
	/// wasted GPU cycles if the operating system updates the same region for each frame
	/// that occurs.  When the client acquires the frame, the client is aware of only the
	/// final update to this region; therefore, any overlapping updates during previous
	/// frames are wasted. When the client acquires a frame, the client owns the surface;
	/// therefore, the operating system can track only the updated regions and cannot copy
	/// desktop updates to the surface. Because of this behavior, we recommend that you
	/// minimize the time between the call to release the current frame and the call to
	/// acquire the next frame.
	/// </para>
	/// <para>
	/// <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-releaseframe#">
	/// Read more on docs.microsoft.com
	/// </a>.
	/// </para>
	/// </remarks>
	void ReleaseFrame( ) ;
} ;