using Windows.Win32 ;
using DXSharp ;
using DXSharp.Windows ;
using DXSharp.Direct3D12 ;
using DXSharp.Direct3D12.Debugging ;
namespace DXSharp.Framework.Debugging ;


public class DebuggingD3D< T >: IDebugD3D12Support< T >
									       where T: IDebug {
	protected HResult _lastCreateHR ;
	public T Instance { get ; protected set ; }
	public IInstantiable? DebugLayer => Instance ;
	
	public DebuggingD3D( T layer ) => this.Instance = layer ;


	public static IDebugSupport Create< TOut, TLayer >( ) where TOut: IDebugSupport
														  where TLayer: IInstantiable, IComIID {
		var hr = D3D12.GetDebugInterface( out TLayer? ppv ) ;
		hr.ThrowOnFailure( ) ;
		if ( ppv is null )
			throw new DirectXComError( $"Failed to create debug layer for " +
									   $"{nameof( TOut )}!" ) ;
		var debugging = new DebuggingD3D< IDebug >( (IDebug)ppv ) ;
		return debugging ;
	}
} ;

//public DXSharp.DXGI.Debug.IDebug1 DxgiDebugLayer { get ; protected set ; }