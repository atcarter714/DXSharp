using Windows.Win32.Foundation ;
using DXSharp.DXGI ;

namespace Windows.Win32.Graphics.Dxgi ;


public partial struct DXGI_SHARED_RESOURCE
{
	public DXGI_SHARED_RESOURCE( HANDLE handle ) => Handle = handle ;
	public DXGI_SHARED_RESOURCE( nint handle ) => Handle = new( handle ) ;
	
	
	public static implicit operator SharedResource( in DXGI_SHARED_RESOURCE res ) => new( res.Handle ) ;
	public static implicit operator DXGI_SHARED_RESOURCE( in SharedResource src ) => new( src.Handle ) ;
}