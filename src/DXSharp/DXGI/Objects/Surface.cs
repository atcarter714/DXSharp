#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;

#endregion

namespace DXSharp.DXGI ;



[StructLayout( LayoutKind.Sequential )]
public struct SurfaceDescription {
	public uint Width, 
				Height ;
	public Format Format ;
	public SampleDescription SampleDesc ;
	public SurfaceDescription( uint width, uint height, Format format,
							   SampleDescription sampleDesc ) {
		Width = width ; Height = height ;
		Format = format ; SampleDesc = sampleDesc ;
	}
} ;


[StructLayout( LayoutKind.Sequential )]
public struct MappedRect { 
	public int Pitch ; public nint pBits ;
	public MappedRect( int pitch, nint bits ) {
		Pitch = pitch ; pBits = bits ;
	}
	public MappedRect( in DXGI_MAPPED_RECT rect ) {
		Pitch = rect.Pitch ;
		unsafe { pBits = (nint)rect.pBits ; }
	}
	public static implicit operator MappedRect( in DXGI_MAPPED_RECT rect ) => new( rect ) ;
	public static unsafe implicit operator DXGI_MAPPED_RECT( in MappedRect rect ) => new DXGI_MAPPED_RECT {
		Pitch = rect.Pitch, pBits = (byte*)rect.pBits, } ;
} ;


[ComImport, Guid( "cafcb56c-6ac3-4889-bf47-9e23bbd260ec" )]
[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface ISurface: IObject<ISurface> {
	void GetDesc( out SurfaceDescription pDesc ) ;
	void Map( ref MappedRect pLockedRect, uint MapFlags ) ;
	void Unmap( ) ;
} ;



// ------------------------------------------------------------
//! TODO: Decide if we should remove this / don't need it
/*[ComImport, Guid( "cafcb56c-6ac3-4889-bf47-9e23bbd260ec" )]
[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IDXGISurface {
	void GetDesc( out SurfaceDescription pDesc ) ;
	void Map( ref MappedRect pLockedRect, uint MapFlags ) ;
	void Unmap( ) ;
} ;*/


public class Surface: Object,
					  ISurface,
					  IObject< Surface >,
					  IDeviceSubObject,
					  IDXGIObjWrapper<IDXGISurface> {
	IDXGISurface? _surface ;
	internal IDXGISurface? _dxgiSurface ;
	
	public new IDXGISurface? COMObject { get ; init ; }
	public new ComPtr< ISurface >? ComPtr { get ; init ; }

	
	public void GetDevice< T >( out T ppDevice ) where T: class, IDevice {
		var _obj = COMUtility.GetDXGIObject< IDXGIDeviceSubObject >( this.ComPtr?.IUnknownAddress ?? 0 ) ;
		if ( _obj is { } subObject ) {
			unsafe {
				var riid = typeof(IDXGIDevice).GUID ;
				subObject.GetDevice( &riid, out var pDevice ) ;
				ppDevice = (T)pDevice ;
			}
		}
		else throw new NullReferenceException( $"{nameof(Surface)} :: " +
											   $"The internal COM interface is destroyed/null." ) ;
	}

	public void GetDesc( out SurfaceDescription pDesc ) { pDesc = default ; }
	public void Map( ref MappedRect pLockedRect, uint MapFlags ) { }
	public void Unmap( ) { }

} ;