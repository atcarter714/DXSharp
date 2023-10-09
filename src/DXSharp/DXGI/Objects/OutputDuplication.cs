using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;

namespace DXSharp.DXGI ;

public class OutputDuplication: Object, IOutputDuplication {
	public IDXGIOutputDuplication? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIOutputDuplication >? ComPointer { get ; protected set ; }
	
	public OutputDuplication( nint pointer ): base( pointer ) { }
	public OutputDuplication( IDXGIOutputDuplication dxgiObj ): base( dxgiObj ) { }
	public OutputDuplication( ComPtr< IDXGIOutputDuplication >? comPtr ): base( comPtr!.InterfaceVPtr ) { }
	
	IDXGIOutputDuplication _dxgiInterface => COMObject ??
		throw ( ComPointer is not null && ComPointer.Disposed
				 ? new ObjectDisposedException( nameof(OutputDuplication) )
				 : new NullReferenceException( $"{nameof(OutputDuplication)} :: " +
											   $"internal {nameof(IDXGIOutputDuplication)} null reference." ) ) ;
	
	
	
	public void GetDesc( out OutputDuplicationDescription pDesc ) {
		_throwIfDestroyed( ) ;
		unsafe {
			pDesc = default ;
			DXGI_OUTDUPL_DESC desc = default ;
			_dxgiInterface.GetDesc( &desc ) ;
			pDesc = new OutputDuplicationDescription( desc ) ;
		}
	}

	public void AcquireNextFrame( uint TimeoutInMilliseconds, 
								  out OutputDuplicationFrameInfo pFrameInfo,
								  out IResource ppDesktopResource ) {
		_throwIfDestroyed( ) ;
		unsafe {
			pFrameInfo = default ;
			ppDesktopResource = default ;
			IDXGIResource* pResource = null ;
			DXGI_OUTDUPL_FRAME_INFO frameInfo = default ;
			_dxgiInterface.AcquireNextFrame( TimeoutInMilliseconds, &frameInfo, &pResource ) ;
			pFrameInfo = new OutputDuplicationFrameInfo( frameInfo ) ;
			ppDesktopResource = new Resource( new ComPtr< IDXGIResource >( pResource ) ) ;
		}
	}

	public void GetFrameDirtyRects( uint     DirtyRectsBufferSize, out Span< Rect > pDirtyRectsBuffer,
									out uint pDirtyRectsBufferSizeRequired ) {
		_throwIfDestroyed( ) ;
	}

	public void GetFrameMoveRects( uint MoveRectsBufferSize, out Span< OutputDuplicationMoveRect > pMoveRectBuffer, out uint pMoveRectsBufferSizeRequired ) {
		_throwIfDestroyed( ) ;
	}

	public void GetFramePointerShape( uint     PointerShapeBufferSize,          IntPtr                                        pPointerShapeBuffer,
									  out uint pPointerShapeBufferSizeRequired, out Span< OutputDuplicationPointerShapeInfo > pPointerShapeInfo ) {
		_throwIfDestroyed( ) ;
	}

	public void MapDesktopSurface( out MappedRect pLockedRect ) {
		_throwIfDestroyed( ) ;
	}

	public void UnMapDesktopSurface() {
		_throwIfDestroyed( ) ;
	}

	public void ReleaseFrame() {
		_throwIfDestroyed( ) ;
	}
}