#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


[Wrapper(typeof(IDXGIOutputDuplication))]
internal class OutputDuplication: Object, IOutputDuplication {
	// ----------------------------------------------------------------------------------------------
	ComPtr< IDXGIOutputDuplication >? _comPtr ;
	public new virtual ComPtr< IDXGIOutputDuplication >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<IDXGIOutputDuplication>(  ) ;
	public override IDXGIOutputDuplication? ComObject => ComPointer?.Interface ;
	
	// ----------------------------------------------------------------------------------------------
	public OutputDuplication( ) {
		_comPtr = ComResources?.GetPointer<IDXGIOutputDuplication>(  ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	public OutputDuplication( nint pComObj ) {
		_comPtr = new( pComObj ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	public OutputDuplication( IDXGIOutputDuplication pComObj ) {
		_comPtr = new( pComObj ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	public OutputDuplication( ComPtr< IDXGIOutputDuplication > ptr ) {
		_comPtr = ptr ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}

	// ----------------------------------------------------------------------------------------------
	
	IDXGIOutputDuplication _dxgiInterface => ComObject ??
											 throw ( ComPointer is not null && ComPointer.Disposed
														 ? new ObjectDisposedException( nameof(OutputDuplication) )
														 : new NullReferenceException( $"{nameof(OutputDuplication)} :: " +
																					   $"internal {nameof(IDXGIOutputDuplication)} null reference." ) ) ;
	
	// ----------------------------------------------------------------------------------------------
	
	public void GetDesc( out OutputDuplicationDescription pDesc ) {
		
		unsafe {
			pDesc = default ;
			DXGI_OUTDUPL_DESC desc = default ;
			_dxgiInterface.GetDesc( &desc ) ;
			pDesc = new OutputDuplicationDescription( desc ) ;
		}
	}

	public void AcquireNextFrame( uint timeoutInMilliseconds, 
								  out OutputDuplicationFrameInfo pFrameInfo,
								  out IResource? ppDesktopResource ) {
		pFrameInfo = default ;
		ppDesktopResource = default ;
		unsafe {
			DXGI_OUTDUPL_FRAME_INFO frameInfo = default ;
			_dxgiInterface.AcquireNextFrame( timeoutInMilliseconds, &frameInfo, out var resource ) ;
			pFrameInfo = frameInfo ;
			ppDesktopResource = new Resource( resource ) ;
		}
	}

	
	const int MAX_RECTS = 0x400 ;
	public HResult GetFrameDirtyRects( uint dirtyRectsBufferSize,
									   out Span< Rect > pDirtyRectsBuffer,
									   out uint pDirtyRectsBufferSizeRequired ) {
		if( dirtyRectsBufferSize > MAX_RECTS )
			throw new ArgumentOutOfRangeException( nameof(dirtyRectsBufferSize), dirtyRectsBufferSize,
												   $"{nameof(GetFrameDirtyRects)} :: " +
												   $"The maximum number of dirty rects is {MAX_RECTS}." ) ;
		
		pDirtyRectsBuffer = new Rect[ dirtyRectsBufferSize ] ;
		
		unsafe {
			HResult hr = default ;
			fixed( Rect* rects = &pDirtyRectsBuffer[ 0x00 ] ) {
				hr = _dxgiInterface.GetFrameDirtyRects( dirtyRectsBufferSize, (RECT *)rects,
															out pDirtyRectsBufferSizeRequired ) ;
			}
			return hr ;
		}
	}

	const int MAX_MOVE_RECTS = 0x400 ;
	public HResult GetFrameMoveRects( uint moveRectsBufferSize, 
									  out Span< OutputDuplicationMoveRect > pMoveRectBuffer, 
									  out uint pMoveRectsBufferSizeRequired ) {
		if( moveRectsBufferSize > MAX_MOVE_RECTS )
			throw new ArgumentOutOfRangeException( nameof(moveRectsBufferSize), moveRectsBufferSize,
												   $"{nameof(GetFrameMoveRects)} :: " +
												   $"The maximum number of move rects is {MAX_MOVE_RECTS}." ) ;
		
		pMoveRectBuffer = new OutputDuplicationMoveRect[ moveRectsBufferSize ] ;
		HResult hr = default ;
		
		unsafe {
			fixed ( OutputDuplicationMoveRect* src = &pMoveRectBuffer[ 0x00 ] ) {
				hr = _dxgiInterface.GetFrameMoveRects( moveRectsBufferSize, 
												  (DXGI_OUTDUPL_MOVE_RECT *)src,
												  out pMoveRectsBufferSizeRequired ) ;
				return hr ;
			}
		}
	}

	const int MAX_POINTER_SHAPE_BUFFER = 0x400 ;
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
	
	// ----------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIOutputDuplication ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutputDuplication ).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	public static IInstantiable  Instantiate( ) => new OutputDuplication( ) ;
	public static IInstantiable Instantiate( nint pComObj ) => new OutputDuplication( pComObj ) ;
	public static IInstantiable Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => 
		new OutputDuplication( ( pComObj as IDXGIOutputDuplication )! ) ;
	// ==============================================================================================
} ;