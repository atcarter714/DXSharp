#region Using Directives
using System.Runtime.CompilerServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


public class OutputDuplication: Object, IOutputDuplication {
	public static Type ComType => typeof( IDXGIOutputDuplication ) ;
	public static Guid InterfaceGUID => typeof( IDXGIOutputDuplication ).GUID ;
	
	
	public new IDXGIOutputDuplication? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIOutputDuplication >? ComPointer { get ; protected set ; }
	
	internal OutputDuplication( ) { }
	internal OutputDuplication( nint ptr ) => ComPointer = new( ptr ) ;
	internal OutputDuplication( IDXGIOutputDuplication dxgiObj ) => ComPointer = new( dxgiObj ) ;
	internal OutputDuplication( ComPtr< IDXGIOutputDuplication >? comPtr ) => ComPointer = comPtr ;

	IDXGIOutputDuplication _dxgiInterface => COMObject ??
											 throw ( ComPointer is not null && ComPointer.Disposed
														 ? new ObjectDisposedException( nameof(OutputDuplication) )
														 : new NullReferenceException( $"{nameof(OutputDuplication)} :: " +
																					   $"internal {nameof(IDXGIOutputDuplication)} null reference." ) ) ;
	
	
	
	public void GetDesc( out OutputDuplicationDescription pDesc ) {
		
		unsafe {
			pDesc = default ;
			DXGI_OUTDUPL_DESC desc = default ;
			_dxgiInterface.GetDesc( &desc ) ;
			pDesc = new OutputDuplicationDescription( desc ) ;
		}
	}

	public void AcquireNextFrame( uint TimeoutInMilliseconds, 
								  out OutputDuplicationFrameInfo pFrameInfo,
								  out IResource? ppDesktopResource ) {
		pFrameInfo = default ;
		ppDesktopResource = default ;
		unsafe {
			DXGI_OUTDUPL_FRAME_INFO frameInfo = default ;
			_dxgiInterface.AcquireNextFrame( TimeoutInMilliseconds, &frameInfo, out var resource ) ;
			pFrameInfo = frameInfo ;
			ppDesktopResource = new Resource( resource ) ;
		}
	}

	
	const int MAX_RECTS = 0x100 ;
	public void GetFrameDirtyRects( uint DirtyRectsBufferSize,
									out Span<Rect> pDirtyRectsBuffer,
									out uint pDirtyRectsBufferSizeRequired ) {
		if( DirtyRectsBufferSize > MAX_RECTS )
			throw new ArgumentOutOfRangeException( nameof(DirtyRectsBufferSize), DirtyRectsBufferSize,
												   $"{nameof(GetFrameDirtyRects)} :: " +
												   $"The maximum number of dirty rects is {MAX_RECTS}." ) ;
		
		
		pDirtyRectsBuffer = default ;
		pDirtyRectsBufferSizeRequired = default ;
		unsafe {
			RECT* src = null ;
			var pBuffer = stackalloc RECT[ (int)DirtyRectsBufferSize ] ;
			_dxgiInterface.GetFrameDirtyRects( DirtyRectsBufferSize, pBuffer, 
												out pDirtyRectsBufferSizeRequired ) ;

			src = pBuffer ;
			if ( pDirtyRectsBufferSizeRequired > DirtyRectsBufferSize ) {
				//! Resize the buffer and try again
				var retryBuffer = stackalloc RECT[ (int)pDirtyRectsBufferSizeRequired ] ;
				_dxgiInterface.GetFrameDirtyRects( pDirtyRectsBufferSizeRequired, retryBuffer, 
												   out uint sizeRequired2 ) ;
				pDirtyRectsBufferSizeRequired = sizeRequired2 ;
				src = retryBuffer ;
			}
			
			// Convert the RECT* to a Span< Rect > in managed heap memory for safe use:
			Span< Rect > dst = ( new Rect[ pDirtyRectsBufferSizeRequired ] ).AsSpan( ) ;
			fixed( Rect* dstSpanPtr = &dst[ 0x00 ] ) {
				RECT* srcPtr = src, dstPtr = (RECT *)dstSpanPtr ;
				for ( int i = 0; i < pDirtyRectsBufferSizeRequired; ++i ) {
					*( dstPtr++ ) = *( srcPtr++ ) ;
				}
			}
			pDirtyRectsBuffer = dst ;
		}
	}

