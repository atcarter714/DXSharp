#region Using Directives
using DXSharp.DXGI ;
using DXSharp.Windows ;
using Windows.Win32.Graphics.Dxgi ;
using static Windows.Win32.PInvoke ;
#endregion
namespace DXSharp ;


public static class DXFunctions {
	
	internal static TFactory? CreateFactory< TFactory >( )
		where TFactory: class, IFactory< IDXGIFactory >
	{
		if( typeof(TFactory) == typeof(Factory) ) {
			var dxgiFactory = DXGIFunctions.CreateDXGIFactory< IDXGIFactory >( ) ;
			return dxgiFactory is null ? null : new Factory( dxgiFactory ) as TFactory ;
		}
		else if( typeof(TFactory) == typeof(Factory1) ) {
			var dxgiFactory = DXGIFunctions.CreateDXGIFactory< IDXGIFactory1 >( ) ;
			return dxgiFactory is null ? null : new Factory1( dxgiFactory ) as TFactory ;
		}
		
		
		
		else throw new ArgumentException( $"{nameof(TFactory)} is not supported!" ) ;
	}
	
	internal static TFactory CreateFactory0< TFactory >( ) 
		where TFactory: class, IFactory< IDXGIFactory >, new() {
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
	
}


/*ArgumentNullException.ThrowIfNull( factoryType, nameof(factoryType) ) ;
		if ( !typeof( TFactory ).IsAssignableFrom(factoryType) )
			throw new ArgumentException( $"{nameof(factoryType)} must be assignable to {nameof(TFactory)}!" ) ;*/