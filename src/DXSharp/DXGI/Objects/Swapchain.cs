#region Using Directives
using System.Runtime.Versioning ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using WinRT ;
using Windows.UI.Core ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


//public enum ColorSpaceSupportFlags: uint { Present = 0x1, OverlayPresent = 0x2, } ;

[Wrapper(typeof(IDXGISwapChain))]
internal class SwapChain: DeviceSubObject,
						  ISwapChain,
						  IComObjectRef< IDXGISwapChain >,
						  IUnknownWrapper< IDXGISwapChain > {
	ComPtr< IDXGISwapChain >? _comPtr ;
	public new virtual ComPtr< IDXGISwapChain >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGISwapChain >(  ) ;
	public override IDXGISwapChain? ComObject => ComPointer?.Interface ;
	
	// -----------------------------------------------------------------
	// Constructors:
	// -----------------------------------------------------------------
	
	internal SwapChain( ) {
		_comPtr = ComResources?.GetPointer< IDXGISwapChain >( ) ;
		if( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	internal SwapChain( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( SwapChain )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain( in IDXGISwapChain dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( SwapChain )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain( ComPtr< IDXGISwapChain > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -----------------------------------------------------------------

	public void Present( uint syncInterval, PresentFlags flags ) => 
						ComObject!.Present( syncInterval, (uint)flags ) ;
	
	public void GetBuffer< TResource >( uint buffer, out TResource pSurface ) where TResource: IDXCOMObject, IInstantiable {
		unsafe {
			Guid guid = TResource.Guid ;
			ComObject!.GetBuffer( buffer, &guid, out var comObj ) ;
			
#if DEBUG || DEBUG_COM || DEV_BUILD
			if( comObj is null ) throw new DirectXComError( $"{nameof(SwapChain)}.{nameof(GetBuffer)} :: " +
															$"Failed to obtain buffer resource of type \"{typeof(TResource).Name}\" " +
															$"(GUID: {typeof(TResource).GUID})" ) ;
#endif
			var surface = (TResource)TResource.Instantiate( (IUnknown)comObj ) ;
			pSurface = surface ;
		}
	}

	public void SetFullscreenState( bool fullscreen, in IOutput? pTarget ) {
		var output = (IComObjectRef< IDXGIOutput >?)pTarget 
					 ?? throw new NullReferenceException( nameof(pTarget) ) ;
		ComObject!.SetFullscreenState( fullscreen, output.ComObject ) ;
	}
	
	public void GetFullscreenState( out bool pFullscreen, out IOutput? ppTarget ) {
		ppTarget = null ;
		unsafe {
			BOOL fullscreen = false ;
			ComObject!.GetFullscreenState( &fullscreen, out var output ) ;
			ppTarget = (IOutput)new Output( output ) ;
			pFullscreen = fullscreen ;
		}
	}
	
	public void GetDesc( out SwapChainDescription pDesc ) {
		unsafe {
			DXGI_SWAP_CHAIN_DESC result = default ;
			ComObject!.GetDesc( &result ) ;
			pDesc = result ;
		}
	}
	
	public void ResizeBuffers( uint bufferCount, uint width, uint height,
							   Format newFormat, SwapChainFlags swapChainFlags ) {
		ComObject!.ResizeBuffers( bufferCount, width, height,
									  (DXGI_FORMAT)newFormat,
										(uint)swapChainFlags ) ;
	}
	
	public void ResizeTarget( in ModeDescription newTargetParameters ) {
		unsafe {
			DXGI_MODE_DESC result = newTargetParameters ;
			ComObject!.ResizeTarget( &result ) ;
		}
	}
	
	public TOutput? GetContainingOutput< TOutput >( ) where TOutput: class, IOutput {
		ComObject!.GetContainingOutput( out var output ) ;
		if( output is null ) return null ;
		return (TOutput)((IOutput) new Output(output)) ;
	}
	
	public void GetContainingOutput( out IOutput? containingOutput ) {
		var swapchain = ComObject ?? throw new NullReferenceException( ) ;
		swapchain.GetContainingOutput( out var output ) ;
		containingOutput = new Output( output ) ;
	}
	
	public void GetFrameStatistics( out FrameStatistics pStats ) {
		unsafe {
			DXGI_FRAME_STATISTICS result = default ;
			ComObject!.GetFrameStatistics( &result ) ;
			pStats = result ;
		}
	}
	
	public uint GetLastPresentCount( ) {
		ComObject!.GetLastPresentCount( out var count ) ;
		return count ;
	}

	// -----------------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGISwapChain ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain ).GUID
																.ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =================================================================================================================
} ;


[Wrapper(typeof(IDXGISwapChain1))]
internal class SwapChain1: SwapChain,
						   ISwapChain1,
						   IComObjectRef< IDXGISwapChain1 >,
						   IUnknownWrapper< IDXGISwapChain1 > {
	ComPtr< IDXGISwapChain1 >? _comPtr ;
	public new virtual ComPtr< IDXGISwapChain1 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGISwapChain1 >(  ) ;
	public override IDXGISwapChain1? ComObject => ComPointer?.Interface ;

	internal SwapChain1( ) {
		_comPtr = ComResources?.GetPointer< IDXGISwapChain1 >( ) ;
		if( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	internal SwapChain1( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( SwapChain1 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain1( in IDXGISwapChain1 dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( SwapChain1 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain1( ComPtr< IDXGISwapChain1 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}

	public SwapChainDescription1 GetDesc1( ) {
		GetDesc1( out var desc ) ;
		return desc ;
	}
	
	public void GetDesc1( out SwapChainDescription1 pDesc ) {
		unsafe {
			DXGI_SWAP_CHAIN_DESC1 result = default ;
			ComObject!.GetDesc1( &result ) ;
			pDesc = result ;
		}
	}
	
	public void GetFullscreenDesc( out SwapChainFullscreenDescription pDesc ) {
		
		unsafe {
			DXGI_SWAP_CHAIN_FULLSCREEN_DESC result = default ;
			ComObject!.GetFullscreenDesc( &result ) ;
			pDesc = new( result ) ;
		}
	}

	public void GetHwnd( out HWND pHwnd ) {
		
		unsafe {
			HWND result = default ;
			ComObject!.GetHwnd( &result ) ;
			pHwnd = result ;
		}
	}

	
	[SupportedOSPlatform("windows10.0.10240.0")]
	public void GetCoreWindow( Guid riid, out CoreWindow? ppUnk ) {
		unsafe {
			ComObject!.GetCoreWindow( &riid, out var unk ) ;
			var u = unk.As<CoreWindow>(  ) ;
			ppUnk = u ;
		}
	}
	
	public void Present1( uint syncInterval, PresentFlags flags, in PresentParameters pPresentParameters ) {
		
		unsafe {
			DXGI_PRESENT_PARAMETERS desc = pPresentParameters ;
			ComObject!.Present1( syncInterval, (uint)flags, &desc ) ;
		}
	}

	public bool IsTemporaryMonoSupported( ) => ComObject!.IsTemporaryMonoSupported( ) ;
	
	public void GetRestrictToOutput( out IOutput ppRestrictToOutput ) {
		ComObject!.GetRestrictToOutput( out var output ) ;
		ppRestrictToOutput = new Output( output ) ;
	}

	public void SetBackgroundColor( in RGBA pColor ) {
		
		unsafe {
			fixed( RGBA* p = &pColor ) {
				ComObject!.SetBackgroundColor( (DXGI_RGBA *)p ) ;
			}
		}
	}

	public void GetBackgroundColor( out RGBA pColor ) {
		
		pColor = default ;
		unsafe {
			DXGI_RGBA result = default ;
			ComObject!.GetBackgroundColor( &result ) ;
			pColor = result ;
		}
	}

	public void SetRotation( ModeRotation rotation ) {
		
		ComObject!.SetRotation( (DXGI_MODE_ROTATION)rotation ) ;
	}

	public void GetRotation( out ModeRotation pRotation ) {
		
		unsafe {
			DXGI_MODE_ROTATION result = default ;
			ComObject!.GetRotation( &result ) ;
			pRotation = (ModeRotation)result ;
		}
	}
	
	// -----------------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGISwapChain1 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain1 ).GUID
																 .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =================================================================================================================
} ;


[Wrapper( typeof( IDXGISwapChain2 ) )]
internal class SwapChain2: SwapChain1,
						   ISwapChain2,
						   IComObjectRef< IDXGISwapChain2 >,
						   IUnknownWrapper< IDXGISwapChain2 > {

	// -----------------------------------------------------------------------------------------------------------------

	ComPtr< IDXGISwapChain2 >? _comPtr ;

	public new ComPtr< IDXGISwapChain2 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGISwapChain2 >( ) ;

	public override IDXGISwapChain2? ComObject => ComPointer?.Interface ;

	// -----------------------------------------------------------------------------------------------------------------

	internal SwapChain2( ) {
		_comPtr = ComResources?.GetPointer< IDXGISwapChain2 >( ) ;
		if ( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	internal SwapChain2( nint ptr ) {
		if ( !ptr.IsValid( ) )
			throw new NullReferenceException( $"{nameof( SwapChain2 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain2( in IDXGISwapChain2 dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( SwapChain2 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain2( ComPtr< IDXGISwapChain2 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}

	// -----------------------------------------------------------------------------------------------------------------

	public void SetSourceSize( USize size ) =>
		ComObject!.SetSourceSize( size.Width, size.Height ) ;

	public void GetSourceSize( out USize size ) {
		ComObject!.GetSourceSize( out uint pWidth, out uint pHeight ) ;
		size = new( pWidth, pHeight ) ;
	}

	public void SetMaximumFrameLatency( uint maxLatency ) => 
		ComObject!.SetMaximumFrameLatency( maxLatency ) ;

	public void GetMaximumFrameLatency( out uint pMaxLatency ) => 
		ComObject!.GetMaximumFrameLatency( out pMaxLatency ) ;

	public Win32Handle GetFrameLatencyWaitableObject( ) => 
		ComObject!.GetFrameLatencyWaitableObject( ) ;

	public void SetMatrixTransform( in Matrix3x2F pMatrix ) {
		unsafe {
			fixed ( Matrix3x2F* p = &pMatrix ) {
				ComObject!.SetMatrixTransform( (DXGI_MATRIX_3X2_F*)p ) ;
			}
		}
	}

	public void GetMatrixTransform( out Matrix3x2F pMatrix ) {
		pMatrix = default ;
		unsafe {
			fixed ( Matrix3x2F* p = &pMatrix ) {
				ComObject!.GetMatrixTransform( (DXGI_MATRIX_3X2_F *)p ) ;
			}
		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGISwapChain2 ) ;

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain2 ).GUID
																 .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =================================================================================================================
} ;


[Wrapper(typeof(IDXGISwapChain3))]
[SupportedOSPlatform("windows10.0.10240")]
internal class SwapChain3: SwapChain2, 
						   ISwapChain3,
						   IComObjectRef< IDXGISwapChain3 >,
						   IUnknownWrapper< IDXGISwapChain3 > {
	// -----------------------------------------------------------------------------------------------------------------
	ComPtr< IDXGISwapChain3 >? _comPtr ;
	public new virtual ComPtr< IDXGISwapChain3 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGISwapChain3 >(  ) ;
	public override IDXGISwapChain3? ComObject => ComPointer?.Interface ;

	// -----------------------------------------------------------------------------------------------------------------
	
	internal SwapChain3( ) {
		_comPtr = ComResources?.GetPointer< IDXGISwapChain3 >( ) ;
		if( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	internal SwapChain3( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( SwapChain3 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain3( in IDXGISwapChain3 dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( SwapChain3 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain3( ComPtr< IDXGISwapChain3 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}

	// -----------------------------------------------------------------------------------------------------------------

	[SupportedOSPlatform("windows10.0.10240")]
	public uint GetCurrentBackBufferIndex( ) => ComObject!.GetCurrentBackBufferIndex( ) ;

	
	[SupportedOSPlatform("windows10.0.10240")]
	public void CheckColorSpaceSupport( ColorSpaceType colorSpace, out ISwapChain.ColorSpaceSupportFlags pColorSpaceSupport ) {
		unsafe {
			ComObject!.CheckColorSpaceSupport( (DXGI_COLOR_SPACE_TYPE)colorSpace, out var support ) ;
			pColorSpaceSupport = (ISwapChain.ColorSpaceSupportFlags)support ;
		}
	}
	
	
	[SupportedOSPlatform("windows10.0.10240")]
	public void SetColorSpace1( ColorSpaceType colorSpace ) {
		
		ComObject!.SetColorSpace1( (DXGI_COLOR_SPACE_TYPE)colorSpace ) ;
	}

	
	[SupportedOSPlatform("windows10.0.10240")]
	public HResult ResizeBuffers1( uint bufferCount = 0U,
								   uint width = 0U, uint height = 0U,
								   Format format = Format.UNKNOWN,
								   SwapChainFlags swapChainFlags = SwapChainFlags.None,
								   uint[ ]? pCreationNodeMask = null,
								   Direct3D12.ICommandQueue[ ]? ppPresentQueue = null  ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ArgumentNullException.ThrowIfNull( ppPresentQueue, nameof( ppPresentQueue ) ) ;
#endif
		
		if( pCreationNodeMask is null ) {
			pCreationNodeMask = Enumerable.Repeat( 0U, (int)bufferCount )
											.ToArray( ) ;
		}
		var swapchain = ComObject ?? throw new NullReferenceException( ) ;
		var presentQueue = new object[ bufferCount ] ;
		
		for( int i = 0; i < bufferCount; ++i ) {
			presentQueue[ i ] = ( (IComObjectRef<ID3D12CommandQueue>?)ppPresentQueue?[ i ] )!.ComObject
#if DEBUG || DEBUG_COM || DEV_BUILD
								?? throw new NullReferenceException( )
#else
				!
#endif
				;
		}
		
		unsafe { fixed ( uint* pMask = pCreationNodeMask ) {
				var hr = swapchain.ResizeBuffers1( bufferCount,
										  width, height,
										  (DXGI_FORMAT)format,
										  (uint)swapChainFlags,
										  pCreationNodeMask,
										  presentQueue ) ;
				return hr ;
			}
		}
	}
	
	// -----------------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGISwapChain3 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain3 ).GUID
																 .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =================================================================================================================
}


[Wrapper( typeof( IDXGISwapChain4 ) )]
[SupportedOSPlatform( "windows10.0.10240" )]
internal class SwapChain4: SwapChain3,
						   ISwapChain4,
						   IComObjectRef< IDXGISwapChain4 >,
						   IUnknownWrapper< IDXGISwapChain4 > {
	// -----------------------------------------------------------------------------------------------------------------
	ComPtr< IDXGISwapChain4 >? _comPtr ;

	public new virtual ComPtr< IDXGISwapChain4 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGISwapChain4 >( ) ;

	public override IDXGISwapChain4? ComObject => ComPointer?.Interface ;

	// -----------------------------------------------------------------------------------------------------------------

	internal SwapChain4( ) {
		_comPtr = ComResources?.GetPointer< IDXGISwapChain4 >( ) ;
		if ( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	internal SwapChain4( nint ptr ) {
		if ( !ptr.IsValid( ) )
			throw new NullReferenceException( $"{nameof( SwapChain4 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain4( in IDXGISwapChain4 dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( SwapChain4 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SwapChain4( ComPtr< IDXGISwapChain4 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}

	// -----------------------------------------------------------------------------------------------------------------

	public void SetHDRMetaData( HDRMetaDataType Type, uint Size,
								in HDRMetaDataHDR10? pMetaData = default ) {
		var swapchain = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			if ( pMetaData is null ) {
				swapchain.SetHDRMetaData( (DXGI_HDR_METADATA_TYPE)Type, Size, null ) ;
			}
			else {
				HDRMetaDataHDR10 data = pMetaData.Value ;
				swapchain.SetHDRMetaData( (DXGI_HDR_METADATA_TYPE)Type, Size, (DXGI_HDR_METADATA_HDR10 *)&data ) ;
			}
		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGISwapChain4 ) ;

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain4 ).GUID
																 .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =================================================================================================================
} ;




//! TODO: Find out what the deal with these is. Where the hell did these come from? lol
/*public void SetSourceSize( uint width, uint height ) { }
public void GetSourceSize( out uint pWidth, out uint pHeight ) { }
public void SetMaximumFrameLatency( uint maxLatency ) { }
public void GetMaximumFrameLatency( out uint pMaxLatency ) { }
public void GetFrameLatencyWaitableObject( out HANDLE pHandle ) { }
public void SetMatrixTransform( in Matrix3x2 pMatrix ) { }
public void GetMatrixTransform( out Matrix3x2 pMatrix ) { }
public uint GetCurrentBackBufferIndex() { }
public void CheckColorSpaceSupport( ColorSpaceType colorSpace, out ColorSpaceSupportFlags pColorSpaceSupport ) { }
public void SetColorSpace1( ColorSpaceType colorSpace ) { }
public void ResizeBuffers1( uint bufferCount, uint width, uint height, Format newFormat, SwapChainFlags swapChainFlags,
							in uint[] pCreationNodeMask, in IUnknown[] ppPresentQueue ) { }
							*/