using System.Buffers ;
using System.Collections ;
using System.Collections.ObjectModel ;
using System.Diagnostics ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Applications ;
using DXSharp.Direct3D12 ;
using DXSharp.Framework.Common ;
using DXSharp.Framework.XTensions.DXGI ;
using DXSharp.XTensions ;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace DXSharp.Framework.Graphics ;


public class DisplaySetup: IInitialize {
	Dictionary< UID32, IAdapter4 >?      _adaptersByUID ;
	Dictionary< UID32, IOutput6 >?       _outputsByUID ;
	Dictionary< uint,  List<IOutput6> >? _outputsByAdapterOrdinal ;
	Dictionary< UID32, List<IOutput6> >? _outputsByAdapterUID ;
	Dictionary< uint,  List<UID32> >     _outputUIDsByAdapterOrdinal ;
	List< IOutput6 >                     _allOutputs ;
	List< IAdapter4 >?                   _allAdapters ;
	
	Dictionary< UID32, OutputDescription1 >   _outputDescsCache ;
	Dictionary< UID32, AdapterDescription3 > _adapterDescsCache ;
	
	/// <summary>The app that owns the <see cref="DisplaySetup"/> instance.</summary>
	public IDXApp App { get ; }
	/// <summary>Current <see cref="IFactory7"/> instance for the application.</summary>
	public IFactory7 AppFactory { get ; }
	/// <summary>The <see cref="GraphicsSettings"/> for the application.</summary>
	public GraphicsSettings Settings { get ; }
	/// <summary>Indicates if the <see cref="DisplaySetup"/> instance has initialized Direct3D 12 resources.</summary>
	public bool IsInitialized { get ; protected set ; }
	
	
	/// <summary>
	/// Gets or sets the exclusion flags for enumerating display adapters.
	/// </summary>
	public AdapterFlag3 AdapterExcludeFilter { get ; set ; } 
		= AdapterFlag3.Software | AdapterFlag3.Remote ;
	
	/// <summary>
	/// Gets a list of all installed adapters.
	/// </summary>
	/// <exception cref="NullReferenceException">
	/// Thrown if DirectX resources are uninitialized.
	/// </exception>
	public List< IAdapter4 > InstalledAdapters => 
		_allAdapters ?? throw new NullReferenceException( ) ;
	
	
	public DisplaySetup( IDXApp app, IFactory7 appFactory,
						   GraphicsSettings? settings = null ) {
#if DEBUG || DEV_BUILD
		ArgumentNullException.ThrowIfNull ( app, nameof (app) ) ;
		ArgumentNullException.ThrowIfNull( app.Window, nameof (app.Window) ) ;
		ArgumentNullException.ThrowIfNull ( appFactory, nameof (appFactory) ) ;
#endif
		this.App = app ;
		this.AppFactory = appFactory ;
		this.Settings = settings ?? GraphicsSettings.Default ;
		
		AppFactory!.MakeWindowAssociation( app!.Window!.Handle, 
										   WindowAssociation.None ) ;
		
		// Declare support for device removed events?
		if ( Settings.SupportDeviceRemoved ) {
			DXGIFunctions.DeclareAdapterRemovalSupport( ) ;
		}
		
		//! Initialize the display adapter & output lookups:
		_initResetLookups( ) ;
	}


