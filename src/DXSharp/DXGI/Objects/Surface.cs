#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
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
	
	public static implicit operator SurfaceDescription( in DXGI_SURFACE_DESC desc ) =>
		new( desc.Width, desc.Height, (Format)desc.Format, desc.SampleDesc ) ;
	public static unsafe implicit operator DXGI_SURFACE_DESC( in SurfaceDescription desc ) =>
	 		new DXGI_SURFACE_DESC {
			Width = desc.Width, Height = desc.Height,
			Format = (DXGI_FORMAT)desc.Format, SampleDesc = desc.SampleDesc, } ;
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


// Wrapper Interface of IDXGISurface ::
//[ComImport, Guid( "cafcb56c-6ac3-4889-bf47-9e23bbd260ec" )]
//[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]

public interface ISurface:  IDeviceSubObject,
							DXGIWrapper< IDXGISurface > {
	void GetDesc( out SurfaceDescription pDesc ) ;
	void Map( ref MappedRect pLockedRect, uint MapFlags ) ;
	void Unmap( ) ;
} ;


//! Concrete Implementation of an IDXGISurface wrapper/proxy ::
public class Surface: DeviceSubObject, ISurface {
	public ComPtr? ComPtrBase => ComPointer ;
	public new ComPtr< IDXGISurface >? ComPointer { get ; protected set ; }
	public new IDXGISurface? COMObject => ComPointer?.Interface as IDXGISurface ;

	internal Surface( ) { }
	public Surface( nint ptr ): base(ptr) {
		if ( !ptr.IsValid() )
			throw new NullReferenceException( $"{nameof( Surface )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		ComPointer = new( ptr ) ;
	}
	public Surface( in IDXGISurface dxgiObj ): base( dxgiObj ) {
		if ( dxgiObj is null )
			throw new NullReferenceException( $"{nameof( Surface )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
		ComPointer = new( dxgiObj ) ;
	}
	
	
	public void GetDesc( out SurfaceDescription pDesc ) { pDesc = default ; }
	public void Map( ref MappedRect pLockedRect, uint MapFlags ) { }
	public void Unmap( ) { }
} ;


	/*public T GetDevice< T >( ) where T: class, IDevice {
		this._throwIfDestroyed( ) ;
		unsafe {
			var riid = typeof( T ).GUID ;
			this.COMObject?.GetDevice( &riid, out var ppDevice ) ;
			return new Device( ppDevice as IDXGIDevice ) ;
		}
	}*/
	
	/*public void GetDevice< T >( out T ppDevice ) where T: class, IDevice {
		ppDevice = default ;
		var _obj = COMUtility.GetDXGIObject< IDXGIDeviceSubObject >( this.BasePointer ) ;
		if ( _obj is { } subObject ) {
			unsafe {
				var riid = typeof(IDXGIDevice).GUID ;
				subObject.GetDevice( &riid, out var pDevice ) ;
				ppDevice = (T)pDevice ;
			}
		}
		else throw new NullReferenceException( $"{nameof(Surface)} :: " +
											   $"The internal COM interface is destroyed/null." ) ;
	}*/
