#region Using Directives

using Windows.Win32 ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.DXGI.Debugging ;
using IDebug = DXSharp.DXGI.Debugging.IDebug ;
#endregion
namespace DXSharp.Framework.Debugging ;


public class DebuggingDXGI< T >: IDebugDXGISupport< T > 
					  where T: IDebug {
	
	protected HResult _lastCreateHR ;
	public T Instance { get ; protected set ; }
	public IInstantiable? DebugLayer => Instance ;
	
	public DebuggingDXGI( T layer ) => this.Instance = layer ;


	public static IDebugSupport Create< TOut, TLayer >( ) where TOut: IDebugSupport
														  where TLayer: IInstantiable, IComIID {
		var hr = DXGIFunctions.GetDebugInterface1( TLayer.Guid, out var ppv ) ;
		hr.ThrowOnFailure( ) ;
		if ( ppv is null )
			throw new DirectXComError( $"Failed to create debug layer for " +
									   $"{nameof( TOut )}!" ) ;
		var debugging = new DebuggingDXGI< IDebug >( (IDebug)ppv ) ;
		return debugging ;
	}
} ;