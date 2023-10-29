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
public class Surface: DeviceSubObject, ISurface {

	public static IInstantiable  Instantiate( )            => new Surface( ) ;
	public static IInstantiable Instantiate( IntPtr ptr ) => new Surface( ptr ) ;
	public static IInstantiable Instantiate< ICom >( ICom dxgiObj ) where ICom: IUnknown? => 
		new Surface( (IDXGISurface)dxgiObj! ) ;

	public new ComPtr< IDXGISurface >? ComPointer { get ; protected set ; }
	public new IDXGISurface? COMObject => ComPointer?.Interface as IDXGISurface ;

	
	internal Surface( ) { }
	internal Surface( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( Surface )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		ComPointer = new( ptr ) ;
	}
	internal Surface( in IDXGISurface dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( Surface )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		ComPointer = new( dxgiObj ) ;
	}
	
	
	public void GetDesc( out SurfaceDescription pDesc ) { pDesc = default ; }
	public void Map( ref MappedRect pLockedRect, uint MapFlags ) { }
	public void Unmap( ) { }
} ;


public class Surface1: Surface, ISurface1 {
	public new ComPtr< IDXGISurface1 >? ComPointer { get ; protected set ; }
	public new IDXGISurface1? COMObject => ComPointer?.Interface as IDXGISurface1 ;

	public static IInstantiable  Instantiate( )            => new Surface1( ) ;
	public static IInstantiable Instantiate( IntPtr ptr ) => new Surface1( ptr ) ;
	public new static IInstantiable Instantiate< ICom >( ICom dxgiObj ) where ICom: IUnknown? => 
		new Surface1( (IDXGISurface1)dxgiObj! ) ;
	
	internal Surface1( ) { }
	internal Surface1( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( Surface1 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		ComPointer = new( ptr ) ;
	}
	internal Surface1( in IDXGISurface1 dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( Surface1 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		ComPointer = new( dxgiObj ) ;
	}
	
	
	/// <inheritdoc cref="ISurface1.GetDC"/>
	public void GetDC( bool Discard, ref HDC phdc ) {
		var surface1 = COMObject ?? throw new NullReferenceException( ) ;
		unsafe { 
			fixed( HDC* pHdc = &phdc ) {
				surface1.GetDC( Discard, pHdc ) ;
				phdc = *pHdc ;
			}
		}
	}

	/// <inheritdoc cref="ISurface1.ReleaseDC"/>
	public void ReleaseDC( in Rect? pDirtyRect = default ) {
		var surface1 = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			if( pDirtyRect is null )
				surface1.ReleaseDC( null ) ;
			else {
				Rect Rect = pDirtyRect.Value ;
				surface1.ReleaseDC( (RECT *)&Rect ) ;
			}
		}
	}
	
} ;


public class Surface2: Surface1, ISurface2 {
	public new ComPtr< IDXGISurface2 >? ComPointer { get ; protected set ; }
	public new IDXGISurface2? COMObject => ComPointer?.Interface as IDXGISurface2 ;

	public static IInstantiable  Instantiate( )            => new Surface2( ) ;
	public static IInstantiable Instantiate( IntPtr ptr ) => new Surface2( ptr ) ;
	public new static IInstantiable Instantiate< ICom >( ICom dxgiObj ) where ICom: IUnknown? => 
		new Surface2( (IDXGISurface2)dxgiObj! ) ;
	
	internal Surface2( ) { }
	internal Surface2( nint ptr ) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( Surface2 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		ComPointer = new( ptr ) ;
	}
	internal Surface2( in IDXGISurface2 dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( Surface2 )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		ComPointer = new( dxgiObj ) ;
	}

	
	/// <inheritdoc cref="ISurface2.GetResource{ TRes }"/>
	public void GetResource< TRes >( in Guid riid, out TRes ppParentResource, out uint pSubresourceIndex )
																				where TRes: IUnknownWrapper, 
																							IInstantiable {
		unsafe {
			var surface2 = COMObject ?? throw new NullReferenceException( ) ;
			fixed( Guid* pRiid = &riid ) {
				surface2.GetResource( pRiid, out var parentResource, out pSubresourceIndex) ;
				ppParentResource = (TRes)TRes.Instantiate( (parentResource as IDXGIResource)! ) ;
			}
		}
	}
	
} ;