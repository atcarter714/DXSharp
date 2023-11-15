#pragma warning disable CA1416
#region Using Directives
using System.Collections ;
using System.Collections.ObjectModel ;
using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.DXGI.Debugging ;
using DXSharp.Windows.COM ;
using DXSharp.XTensions ;
#endregion
namespace Windows.Win32.Graphics.Dxgi ;


internal static class DXGIWrapperFactory {
	//! Lookup wrapper creation functions by native COM interface type:
	internal static ReadOnlyDictionary< Type, IEnumerable > _creationFunctionEnumerablesMap =
		new( new Dictionary< Type, IEnumerable  > {
			{ typeof(IDXGIDebug), IDebug._layerCreationFunctions },
			{ typeof(IDXGIDevice), IDevice._deviceCreationFunctions },
			{ typeof(IDXGIOutput), IOutput._outputCreationFunctions },
			{ typeof(IDXGIOutputDuplication), IOutputDuplication._outputDuplicationCreationFunctions },
			{ typeof(IDXGIAdapter), IAdapter._adapterCreationFunctions },
			{ typeof(IDXGISwapChain), ISwapChain._swapChainCreationFunctions  },
			{ typeof(IDXGIFactory), IFactory._factoryCreationFunctions },
			{ typeof(IDXGIFactoryMedia), IFactoryMedia._factoryMediaCreationFunctions },
			{ typeof(IDXGIKeyedMutex), IKeyedMutex._keyedMutexCreationFunctions },
			{ typeof(IDXGIResource), IResource._resourceCreationFunctions },
			{ typeof(IDXGISurface), ISurface._surfaceCreationFunctions },
			{ typeof(IDXGIDecodeSwapChain), IDecodeSwapChain._decodeSwapChainCreationFunctions },
		} ) ;
	
	//! Lookup native interop RCW Interface by COM IID:
	internal static ReadOnlyDictionary< Guid, Type > _rcwTypesByCOMIID =
		new( new Dictionary< Guid, Type > {
			{ typeof(IDXGIDebug).GUID, typeof(IDXGIDebug) },
			{ typeof(IDXGIDevice).GUID, typeof(IDXGIDevice) },
			{ typeof(IDXGIOutput).GUID, typeof(IDXGIOutput) },
			{ typeof(IDXGIOutputDuplication).GUID, typeof(IDXGIOutputDuplication) },
			{ typeof(IDXGIAdapter).GUID, typeof(IDXGIAdapter) },
			{ typeof(IDXGISwapChain).GUID, typeof(IDXGISwapChain) },
			{ typeof(IDXGIFactory).GUID, typeof(IDXGIFactory) },
			{ typeof(IDXGIFactoryMedia).GUID, typeof(IDXGIFactoryMedia) },
			{ typeof(IDXGIKeyedMutex).GUID, typeof(IDXGIKeyedMutex) },
			{ typeof(IDXGIResource).GUID, typeof(IDXGIResource) },
			{ typeof(IDXGISurface).GUID, typeof(IDXGISurface) },
			{ typeof(IDXGIDecodeSwapChain).GUID, typeof(IDXGIDecodeSwapChain) },
		} ) ;
	
	//! Lookup Proxy Interface by COM IID:
	internal static ReadOnlyDictionary< Guid, Type > _proxyTypesByCOMIID =
		new( new Dictionary< Guid, Type > {
			{ typeof(IDXGIDebug).GUID, typeof(IDebug) },
			{ typeof(IDXGIDevice).GUID, typeof(IDevice) },
			{ typeof(IDXGIOutput).GUID, typeof(IOutput) },
			{ typeof(IDXGIOutputDuplication).GUID, typeof(IOutputDuplication) },
			{ typeof(IDXGIAdapter).GUID, typeof(IAdapter) },
			{ typeof(IDXGISwapChain).GUID, typeof(ISwapChain) },
			{ typeof(IDXGIFactory).GUID, typeof(IFactory) },
			{ typeof(IDXGIFactoryMedia).GUID, typeof(IFactoryMedia) },
			{ typeof(IDXGIKeyedMutex).GUID, typeof(IKeyedMutex) },
			{ typeof(IDXGIResource).GUID, typeof(IResource) },
			{ typeof(IDXGISurface).GUID, typeof(ISurface) },
			{ typeof(IDXGIDecodeSwapChain).GUID, typeof(IDecodeSwapChain) },
		} ) ;

	//! Lookup Proxy Interface by RCW Interface Type:
	internal static ReadOnlyDictionary< Type, Type > _proxyTypesByRCWTypes =
		new( new Dictionary< Type, Type > {
			{ typeof(IDebug), typeof(IDXGIDebug) },
			{ typeof(IDevice), typeof(IDXGIDevice) },
			{ typeof(IOutput), typeof(IDXGIOutput) },
			{ typeof(IOutputDuplication), typeof(IDXGIOutputDuplication) },
			{ typeof(IAdapter), typeof(IDXGIAdapter) },
			{ typeof(ISwapChain), typeof(IDXGISwapChain) },
			{ typeof(IFactory), typeof(IDXGIFactory) },
			{ typeof(IFactoryMedia), typeof(IDXGIFactoryMedia) },
			{ typeof(IKeyedMutex), typeof(IDXGIKeyedMutex) },
			{ typeof(IResource), typeof(IDXGIResource) },
			{ typeof(ISurface), typeof(IDXGISurface) },
			{ typeof(IDecodeSwapChain), typeof(IDXGIDecodeSwapChain) },
		} ) ;
	
	
	static DXGIWrapperFactory( ) {
		
	}
	
	
	/*internal static bool _HasEntryForIID( Guid iid ) {
		_creationFunctionEnumerablesMap.Any( kvp => kvp.Key == _proxyTypesByCOMIID[ iid ] ) ;
	}*/

	internal static bool _HasEntryFor( Type comType ) =>
		_creationFunctionEnumerablesMap.Any( kvp => kvp.Key.IsAssignableFrom( comType ) ) ;
	
	internal static Type _GetWrapperTypeForIID( Guid iid ) => _proxyTypesByCOMIID[ iid ] ;
	
	internal static IEnumerable _getCreationFuncEnumerable( Type type ) => _creationFunctionEnumerablesMap[ type ] ;
	internal static IEnumerable _getCreationFuncEnumerable( Guid iid ) => _creationFunctionEnumerablesMap[ _proxyTypesByCOMIID[iid] ] ;
	internal static ReadOnlyDictionary< Guid, Func< TIn, IInstantiable > > GetFunctions< TIn >( IEnumerable dictionary )
																									where TIn: IUnknown {
		var functionSet = dictionary as ReadOnlyDictionary< Guid, Func< TIn, IInstantiable > > 
						  ?? throw new ArgumentException( ) ;
		return functionSet ;
	}
	
	internal static Delegate GetFunction< TProxy >( Guid iid ) 
		where TProxy: IInstantiable, IComIID => GetFunction( TProxy.Guid ) ;
	
	internal static Delegate GetFunction( Guid iid ) {
		var functionSet = _getCreationFuncEnumerable( iid ) ;
		foreach ( var entry in functionSet ) {
			KeyValuePair<Guid, Delegate> kvp = (KeyValuePair<Guid, Delegate>)entry ;
			if ( kvp.Key == iid ) return kvp.Value ;
		}

		throw new ArgumentException( nameof( iid ) ) ;
	}
} ;