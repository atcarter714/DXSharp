#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
using DXSharp.Windows ;
#endregion
namespace DXSharp.DXGI ;

[Wrapper( typeof( IDXGIDecodeSwapChain ) )]
internal class DecodeSwapChain: DisposableObject,
								IDecodeSwapChain,
								IComObjectRef< IDXGIDecodeSwapChain >,
								IUnknownWrapper< IDXGIDecodeSwapChain > {
	protected COMResource? ComResources { get ; set ; }
	void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if ( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer< T >( ptr ) ;
	}

	protected ComPtr< IDXGIDecodeSwapChain >? _comptr ;
	public virtual ComPtr? ComPointer => _comptr ??= ComResources?.GetPointer< IDXGIDecodeSwapChain >( ) ;
	public virtual ComPtr? ComPtrBase => this.ComPointer ;

	public virtual IDXGIDecodeSwapChain? ComObject =>
		ComPointer?.InterfaceObjectRef as IDXGIDecodeSwapChain ;

	
	// ---------------------------------------------------------------------------
	internal DecodeSwapChain( ) {
		_comptr = ComResources?.GetPointer< IDXGIDecodeSwapChain >( ) ;
		if ( _comptr is not null ) _initOrAdd( _comptr ) ;
	}
	internal DecodeSwapChain( nint ptr ) {
		_comptr = new( ptr ) ;
		_initOrAdd( _comptr ) ;
	}
	internal DecodeSwapChain( in IDXGIDecodeSwapChain dxgiObj ) {
		_comptr = new( dxgiObj ) ;
		_initOrAdd( _comptr ) ;
	}
	internal DecodeSwapChain( ComPtr< IDXGIDecodeSwapChain > otherPtr ) {
		_comptr = otherPtr ;
		_initOrAdd( _comptr ) ;
	}
	~DecodeSwapChain( ) => Dispose( false ) ;
	// ---------------------------------------------------------------------------
	
	public HResult PresentBuffer( uint BufferToPresent, uint SyncInterval, PresentFlags Flags ) => 
		ComObject!.PresentBuffer( BufferToPresent, SyncInterval, (uint)Flags ) ;

	public void SetSourceRect( in Rect pRect ) {
		unsafe {
			fixed( Rect* pRectPtr = &pRect ) {
				ComObject!.SetSourceRect( pRectPtr ) ;
			}
		}
	}

	public void SetTargetRect( in Rect pRect ) {
		unsafe {
			fixed( Rect* pRectPtr = &pRect ) {
				ComObject!.SetTargetRect( pRectPtr ) ;
			}
		}
	}

	public void SetDestSize( uint Width, uint Height ) => 
		ComObject!.SetDestSize( Width, Height ) ;

	public void GetSourceRect( out Rect pRect ) {
		unsafe {
			Rect rect = default ;
			ComObject!.GetSourceRect( &rect ) ;
			pRect = rect ;
		}
	}

	public void GetTargetRect( out Rect pRect ) {
		unsafe {
			Rect rect = default ;
			ComObject!.GetTargetRect( &rect ) ;
			pRect = rect ;
		}
	}

	public void GetDestSize( out uint pWidth, out uint pHeight ) => 
		ComObject!.GetDestSize( out pWidth, out pHeight ) ;

	public void SetColorSpace( MultiplaneOverlayYCbCrFlags ColorSpace ) => 
		ComObject!.SetColorSpace( ColorSpace ) ;

	public MultiplaneOverlayYCbCrFlags GetColorSpace( ) => 
		ComObject!.GetColorSpace( ) ;


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
	public static Type ComType => typeof(IDXGIDecodeSwapChain) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof( IDXGIDecodeSwapChain ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ===========================================================================
} ;