	protected void _initResetLookups( ) {
		_adaptersByUID              ??= new( 2 ) ;
		_outputsByAdapterUID        ??= new( 2 ) ;
		_outputsByAdapterOrdinal    ??= new( 2 ) ;
		_outputsByAdapterUID        ??= new( 2 ) ;
		_outputsByUID               ??= new( 2 ) ;
		_outputUIDsByAdapterOrdinal ??= new( 2 ) ;
		_allOutputs                  ??= new( 2 ) ;

		_allOutputs.Clear( ) ;
		_allAdapters?.Clear( ) ;
		_allAdapters = null ;
		
		_outputDescsCache?.Clear( ) ;
		_adapterDescsCache?.Clear( ) ;

		//! Make sure lookup tables are cleared (in case re-creating resources):
		_adaptersByUID.Clear( ) ;
		_outputsByAdapterUID.Clear( ) ;
		_outputsByAdapterOrdinal.Clear( ) ;
		_outputsByAdapterUID.Clear( ) ;
		_outputsByUID.Clear( ) ;
		_outputUIDsByAdapterOrdinal.Clear( ) ;

		this.IsInitialized = false ;
	}

	
	public virtual void InitializeResources( ) {
		_initResetLookups( ) ;
		
		// Get all installed adapters:
		_allAdapters = AppFactory
			.GetAllInstalledAdapterExcluding< IAdapter4 >( AdapterExcludeFilter ) 
						  ?? throw new ObjectDisposedException( nameof(AppFactory) ) ;
		
#if !STRIP_CHECKS || DEBUG
#if DEBUG || DEV_BUILD
		Debug.Assert( _allAdapters is { Count: > 0 } ) ;
#endif
		if( _allAdapters is null ) {
			throw new FrameworkErrorException( $"{nameof(DisplaySetup)} :: " +
											   $"Failed to get installed adapters." ) ;
		}
		if ( _adaptersByUID is null || _outputsByUID is null ||
			 _outputsByAdapterOrdinal is null || _outputsByAdapterUID is null ||
			 _outputUIDsByAdapterOrdinal is null || _allOutputs is null ) {
			throw new FrameworkErrorException( $"{nameof(DisplaySetup)} :: " +
											   $"Resource lookup tables are not properly initialized." ) ;
		}
#endif
		
		uint adapterOrdinal = 0 ;
		foreach ( var adapter4 in InstalledAdapters ) {
			// Add adapter to lookup table:
			_adaptersByUID.Add( UID32.CreateInstanceIDFrom(adapter4), adapter4 ) ;
			
			// Get all outputs for this adapter:
			var connectedOutputs = adapter4.GetAllOutputs< IOutput6 >( ) ;
			
			// Add outputs to lookup tables:
			_outputsByAdapterUID.Add( UID32.CreateInstanceIDFrom(adapter4), connectedOutputs ) ;
			_outputsByAdapterOrdinal.Add( adapterOrdinal, connectedOutputs ) ;
			
			List< UID32 > outputUIDs = new( 2 ) ;
			// Generate output UIDs and add to lookup table:
			foreach ( var output in connectedOutputs ) {
				var outputUID = UID32.CreateInstanceIDFrom( output ) ;
				_outputsByUID.Add( outputUID, output ) ;
				outputUIDs.Add( outputUID ) ;
				_allOutputs.Add( output ) ;
			}
			
			_outputUIDsByAdapterOrdinal.Add( adapterOrdinal, outputUIDs ) ;
			++adapterOrdinal ;
		}
		
		this.IsInitialized = true ;
	}
	
	public virtual void FreeResources( ) {
		// Release all outputs:
		if( _allOutputs is { Count: > 0 } ) {
			foreach ( var output6 in _allOutputs ) {
				output6?.Release( ) ;
				output6?.Dispose( ) ;
			}
		}
		
		// Release all adapters:
		if( _allAdapters is { Count: > 0 } ) {
			foreach ( var adapter in _allAdapters ) {
				adapter?.Release( ) ;
				adapter?.Dispose( ) ;
			}
			_allAdapters.Clear( ) ;
			_allAdapters = null ;
		}
		
		// Clear all collections:
		_outputsByAdapterUID?.Clear( ) ;
		_outputsByAdapterOrdinal?.Clear( ) ;
		_outputsByUID?.Clear( ) ;
		_adaptersByUID?.Clear( ) ;
		_outputUIDsByAdapterOrdinal?.Clear( ) ;
		_allOutputs?.Clear( ) ;
		
		this.IsInitialized = false ;
	}
	