	const int MAX_MOVE_RECTS = 0x100 ;
	public void GetFrameMoveRects( uint MoveRectsBufferSize, 
								   out Span< OutputDuplicationMoveRect > pMoveRectBuffer, 
								   out uint pMoveRectsBufferSizeRequired ) {
		if( MoveRectsBufferSize > MAX_MOVE_RECTS )
			throw new ArgumentOutOfRangeException( nameof(MoveRectsBufferSize), MoveRectsBufferSize,
												   $"{nameof(GetFrameMoveRects)} :: " +
												   $"The maximum number of move rects is {MAX_MOVE_RECTS}." ) ;
		
		 
		pMoveRectBuffer = default ;
		pMoveRectsBufferSizeRequired = default ;
		unsafe {
			DXGI_OUTDUPL_MOVE_RECT* src = null ;
			var pBuffer = stackalloc DXGI_OUTDUPL_MOVE_RECT[ (int)MoveRectsBufferSize ] ;
			_dxgiInterface.GetFrameMoveRects( MoveRectsBufferSize, pBuffer, 
											  out pMoveRectsBufferSizeRequired ) ;

			src = pBuffer ;
			if ( pMoveRectsBufferSizeRequired > MoveRectsBufferSize ) {
				//! Resize the buffer and try again
				var retryBuffer = stackalloc DXGI_OUTDUPL_MOVE_RECT[ (int)pMoveRectsBufferSizeRequired ] ;
				_dxgiInterface.GetFrameMoveRects( pMoveRectsBufferSizeRequired, retryBuffer, 
												 out uint sizeRequired2 ) ;
				pMoveRectsBufferSizeRequired = sizeRequired2 ;
				src = retryBuffer ;
			}
			
			// Convert the RECT* to a Span< Rect > in managed heap memory for safe use:
			Span< OutputDuplicationMoveRect > dst = ( new OutputDuplicationMoveRect[ pMoveRectsBufferSizeRequired ] ).AsSpan( ) ;
			fixed( OutputDuplicationMoveRect* dstSpanPtr = &dst[ 0x00 ] ) {
				DXGI_OUTDUPL_MOVE_RECT* srcPtr = src, dstPtr = (DXGI_OUTDUPL_MOVE_RECT *)dstSpanPtr ;
				for ( int i = 0; i < pMoveRectsBufferSizeRequired; ++i ) {
					*( dstPtr++ ) = *( srcPtr++ ) ;
				}
			}
			pMoveRectBuffer = dst ;
		}
	}

	const int MAX_POINTER_SHAPE_BUFFER = 0x100 ;
	public void GetFramePointerShape( uint PointerShapeBufferSize, 
									  nint pPointerShapeBuffer,
									  out uint pPointerShapeBufferSizeRequired,
									  out Span< OutputDuplicationPointerShapeInfo > pPointerShapeInfo ) {
		if( PointerShapeBufferSize > MAX_POINTER_SHAPE_BUFFER )
			throw new ArgumentOutOfRangeException( nameof(PointerShapeBufferSize), PointerShapeBufferSize,
												   $"{nameof(GetFramePointerShape)} :: " +
												   $"The maximum number of pointer shapes is {MAX_POINTER_SHAPE_BUFFER}." ) ;
		
		
		pPointerShapeInfo = default ;
		pPointerShapeBufferSizeRequired = default ;
		unsafe {
			DXGI_OUTDUPL_POINTER_SHAPE_INFO* src = null ;
			var pBuffer = stackalloc 
				DXGI_OUTDUPL_POINTER_SHAPE_INFO[ (int)PointerShapeBufferSize ] ;
			
			DXGI_OUTDUPL_POINTER_SHAPE_INFO shapeInfo = default ;
			
			_dxgiInterface.GetFramePointerShape( PointerShapeBufferSize, pBuffer,
												 out pPointerShapeBufferSizeRequired, &shapeInfo ) ;

			src = pBuffer ;
			if ( pPointerShapeBufferSizeRequired > PointerShapeBufferSize ) {
				//! Resize the buffer and try again
				var retryBuffer = stackalloc DXGI_OUTDUPL_POINTER_SHAPE_INFO[ (int)pPointerShapeBufferSizeRequired ] ;
				_dxgiInterface.GetFramePointerShape( pPointerShapeBufferSizeRequired, retryBuffer, 
													 out uint sizeRequired2, &shapeInfo ) ;
				pPointerShapeBufferSizeRequired = sizeRequired2 ;
				src = retryBuffer ;
			}
			
			// Convert the RECT* to a Span< Rect > in managed heap memory for safe use:
			Span< OutputDuplicationPointerShapeInfo > dst = ( new OutputDuplicationPointerShapeInfo[ pPointerShapeBufferSizeRequired ] ).AsSpan( ) ;
			fixed( OutputDuplicationPointerShapeInfo* dstSpanPtr = &dst[ 0x00 ] ) {
				DXGI_OUTDUPL_POINTER_SHAPE_INFO* srcPtr = src, dstPtr = (DXGI_OUTDUPL_POINTER_SHAPE_INFO *)dstSpanPtr ;
				for ( int i = 0; i < pPointerShapeBufferSizeRequired; ++i ) {
					*( dstPtr++ ) = *( srcPtr++ ) ;
				}
			}
			pPointerShapeInfo = dst ;
		}
	}

	public void MapDesktopSurface( out MappedRect pLockedRect ) {
		
		unsafe {
			DXGI_MAPPED_RECT lockedRect = default ;
			_dxgiInterface.MapDesktopSurface( &lockedRect ) ;
			pLockedRect = lockedRect ;
		}
	}

	public void UnMapDesktopSurface( ) {
		
		_dxgiInterface.UnMapDesktopSurface( ) ;
	}

	public void ReleaseFrame( ) {
		
		_dxgiInterface.ReleaseFrame( ) ;
	}

	public static IDXCOMObject Instantiate( ) => new OutputDuplication( ) ;
	public static IDXCOMObject Instantiate( IntPtr pComObj ) => new OutputDuplication( pComObj ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => 
		new OutputDuplication( ( pComObj as IDXGIOutputDuplication )! ) ;
} ;