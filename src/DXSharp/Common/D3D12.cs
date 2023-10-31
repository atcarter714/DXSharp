#region Using Directives
using System.Collections.ObjectModel ;
using System.Runtime.Versioning ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using Device = DXSharp.Direct3D12.Device ;
using IDevice = DXSharp.Direct3D12.IDevice ;
#endregion
namespace DXSharp.Direct3D12 ;


public static class D3D12 {
	internal static TWrapper _CreateDevice< A, D, TWrapper >( A? pAdapter, FeatureLevel MinimumFeatureLevel = FeatureLevel.D3D12_0 ) 
																	where A: IDXGIAdapter
																	where D: ID3D12Device
																	where TWrapper: IDevice, IUnknownWrapper< D > {
		PInvoke.D3D12CreateDevice( pAdapter, (D3D_FEATURE_LEVEL)MinimumFeatureLevel, 
								   TWrapper.InterfaceGUID, out var device ) ;
		
		return (TWrapper) TWrapper.Instantiate( (D)device ) ;
	}

	public static HResult GetDebugInterface< TDbg >( out TDbg? _interface ) 
													where TDbg: IUnknownWrapper, IInstantiable {
		var hr = PInvoke.D3D12GetDebugInterface( TDbg.InterfaceGUID, out var pDebug ) ;
		
		if( TDbg.InterfaceGUID == IDebug6.InterfaceGUID )
			_interface = (TDbg) TDbg.Instantiate( (ID3D12Debug6)pDebug ) ;
		else if( TDbg.InterfaceGUID == IDebug5.InterfaceGUID )
			_interface = (TDbg) TDbg.Instantiate( (ID3D12Debug5)pDebug ) ;
		else if( TDbg.InterfaceGUID == IDebug4.InterfaceGUID )
			_interface = (TDbg) TDbg.Instantiate( (ID3D12Debug4)pDebug ) ;
		else if( TDbg.InterfaceGUID == IDebug3.InterfaceGUID )
			_interface = (TDbg) TDbg.Instantiate( (ID3D12Debug3)pDebug ) ;
		else if( TDbg.InterfaceGUID == IDebug2.InterfaceGUID )
			_interface = (TDbg) TDbg.Instantiate( (ID3D12Debug2)pDebug ) ;
		else if( TDbg.InterfaceGUID == IDebug1.InterfaceGUID )
			_interface = (TDbg) TDbg.Instantiate( (ID3D12Debug1)pDebug ) ;
		else if( TDbg.InterfaceGUID == IDebug.InterfaceGUID )
			_interface = (TDbg) TDbg.Instantiate( (ID3D12Debug)pDebug ) ;
		else
			throw new ArgumentException( $"The type parameter " +
										 $"{typeof(TDbg).Name} is not a valid {nameof(IDebug)} interface!" ) ;

		return hr ;
	}
	
	public static HResult EnableExperimentalFeatures( ReadOnlySpan< Guid > iids = default,
													  nint pConfigurations = 0,
													  Span< uint > pStructSizes = default ) {
		unsafe {
			return PInvoke.D3D12EnableExperimentalFeatures( iids, (void *)pConfigurations, pStructSizes ) ;
		}
	}
	
	public static T CreateDevice< T >( IAdapter? adapter = null,
									   FeatureLevel featureLevel = FeatureLevel.D3D12_0 ) 
															where T: IDevice1, IInstantiable {
		var guid = T.InterfaceGUID ;
		var hr = PInvoke.D3D12CreateDevice( adapter?.COMObject ?? null,
											(D3D_FEATURE_LEVEL)featureLevel,
											guid, out var ppDevice ) ;
		hr.ThrowOnFailure( ) ;
		
		return (T) T.Instantiate( (ID3D12Device1)ppDevice ) ;
	}
}

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
	
	
}