	public virtual void ReEnumerateOutputDisplaysFor( IAdapter4 adapter4 ) {
		// Make sure lookup tables are initialized:
#if !STRIP_CHECKS || DEBUG || DEV_BUILD
		if( _adaptersByUID is null || _outputsByAdapterUID is null || _outputsByAdapterOrdinal is null ||
			_outputUIDsByAdapterOrdinal is null || _allOutputs is null || _outputsByUID is null ) {
			throw new FrameworkErrorException( $"{nameof(DisplaySetup)} :: " +
											   $"Resource lookup tables are not properly initialized." ) ;
		}
#endif
		
		// Get adapter ordinal:
		uint adapterOrdinal = (uint)_allAdapters!.IndexOf( adapter4 ) ;
		
		// Get adapter UID:
		var adapterUID =
			_adaptersByUID.ElementAt( (int)adapterOrdinal ).Key ;
		
		// Remove old outputs data from lookup tables:
		if( _outputsByAdapterUID.ContainsKey( adapterUID ) ) {
			var outputs = _outputsByAdapterUID[ adapterUID ] ;
			foreach ( var output in outputs ) {
				_allOutputs.Remove( output ) ;
				output?.Release( ) ;
				if( output is DisposableObject { Disposed: false } d ) {
					d.Dispose( ) ;
				}
			}
			outputs.Clear( ) ;
			
			_outputsByAdapterUID.Remove( adapterUID ) ;
		}
		if( _outputsByAdapterOrdinal.ContainsKey( adapterOrdinal ) ) {
			var outputs = _outputsByAdapterOrdinal[ adapterOrdinal ] ;
			foreach ( var output in outputs ) {
				_allOutputs.Remove( output ) ;
				output?.Release( ) ;
				if( output is DisposableObject { Disposed: false } d ) {
					d.Dispose( ) ;
				}
			}
			outputs.Clear( ) ;
			_outputsByAdapterOrdinal.Remove( adapterOrdinal ) ;
		}
		if( _outputUIDsByAdapterOrdinal.ContainsKey( adapterOrdinal ) ) {
			var oldUIDs = _outputUIDsByAdapterOrdinal[ adapterOrdinal ] ;
			oldUIDs.Clear( ) ;
			_outputUIDsByAdapterOrdinal.Remove( adapterOrdinal ) ;
		}
		 
		
		// Get all outputs for this adapter:
		var connectedOutputs = adapter4.GetAllOutputs< IOutput6 >( ) ;
		
		// Add outputs to lookup tables:
		_outputsByAdapterUID.Add( adapterUID, connectedOutputs ) ;
		_outputsByAdapterOrdinal.Add( adapterOrdinal, connectedOutputs ) ;
		
		List< UID32 > outputUIDs = new( 2 ) ;
		// Generate output UIDs and add to lookup table:
		foreach ( var output in connectedOutputs ) {
			var outputUID = UID32.CreateInstanceIDFrom( output ) ;
			_outputsByUID.Add( outputUID, output ) ;
			outputUIDs.Add( outputUID ) ;
			_allOutputs.Add( output ) ;
		}
		
		_outputUIDsByAdapterOrdinal.Add( adapterOrdinal, outputUIDs ) ;
	}
	
	public DisplayOptions GetDisplayOptionsFor( IOutput6 output, bool vrStereo = false ) {
		// Make sure lookup tables are initialized:
#if !STRIP_CHECKS || DEBUG || DEV_BUILD
		if ( _outputsByAdapterUID is null || _outputsByAdapterOrdinal is null ||
			 _outputUIDsByAdapterOrdinal is null || _allOutputs is null || _outputsByUID is null ) {
			throw new FrameworkErrorException( $"{nameof( DisplaySetup )} :: " +
											   $"Resource lookup tables are not properly initialized." ) ;
		}
#endif
		
		_outputDescsCache ??= new( 2 ) ;
		
		UID32 outputUID = _outputsByUID.IndexOfReference( output ) ;
		
		output.GetDesc1( out var desc ) ;
		_outputDescsCache[ outputUID ] = desc ;
		
		output.GetDisplayModeList1( Format.R8G8B8A8_UNORM, EnumModesFlags.None,
									out uint count, out var pDescs ) ;
		
		if( count < 1 ) {
			throw new FrameworkErrorException( $"{nameof(DisplaySetup)} :: " +
											   $"Failed to get display mode list for output." ) ;
		}
		
		HashSet< DisplayMode > allAvailableModes = new( 2 ) ;
		foreach ( var modeDesc1 in pDescs ) {
			// Skip invalid modes:
			if( modeDesc1 is { Width: 0 } or { Height: 0 }
			   or { RefreshRate: { Numerator: 0 } or { Denominator: 0 } } ) continue ;
			if( vrStereo && !modeDesc1.Stereo ) continue ;
			
			DisplayMode mode = modeDesc1 ;
			allAvailableModes.Add( mode ) ;
		}

#if DEBUG || DEV_BUILD
		Debug.Assert( allAvailableModes.Count > 0 ) ;
#endif
		
		return new DisplayOptions( allAvailableModes ) ;
	}
	
	
	public void CheckTearingSupport( ) {
		unsafe {
			//! Check tearing support:
			BOOL allowTearing = false ;
			nint ptr = (nint)(&allowTearing) ;
			uint size = (uint)sizeof( BOOL ) ;
			AppFactory.CheckFeatureSupport( Feature.AllowTearing, ptr, size ) ;
			Settings.TearingSupport = allowTearing ;
		}
	}
	
