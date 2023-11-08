#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
using HResult = DXSharp.Windows.HResult ;
#endregion
namespace DXSharp.DXGI ;


[Wrapper( typeof( IDXGIFactoryMedia ) )]
internal class FactoryMedia: DisposableObject,
							 IFactoryMedia,
							 IComObjectRef< IDXGIFactoryMedia >,
							 IUnknownWrapper< IDXGIFactoryMedia > {
	protected COMResource? ComResources { get ; set ; }
	void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if ( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer< T >( ptr ) ;
	}

	protected ComPtr< IDXGIFactoryMedia >? _comptr ;
	public virtual ComPtr? ComPointer => _comptr ??= ComResources?.GetPointer< IDXGIFactoryMedia >( ) ;
	public virtual ComPtr? ComPtrBase => this.ComPointer ;

	public virtual IDXGIFactoryMedia? ComObject =>
		ComPointer?.InterfaceObjectRef as IDXGIFactoryMedia ;

	
	// ---------------------------------------------------------------------------
	internal FactoryMedia( ) {
		_comptr = ComResources?.GetPointer< IDXGIFactoryMedia >( ) ;
	}

	internal FactoryMedia( nint ptr ) {
		_comptr = new( ptr ) ;
		_initOrAdd( _comptr ) ;
	}

	internal FactoryMedia( in IDXGIFactoryMedia dxgiObj ) {
		_comptr = new( dxgiObj ) ;
		_initOrAdd( _comptr ) ;
	}

	internal FactoryMedia( ComPtr< IDXGIFactoryMedia > otherPtr ) {
		_comptr = otherPtr ;
		_initOrAdd( _comptr ) ;
	}
	~FactoryMedia( ) => Dispose( false ) ;
	// ---------------------------------------------------------------------------

	public unsafe void CreateSwapChainForCompositionSurfaceHandle( IDXCOMObject pDevice,
																   Win32Handle hSurface,
																   in SwapChainDescription1 pDesc,
																   [Optional] IOutput? pRestrictToOutput,
																   out ISwapChain1 ppSwapChain ) {
		var fmedia = ComObject ?? throw new NullReferenceException( ) ;
		var device = (IComObjectRef< ID3D12Device >?)pDevice ?? null ;
		var output  = (IComObjectRef< IDXGIOutput >?)pRestrictToOutput ?? null ;
		
		fixed( void *pDesc_ = &pDesc ) {
			fmedia.CreateSwapChainForCompositionSurfaceHandle( device?.ComObject!, hSurface,
															   (DXGI_SWAP_CHAIN_DESC1 *)pDesc_,
															   ( output?.ComObject ?? null )!,
																 out var swapChain1 ) ;

#if DEBUG || DEBUG_COM || DEV_BUILD
			if ( swapChain1 is null )
				throw new DirectXComError( $"Failed to create {nameof(ISwapChain1)} " +
										   $"({nameof(IDXGISwapChain1)}) interface!" ) ;
#endif
			ppSwapChain = new SwapChain1( swapChain1! ) ;
		}
	}
	

	public void CreateDecodeSwapChainForCompositionSurfaceHandle( IDXCOMObject                  pDevice,
																  Win32Handle                   hSurface,
																  in DecodeSwapChainDescription pDesc,
																  IResource                     pYuvDecodeBuffers,
																  [Optional] IOutput?           pRestrictToOutput,
																  out        IDecodeSwapChain?  ppSwapChain ) {
		var fmedia = ComObject ?? throw new NullReferenceException( ) ;
		var device = (IComObjectRef< ID3D12Device >?)pDevice ?? null ;
		var output  = (IComObjectRef< IDXGIOutput >?)pRestrictToOutput ?? null ;
		var pYuvDecode = (IComObjectRef< IDXGIResource >?)pYuvDecodeBuffers ?? null ;

		unsafe {
			fixed ( DecodeSwapChainDescription* _desc = &pDesc ) {
				fmedia.CreateDecodeSwapChainForCompositionSurfaceHandle( device?.ComObject!, hSurface,
																		 (DXGI_DECODE_SWAP_CHAIN_DESC *)_desc,
																		  ( pYuvDecode?.ComObject )!,
																						  ( output?.ComObject )!, out var decodeSwapChainObj ) ;
#if DEBUG || DEBUG_COM || DEV_BUILD
						if ( decodeSwapChainObj is null )
							throw new DirectXComError( $"Failed to create {nameof(IDecodeSwapChain)} " +
													   $"({nameof(IDXGIDecodeSwapChain)}) interface!" ) ;
#endif
						ppSwapChain = new DecodeSwapChain( decodeSwapChainObj ) ;
			}
		}
	}


	// ---------------------------------------------------------------------------
	protected override ValueTask DisposeUnmanaged( ) {
		_comptr?.Dispose( ) ;
		return ValueTask.CompletedTask ;
	}

	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	// ---------------------------------------------------------------------------
	public static Type ComType => typeof(IDXGIFactoryMedia) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof( IDXGIFactoryMedia ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ===========================================================================
} ;