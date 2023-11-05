#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
using Windows.Win32.Graphics.Gdi ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;

#endregion

namespace DXSharp.DXGI ;






//! Concrete Implementation of an IDXGISurface wrapper/proxy ::
[Wrapper(typeof(IDXGISurface))]
internal class Surface: DeviceSubObject,
						ISurface,
						IComObjectRef< IDXGISurface >,
						IUnknownWrapper< IDXGISurface > {
	// --------------------------------------------------------------------------------------------
	ComPtr< IDXGISurface >? _comPtr ;
	public new ComPtr< IDXGISurface >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGISurface >(  ) ;
	public override IDXGISurface? ComObject => ComPointer?.Interface as IDXGISurface ;
	
	// --------------------------------------------------------------------------------------------
	
	internal Surface( ) {
		_comPtr = ComResources?.GetPointer< IDXGISurface >( ) ;
		if( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	internal Surface( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( Surface )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Surface( in IDXGISurface dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( Surface )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------
	public void GetDesc( out SurfaceDescription pDesc ) {
		var surface = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			DXGI_SURFACE_DESC desc ;
			surface.GetDesc( &desc ) ;
			pDesc = desc ;
		}
	}
	
	public void Map( ref MappedRect pLockedRect, MapFlags mapFlags ) {
		var surface = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed( MappedRect* pRect = &pLockedRect ) {
				surface.Map( (DXGI_MAPPED_RECT *)pRect, (uint)mapFlags ) ;
				pLockedRect = *pRect ;
			}
		}
	}
	
	public void Unmap( ) {
		var surface = ComObject ?? throw new NullReferenceException( ) ;
		surface.Unmap( ) ;
	}
	
	// --------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGISurface ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISurface ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}

	public static IInstantiable  Instantiate( ) => new Surface( ) ;
	public static IInstantiable Instantiate( IntPtr ptr ) => new Surface( ptr ) ;
	public static IInstantiable Instantiate< ICom >( ICom dxgiObj ) where ICom: IUnknown? => 
		new Surface( (IDXGISurface)dxgiObj! ) ;
	// ============================================================================================
} ;


[Wrapper(typeof(IDXGISurface1))]
internal class Surface1: Surface, 
						 ISurface1,
						 IComObjectRef< IDXGISurface1 >,
						 IUnknownWrapper< IDXGISurface1 > {
	// --------------------------------------------------------------------------------------------
	
	ComPtr< IDXGISurface1 >? _comPtr ;
	public new ComPtr< IDXGISurface1 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGISurface1 >(  ) ;
	public override IDXGISurface1? ComObject => ComPointer?.Interface as IDXGISurface1 ;
	
	// --------------------------------------------------------------------------------------------
	
	internal Surface1( ) {
		_comPtr = ComResources?.GetPointer< IDXGISurface1 >( ) ;
		if( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	internal Surface1( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( Surface1 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Surface1( in IDXGISurface1 dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( Surface1 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Surface1( ComPtr< IDXGISurface1 > ptr ) {
		ArgumentNullException.ThrowIfNull( ptr, nameof( ptr ) ) ;
		_comPtr = ptr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// --------------------------------------------------------------------------------------------
	
	/// <inheritdoc cref="ISurface1.GetDC"/>
	public void GetDC( bool Discard, ref HDC phdc ) {
		var surface1 = ComObject ?? throw new NullReferenceException( ) ;
		unsafe { 
			fixed( HDC* pHdc = &phdc ) {
				surface1.GetDC( Discard, pHdc ) ;
				phdc = *pHdc ;
			}
		}
	}

	/// <inheritdoc cref="ISurface1.ReleaseDC"/>
	public void ReleaseDC( in Rect? pDirtyRect = default ) {
		var surface1 = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			if( pDirtyRect is null )
				surface1.ReleaseDC( null ) ;
			else {
				Rect Rect = pDirtyRect.Value ;
				surface1.ReleaseDC( (RECT *)&Rect ) ;
			}
		}
	}
	
	// --------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGISurface1 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISurface1 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ============================================================================================
} ;



[Wrapper(typeof(IDXGISurface2))]
internal class Surface2: Surface1, ISurface2 {
	// --------------------------------------------------------------------------------------------
	ComPtr< IDXGISurface2 >? _comPtr ;
	public new ComPtr< IDXGISurface2 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGISurface2 >(  ) ;
	public override IDXGISurface2? ComObject => ComPointer?.Interface as IDXGISurface2 ;
	
	// --------------------------------------------------------------------------------------------
	 
	internal Surface2( ) {
		_comPtr = ComResources?.GetPointer< IDXGISurface2 >( ) ;
		if( _comPtr is not null )
			_initOrAdd( _comPtr ) ;
	}
	internal Surface2( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( Surface2 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Surface2( in IDXGISurface2 dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( Surface2 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Surface2( ComPtr< IDXGISurface2 > ptr ) {
		ArgumentNullException.ThrowIfNull( ptr, nameof( ptr ) ) ;
		_comPtr = ptr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// --------------------------------------------------------------------------------------------
	
	/// <inheritdoc cref="ISurface2.GetResource{ TRes }"/>
	public void GetResource< TRes >( in Guid riid, out TRes ppParentResource, out uint pSubresourceIndex )
																				where TRes: IUnknownWrapper, 
																							IInstantiable {
		unsafe {
			var surface2 = ComObject ?? throw new NullReferenceException( ) ;
			fixed( Guid* pRiid = &riid ) {
				surface2.GetResource( pRiid, out var parentResource, out pSubresourceIndex) ;
				ppParentResource = (TRes)TRes.Instantiate( (parentResource as IDXGIResource)! ) ;
			}
		}
	}
	
	// --------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGISurface2 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISurface2 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ============================================================================================
} ;