	public void CheckSupportedD3D12Features( IAdapter adapter, 
											 D3D12Feature[ ] features, 
											    params FeatureLevel[ ]? levels ) {
		ArgumentNullException.ThrowIfNull( adapter, nameof( adapter ) ) ;
		bool[ ] supported = new bool[ features.Length ] ;
		uint counter = 0 ;

		levels ??= DefaultFeatureLevels ;
		var preferredLevel = levels[ 0 ] ;
		foreach ( var feature in features ) {
			unsafe {
				bool supportedFeature = false ;
				IDevice10? device10 = null ;
				HResult hr ;
				try {
					// Try to create device with preferred feature levels:
					foreach( var level in levels ) {
						hr = D3D12.CreateDevice( out var device, adapter,
													 IDevice10.IID, level ) ;
						
						//! If failed to create device, feature is not supported:
						if ( hr.Failed || device is null ) {
							supported[ counter++ ] = supportedFeature = false ;
							continue ;
						}
						device10 = device as IDevice10
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
							   ?? throw new FrameworkErrorException( $"{nameof(DisplaySetup)} :: " +
																	 $"Failed to cast device to {nameof(IDevice10)}." ) 
#endif
							   ;
					}
					
					// Allocate memory for feature struct:
					uint size = FeatureStructSizes[ feature ] ;
					var  featureData = AllocateFeatureStructMemory( feature, out var handle ) ;
					_featureStructHandles.Add( handle ) ;
					
					hr = device10!.CheckFeatureSupport( feature, (nint)handle.Pointer, size ) ;
					if ( hr.Failed ) {
						supported[ counter ] = supportedFeature = false ;
						handle.Dispose( ) ;
						continue ;
					}
					
					// Get feature struct:
					var featureStruct = featureData.Span ;
					
				}
				catch ( Exception e ) {
					supported[ counter ] = supportedFeature = false ;
				}
				finally {
					
				}
			}
			++counter ;
		}
		
	}
	
	static List< (nint Address, uint Size, Type DataType) > _unmanagedAllocations = new( 2 ) ;
	static MemoryPool< byte > _featureDataMemoryPool => MemoryPool< byte >.Shared ;
	static HashSet< MemoryHandle > _featureStructHandles    = new( 2 ) ;
	
	internal static FeatureLevel[ ] DefaultFeatureLevels = {
		FeatureLevel.D3D12_2,
		FeatureLevel.D3D12_1,
		FeatureLevel.D3D12_0,
		FeatureLevel.D3D11_1,
		FeatureLevel.D3D11_0,
	} ;

	internal static D3D12Feature[ ] DefaultFeaturesToCheck = {
		D3D12Feature.FeatureLevels, 
	} ;
	
	// --------------------------------------------------------
	// Feature Support Data Structs:
	// --------------------------------------------------------
	
