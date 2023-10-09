using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using static Windows.Win32.PInvoke ;
namespace DXSharp ;


public static class DirectX {
	internal static TFactory CreateFactory< TFactory >( Type factoryType )
		where TFactory: class, IFactory< IDXGIFactory > {
		ArgumentNullException.ThrowIfNull( factoryType, nameof(factoryType) ) ;
		if ( !typeof( TFactory ).IsAssignableFrom( factoryType ) )
			throw new ArgumentException( $"{nameof(factoryType)} must be assignable to TFactory" ) ;
	}
	
	internal static TFactory CreateFactory0< TFactory >( ) 
		where TFactory: class, IFactory< IDXGIFactory >, new() 
	{
		object? ppFactory    = null ;
		HResult createResult = default ;
		var     guid         = TFactory.InterfaceGUID ;
		
		unsafe { createResult = CreateDXGIFactory( &guid, out ppFactory ) ; }
		if ( createResult.Failed || ppFactory is null )
			throw new DirectXComError( "Failed to create DXGI Factory!" ) ;
		
		var factory = new TFactory( ) ;
		factory.SetComPointer( new( (ppFactory as IDXGIFactory)! ) ) ;
		return factory ;
	}
	
	public static TFactory CreateFactory< TFactory >( )
		where TFactory: class, IFactory< IDXGIFactory > => null ;
}