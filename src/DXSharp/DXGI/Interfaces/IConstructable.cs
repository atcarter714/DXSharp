using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;

namespace DXSharp.DXGI ;

/*internal delegate TWrapper ConstructWrapper< TWrapper, TUnknown >( TUnknown unknown )
									where TWrapper: IUnknownWrapper< TUnknown >
									where TUnknown: IUnknown ;


internal interface IConstructable< out T > where T: IUnknownWrapper {
	internal static abstract T ConstructEmpty( ) ;
} ;

internal interface IConstructable< out TWrapper, in TUnknown >
									where TWrapper: IUnknownWrapper 
									where TUnknown: IUnknown {
	internal static abstract TWrapper ConstructWith( TUnknown arg1 ) ;
} ;*/

internal static class ComWrapperFactory {
	
	/*internal static TWrapper Create< TWrapper, TUnknown >( TUnknown unknown )
		where TWrapper: IUnknownWrapper< TUnknown >, IConstructable< TWrapper, TUnknown >
		where TUnknown: IUnknown {
		 return TWrapper.ConstructWith( unknown ) ;
	}*/

	/*static void f() {
		Adapter.Instantiate< Adapter1 >( ) ;
		SwapChain1.Instantiate( ) ;
	}*/
} ;