	/// <summary>Lookup table for feature data struct types by feature flag.</summary>
	public static IReadOnlyDictionary< D3D12Feature, Type > FeatureDataStructTypes 
											=> _featureDataStructTypesByFeatureFlag ;
	static readonly ReadOnlyDictionary< D3D12Feature, Type > _featureDataStructTypesByFeatureFlag = 
		new( new Dictionary< D3D12Feature, Type >( ) {
		{ D3D12Feature.FormatSupport, typeof( FeatureDataFormatSupport ) },
		{ D3D12Feature.MultisampleQualityLevels, typeof( FeatureDataMultisampleQualityLevels ) },
		{ D3D12Feature.FormatInfo, typeof( FeatureDataFormatInfo ) },
		{ D3D12Feature.ShaderModel, typeof( FeatureDataShaderModel ) },
		{ D3D12Feature.ProtectedResourceSessionSupport, typeof( FeatureDataProtectedResourceSessionSupport ) },
		{ D3D12Feature.RootSignature, typeof( FeatureDataRootSignature ) },
		{ D3D12Feature.ShaderCache, typeof( FeatureDataShaderCache ) },
		{ D3D12Feature.CommandQueuePriority, typeof( FeatureDataCommandQueuePriority ) },
		{ D3D12Feature.CrossNode, typeof(FeatureDataCrossNode) },
		{ D3D12Feature.Displayable, typeof(FeatureDataDisplayable ) },
		{ D3D12Feature.FeatureLevels, typeof(FeatureDataFeatureLevels) },
		{ D3D12Feature.Serialization, typeof(FeatureDataSerialization) },
		{ D3D12Feature.ExistingHeaps, typeof(FeatureDataExistingHeaps) },
		{ D3D12Feature.QueryMetaCommand, typeof(FeatureDataQueryMetaCommand) },
		{ D3D12Feature.ProtectedResourceSessionTypes, typeof(FeatureDataProtectedResourceSessionTypes) },
		{ D3D12Feature.ResourceSessionTypeCount, typeof(FeatureDataProtectedResourceSessionTypeCount) },
		{ D3D12Feature.GPUVirtualAddressSupport, typeof(FeatureDataGPUVirtualAddressSupport) },
		{ D3D12Feature.Architecture, typeof(FeatureDataArchitecture) },
		{ D3D12Feature.Architecture1, typeof(FeatureDataArchitecture1) },
		{ D3D12Feature.D3D12Options, typeof(D3D12Options) },
		{ D3D12Feature.D3D12Options1, typeof(D3D12Options1) },
		{ D3D12Feature.D3D12Options2, typeof(D3D12Options2) },
		{ D3D12Feature.D3D12Options3, typeof(D3D12Options2) },
		{ D3D12Feature.D3D12Options4, typeof(D3D12Options4) },
		{ D3D12Feature.D3D12Options5, typeof(D3D12Options5) },
		{ D3D12Feature.D3D12Options6, typeof(D3D12Options6) },
		{ D3D12Feature.D3D12Options7, typeof(D3D12Options7) },
		{ D3D12Feature.D3D12Options8, typeof(D3D12Options8) },
		{ D3D12Feature.D3D12Options9, typeof(D3D12Options9) },
		{ D3D12Feature.D3D12Options10, typeof(D3D12Options10) },
		{ D3D12Feature.D3D12Options11, typeof(D3D12Options11) },
	} ) ;
	
	/// <summary>Lookup table for feature struct sizes by feature flag.</summary>
	public static IReadOnlyDictionary< D3D12Feature, uint > FeatureStructSizes 
											=> _featureStructSizesByFeatureFlag ;

