#region Using Directives

using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Direct3D12.Debugging ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;

/// <summary>Implements D3D12* functions from the Windows API.</summary>
public static class D3D12 {
	// -----------------------------------------------------------------------------
	
	#region GUID Helpers
	public static class CLSIDs {
		/// <summary>CLSID_D3D12Debug</summary>
		public static readonly Guid Debug = new( 0xf2352aeb, 0xdd84, 0x49fe, 0xb9, 0x7b, 
												 0xa9, 0xdc, 0xfd, 0xcc, 0x1b, 0x4f ) ;
		
		/// <summary>CLSID_D3D12Tools</summary>
		public static readonly Guid Tools = new( 0xe38216b1, 0x3c8c, 0x4833, 0xaa, 0x09, 
												 0x0a, 0x06, 0xb6, 0x5d, 0x96, 0xc8 ) ;
		
		/// <summary>CLSID_D3D12DeviceRemovedExtendedData</summary>
		public static readonly Guid DeviceRemovedExtendedData = new( 0x4a75bbc4, 0x9ff4, 0x4ad8, 0x9f, 0x18, 
																	 0xab, 0xae, 0x84, 0xdc, 0x5f, 0xf2 ) ;
		
		/// <summary>CLSID_D3D12SDKConfiguration</summary>
		public static readonly Guid SDKConfiguration = new( 0x7cda6aca, 0xa03e, 0x49c8, 0x94, 0x58, 
															0x03, 0x34, 0xd2, 0x0e, 0x07, 0xce ) ;
	}
	public static class ExperimentalFeatures {
		/// <summary>D3D12ExperimentalShaderModels</summary>
		public static readonly Guid ShaderModels = new( "76f5573e-f13a-40f5-b297-81ce9e18933f" ) ;
		/// <summary>D3D12TiledResourceTier4</summary>
		public static readonly Guid TiledResourceTier4 = new( "c9c4725f-a81a-4f56-8c5b-c51039d694fb" ) ;
		/// <summary>D3D12MetaCommand</summary>
		public static readonly Guid MetaCommands = new( "C734C97E-8077-48C8-9FDC-D9D1DD31DD77" ) ;
	}
	#endregion
	
	
	// -----------------------------------------------------------------------------
	// D3D12 Functions:
	// -----------------------------------------------------------------------------
	
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
		else if ( TDbg.Guid == DXSharp.Direct3D12.Debugging.Debug.Guid )
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
			return PInvoke.D3D12EnableExperimentalFeatures( iids, (void *)pConfigurations, pStructSizes ) ;
		}
	}

	public static T? CreateDevice< T >( IAdapter? adapter = null,
										FeatureLevel featureLevel =
											FeatureLevel.D3D12_0 ) where T: IDevice, IInstantiable {
		var guid = T.Guid ;
		var _adapter = (IComObjectRef< IDXGIAdapter >?)adapter
					   ?? throw new ArgumentNullException( nameof( adapter ) ) ;
		
		HResult hr = PInvoke.D3D12CreateDevice( _adapter?.ComObject,
											(D3D_FEATURE_LEVEL)featureLevel,
											guid, out var ppDevice ) ;
		hr.SetAsLastErrorForThread( ) ;
#if DEBUG_COM
		hr.ThrowOnFailure( ) ;
#endif
		
		var _rcwObj = ( ppDevice as ID3D12Device
#if (DEBUG || DEBUG_COM || DEV_BUILD)
					  ?? throw new DirectXComError( $"{nameof(PInvoke.D3D12CreateDevice)} failed!" ) )
#else
					  )!
#endif
						;
		
		var _createFn = Direct3D12.IDevice._deviceCreationFunctions[ guid ] ;
		var _device   = (T)_createFn( _rcwObj ) ;
		return _device ;
	}

	public static HResult CreateDevice( out IDevice? device, 
										IAdapter? adapter = null, 
										in Guid riid = default,
										FeatureLevel featureLevel = FeatureLevel.D3D12_0 ) {
		var _adapter = (IComObjectRef< IDXGIAdapter >?)adapter
					   ?? throw new ArgumentNullException( nameof( adapter ) ) ;
		
		HResult hr = PInvoke.D3D12CreateDevice( _adapter?.ComObject,
												(D3D_FEATURE_LEVEL)featureLevel,
												riid, out var ppDevice ) ;
		hr.SetAsLastErrorForThread( ) ;
		if( ppDevice is null ) {
			device = null ;
			return hr ;
		}
		
		var _rcwObj   = ( ppDevice as ID3D12Device ) ;
		if( _rcwObj is null ) {
			Marshal.FinalReleaseComObject( ppDevice ) ;
			throw new DirectXComError( $"{nameof(PInvoke.D3D12CreateDevice)} failed!" ) ;
		}
		
		var createFunctions = IDevice._deviceCreationFunctions ;
		if( !createFunctions.ContainsKey( riid ) ) {
			Marshal.FinalReleaseComObject( ppDevice ) ;
			device = null ;
			return hr ;
		}
		
		var _createFn = createFunctions[ riid ] ;
		device   = (IDevice)_createFn( _rcwObj ) ;
		
		return hr ;
	}
	
	public static HResult SerializeVersionedRootSignature( in  VersionedRootSignatureDescription desc,
														   out (IBlob? blob, IBlob? errors)      blobs ) {
		IBlob? _blob = null, _errors = null ;
		var hr = SerializeVersionedRootSignature( desc, out _blob, out _errors ) ;
		blobs = ( _blob, _errors ) ;
		return hr ;
	}
	
	
	public static HResult SerializeVersionedRootSignature( in VersionedRootSignatureDescription desc,
																out IBlob? blob, out IBlob? errorBlob ) {
		var _desc = desc ;
		unsafe {
			fixed ( VersionedRootSignatureDescription* pDesc = &desc ) {
				HResult hr = PInvoke.D3D12SerializeVersionedRootSignature( (D3D12_VERSIONED_ROOT_SIGNATURE_DESC*)pDesc,
																	   out var _b, out var _e ) ;
				hr.SetAsLastErrorForThread( ) ;
				if( hr.Failed || (_b is null && _e is null) ) {
					blob = null ; errorBlob = null ;
					return hr ;
				}
				
				errorBlob = _e is not null 
								? new Blob( _e ) : null ;
				blob = _b is not null 
						   ? new Blob( _b ) : null ;
				
				return hr ;
			}
		}
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

/*internal static TWrapper _CreateDevice< A, D, TWrapper >( A? pAdapter,
															  FeatureLevel MinimumFeatureLevel = FeatureLevel.D3D12_0 )
																where A: IDXGIAdapter
																where D: ID3D12Device
																where TWrapper: IDevice, IUnknownWrapper< D > {
		PInvoke.D3D12CreateDevice( pAdapter, (D3D_FEATURE_LEVEL)MinimumFeatureLevel,
								   TWrapper.Guid, out var device ) ;

		return (TWrapper)TWrapper.Instantiate( (D)device ) ;
	}*/