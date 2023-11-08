#region Using Directives
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Direct3D12.Debug ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


public static class D3D12 {

	internal static TWrapper _CreateDevice< A, D, TWrapper >( A? pAdapter,
															  FeatureLevel MinimumFeatureLevel = FeatureLevel.D3D12_0 ) 
																where A: IDXGIAdapter
																where D: ID3D12Device
																where TWrapper: IDevice, IUnknownWrapper< D > {
		PInvoke.D3D12CreateDevice( pAdapter, (D3D_FEATURE_LEVEL)MinimumFeatureLevel,
								   TWrapper.Guid, out var device ) ;

		return (TWrapper)TWrapper.Instantiate( (D)device ) ;
	}

	public static HResult GetDebugInterface< TDbg >( out TDbg? _interface )
		where TDbg: IInstantiable, IComIID {
		var hr = PInvoke.D3D12GetDebugInterface( TDbg.Guid, out var pDebug ) ;

		if ( TDbg.Guid == Debug6.Guid )
			_interface = (TDbg)TDbg.Instantiate( (ID3D12Debug6)pDebug ) ;
		else if ( TDbg.Guid == Debug5.Guid )
			_interface = (TDbg)TDbg.Instantiate( (ID3D12Debug5)pDebug ) ;
		else if ( TDbg.Guid == Debug4.Guid )
			_interface = (TDbg)TDbg.Instantiate( (ID3D12Debug4)pDebug ) ;
		else if ( TDbg.Guid == Debug3.Guid )
			_interface = (TDbg)TDbg.Instantiate( (ID3D12Debug3)pDebug ) ;
		else if ( TDbg.Guid == Debug2.Guid )
			_interface = (TDbg)TDbg.Instantiate( (ID3D12Debug2)pDebug ) ;
		else if ( TDbg.Guid == Debug1.Guid )
			_interface = (TDbg)TDbg.Instantiate( (ID3D12Debug1)pDebug ) ;
		else if ( TDbg.Guid == DXSharp.Direct3D12.Debug.Debug.Guid )
			_interface = (TDbg)TDbg.Instantiate( (ID3D12Debug)pDebug ) ;
		else
			throw new ArgumentException( $"The type parameter " +
										 $"{typeof( TDbg ).Name} is not a valid {nameof( IDebug )} interface!" ) ;

		return hr ;
	}

	public static HResult EnableExperimentalFeatures( ReadOnlySpan< Guid > iids            = default,
													  nint                 pConfigurations = 0,
													  Span< uint >         pStructSizes    = default ) {
		unsafe {
			return PInvoke.D3D12EnableExperimentalFeatures( iids, (void*)pConfigurations, pStructSizes ) ;
		}
	}

	public static T CreateDevice< T >( IAdapter? adapter = null,
									   FeatureLevel featureLevel =
										   FeatureLevel.D3D12_0 ) where T: IDevice, IInstantiable {
		var guid = T.Guid ;
		var _adapter = (IComObjectRef< IDXGIAdapter >?)adapter
					   ?? throw new ArgumentNullException( nameof( adapter ) ) ;
		
		var hr = PInvoke.D3D12CreateDevice( _adapter?.ComObject,
											(D3D_FEATURE_LEVEL)featureLevel,
											guid, out var ppDevice ) ;
		hr.ThrowOnFailure( ) ;

		var _rcwObj   = ppDevice as ID3D12Device ?? throw new DirectXComError( $"" );
		var _createFn = Direct3D12.IDevice._resourceCreationFunctions[ guid ] ;
		var _device   = (T)_createFn( _rcwObj ) ;
		
		return _device ;
		
		//return (T)T.Instantiate( (ID3D12Device)ppDevice ) ;
	}
} ;


/*
internal static class WrapperFactory {
	public static ReadOnlyDictionary< Guid, Func< nint, IDXCOMObject> > D3D12CreationFunctions { get ; } =
		new( new Dictionary< Guid, Func< nint, IDXCOMObject > > {
			{ typeof( ID3D12Device ).GUID, p => new Device(p) },
			{ typeof( ID3D12Device1 ).GUID, p => new Device1(p) },
			{ typeof( ID3D12Device2 ).GUID, p => new Device2(p) },
			{ typeof( ID3D12Device3 ).GUID, p => new Device3(p) },
			{ typeof( ID3D12Device4 ).GUID, p => new Device4(p) },
			{ typeof( ID3D12Device5 ).GUID, p => new Device5(p) },
			{ typeof( ID3D12Device6 ).GUID, p => new Device6(p) },
			{ typeof( ID3D12Device7 ).GUID, p => new Device7(p) },
			{ typeof( ID3D12Device8 ).GUID, p => new Device8(p) },
			{ typeof( ID3D12Device9 ).GUID, p => new Device9(p) },
			{ typeof( ID3D12Device10 ).GUID, p => new Device10(p) },
			{ typeof( ID3D12Device11 ).GUID, p => new Device11(p) },
			{ typeof( ID3D12Device12 ).GUID, p => new Device12(p) },
			{ typeof( ID3D12PipelineLibrary ).GUID, p => new PipelineLibrary(p) },
			{ typeof( ID3D12PipelineLibrary1 ).GUID, p => new PipelineLibrary1(p) },
		} ) ;
	
	
}*/