	static readonly unsafe ReadOnlyDictionary< D3D12Feature, uint > _featureStructSizesByFeatureFlag =
		new( new Dictionary< D3D12Feature, uint >( ) {
			{ D3D12Feature.Architecture, (uint)sizeof(FeatureDataArchitecture) },
			{ D3D12Feature.Architecture1, (uint)sizeof(FeatureDataArchitecture1) },
			{ D3D12Feature.CommandQueuePriority, (uint)sizeof(FeatureDataCommandQueuePriority) },
			{ D3D12Feature.CrossNode, (uint)sizeof(FeatureDataCrossNode) },
			{ D3D12Feature.Displayable, (uint)sizeof(FeatureDataDisplayable) },
			{ D3D12Feature.D3D12Options, (uint)sizeof(D3D12Options) },
			{ D3D12Feature.D3D12Options1, (uint)sizeof(D3D12Options1) },
			{ D3D12Feature.D3D12Options2, (uint)sizeof(D3D12Options2) },
			{ D3D12Feature.D3D12Options3, (uint)sizeof(D3D12Options3) },
			{ D3D12Feature.D3D12Options4, (uint)sizeof(D3D12Options4) },
			{ D3D12Feature.D3D12Options5, (uint)sizeof(D3D12Options5) },
			{ D3D12Feature.D3D12Options6, (uint)sizeof(D3D12Options6) },
			{ D3D12Feature.D3D12Options7, (uint)sizeof(D3D12Options7) },
			{ D3D12Feature.D3D12Options8, (uint)sizeof(D3D12Options8) },
			{ D3D12Feature.D3D12Options9, (uint)sizeof(D3D12Options9) },
			{ D3D12Feature.D3D12Options10, (uint)sizeof(D3D12Options10) },
			{ D3D12Feature.D3D12Options11, (uint)sizeof(D3D12Options11) },
			{ D3D12Feature.ExistingHeaps, (uint)sizeof(FeatureDataExistingHeaps) },
			{ D3D12Feature.FeatureLevels, (uint)sizeof(FeatureDataFeatureLevels) },
			{ D3D12Feature.FormatInfo, (uint)sizeof(FeatureDataFormatInfo) },
			{ D3D12Feature.FormatSupport, (uint)sizeof(FeatureDataFormatSupport) },
			{ D3D12Feature.GPUVirtualAddressSupport, (uint)sizeof(FeatureDataGPUVirtualAddressSupport) },
			{ D3D12Feature.MultisampleQualityLevels, (uint)sizeof(FeatureDataMultisampleQualityLevels) },
			{ D3D12Feature.ProtectedResourceSessionSupport, (uint)sizeof(FeatureDataProtectedResourceSessionSupport) },
			{ D3D12Feature.ProtectedResourceSessionTypes, (uint)sizeof(FeatureDataProtectedResourceSessionTypes) },
			{ D3D12Feature.QueryMetaCommand, (uint)sizeof(FeatureDataQueryMetaCommand) },
			{ D3D12Feature.ResourceSessionTypeCount, (uint)sizeof(FeatureDataProtectedResourceSessionTypeCount) },
			{ D3D12Feature.RootSignature, (uint)sizeof(FeatureDataRootSignature) },
			{ D3D12Feature.Serialization, (uint)sizeof(FeatureDataSerialization) },
			{ D3D12Feature.ShaderCache, (uint)sizeof(FeatureDataShaderCache) },
			{ D3D12Feature.ShaderModel, (uint)sizeof(FeatureDataShaderModel) },
		} ) ;

	
	
	public static (nint Address, uint Size, Type DataType) 
								AllocateFeatureStructUnmanagedMemory( D3D12Feature feature ) {
		if( !_featureDataStructTypesByFeatureFlag.TryGetValue( feature, out var type ) ) {
			throw new ArgumentException( $"{nameof(DisplaySetup)} :: Unknown feature flag: {feature}" ) ;
		}
		uint size = _featureStructSizesByFeatureFlag[ feature ] ;
		var ptr = Marshal.AllocHGlobal( (int)size ) ;
		
		var allocInfo = (ptr, size, type) ;
		_unmanagedAllocations.Add( allocInfo ) ;
		return allocInfo ;
	}
	
	public static Memory< byte > AllocateFeatureStructMemory( D3D12Feature feature, out MemoryHandle handle ) {
		if( !_featureDataStructTypesByFeatureFlag.TryGetValue( feature, out var type ) ) {
			throw new ArgumentException( $"{nameof(DisplaySetup)} :: Unknown feature flag: {feature}" ) ;
		}
		
		uint size = _featureStructSizesByFeatureFlag[ feature ] ;
		
		/*Memory< byte > mem  = new ( new byte[ size ] ) ;
		handle = mem.Pin( ) ;*/
		
		var owner = _featureDataMemoryPool.Rent( (int)size ) ;
		handle = owner.Memory.Pin( ) ;
		
		
		return owner.Memory ;
	}
	// =========================================================
} ;