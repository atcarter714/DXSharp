#region MyRegion
using System.Buffers ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using IDevice = DXSharp.Direct3D12.IDevice ;

//using static Windows.Win32.PInvoke ;
#endregion
namespace DXSharp.Framework.Graphics ;


public abstract class DeviceCaps: DisposableObject {
	// ----------------------------------------------------
	internal static readonly ReadOnlyMemory< Type > _allDeviceInterfaceTypes ;
	internal static readonly ReadOnlyMemory< D3D_FEATURE_LEVEL > _allFeatureLevels ;
	
	
	
	static DeviceCaps( ) {
		var all_levels =
			Enum.GetValues< D3D_FEATURE_LEVEL >( ) ;
		Array.Sort( all_levels,
					( a, b ) => (int)b - (int)a ) ;
		_allFeatureLevels = all_levels ;
		
		Type[ ] deviceTypes = {
			typeof( ID3D12Device ),
			typeof( ID3D12Device1 ),
			typeof( ID3D12Device2 ),
			typeof( ID3D12Device3 ),
			typeof( ID3D12Device4 ),
			typeof( ID3D12Device5 ),
			typeof( ID3D12Device6 ),
			typeof( ID3D12Device7 ),
			typeof( ID3D12Device8 ),
			typeof( ID3D12Device9 ),
			typeof( ID3D12Device10 ),
			typeof( ID3D12Device11 ),
			typeof( ID3D12Device12 ),
		} ;
		_allDeviceInterfaceTypes = deviceTypes ;
	}
	
	// ----------------------------------------------------

	public static FeatureLevel GetBestFeatureLevel< TAdapter >( TAdapter adapter )
															where TAdapter: IAdapter {
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
		ArgumentNullException.ThrowIfNull( adapter, nameof(adapter) ) ;
#endif
		
		int len    = _allFeatureLevels.Length ;
		var levels = _allFeatureLevels.Span ;
		var _ppvAdapter = ( (IComObjectRef< IDXGIAdapter >?)adapter )?.ComObject
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
						  ?? throw new ArgumentNullException( nameof( adapter ) )
#endif
			;

		HResult           hr              = default ;
		D3D_FEATURE_LEVEL d3DFeatureLevel = default ;
		
		for ( int i = len - 1; i >= 0; --i ) {
			d3DFeatureLevel = levels[ i ] ;
			hr = PInvoke.D3D12CreateDevice( _ppvAdapter, d3DFeatureLevel,
											TAdapter.Guid, out var device ) ;
			if ( hr.Failed || device is null ) continue ;
			
			//! Feature level is supported ...
			Marshal.FinalReleaseComObject( device ) ;
		}
		
		if ( !hr.Succeeded )
			throw new FrameworkErrorException( $"{hr} Failed to created requested device: ",
											   new NotSupportedException( "No supported feature level found." ) ) ;
		
		return (FeatureLevel)d3DFeatureLevel ;
	}
	
	// ----------------------------------------------------
} ;


public class DeviceCaps< A >: DeviceCaps where A: IAdapter {
	
	// ----------------------------------------------------
	
	
	
	public A AdapterInterface { get ; }
	
	public FeatureLevel FeatureLevel { get ; protected set ; }
	public Type? BestSupportedDeviceInterface { get ; protected set ; }
	public Guid? BestSupportedDeviceInterfaceGuid { get ; protected set ; }
	
	
	// ----------------------------------------------------
	
	public DeviceCaps( A adapter ) {
		ArgumentNullException.ThrowIfNull( adapter, nameof(adapter) ) ;
		this.AdapterInterface = adapter ;
	}
	
	// ----------------------------------------------------

	public void Initialize( ) {
		this.FeatureLevel = GetBestFeatureLevel( this.AdapterInterface ) ;
		this.FindBestSupportedDeviceInterface( ) ;
	}
	public void CheckSupport( D3D12Feature feature ) {
		
	}
	
	public void FindBestSupportedDeviceInterface( ) {
		var featureLevel   = this.FeatureLevel ;
		var adapter      = this.AdapterInterface ;
		
		var _ppvAdapter = ( (IComObjectRef< IDXGIAdapter >?)adapter )?.ComObject 
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
						  ?? throw new ArgumentNullException( nameof( adapter ) )
#endif
		 			;
		
		var types = _allDeviceInterfaceTypes.Span ;
		int len  = types.Length ;
		
		Type? type = null ;
		Guid riid  = Guid.Empty ;
		for( int i = len - 1; i >= 0; --i ) {
			type = types[ i ] ;
			riid = type.GUID ;
			
			unsafe {
				HResult hr = PInvoke.D3D12CreateDevice( _ppvAdapter, (D3D_FEATURE_LEVEL)featureLevel,
															&riid, out var device ) ;
				if ( hr.Failed ) {
					if( hr == HResult.E_NOINTERFACE ) continue ;
					hr.ThrowOnFailure( ) ;
				}
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
				if ( device is null ) {
					throw new FrameworkErrorException( $"{nameof(DeviceCaps<A>)} :: " +
													   $"Unexpected failure at callsite: {nameof( PInvoke.D3D12CreateDevice )}" ) ;
				}
#endif
				
				// Interface is supported ...
				hr = (HResult)Marshal.FinalReleaseComObject( device ) ;
				if( hr.Failed ) hr.ThrowOnFailure( ) ;
				
				this.BestSupportedDeviceInterface = type ;
				this.BestSupportedDeviceInterfaceGuid = riid ;
				break ;
			}
		}

#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
		if( BestSupportedDeviceInterface is null || BestSupportedDeviceInterfaceGuid is null ) {
			throw new FrameworkErrorException( $"{nameof(DeviceCaps<A>)} :: " +
											   $"Unexpected failure - {nameof( PInvoke.D3D12CreateDevice )}" ) ;
		}
#endif
	}
	
	public ID3D12Device CreateBestDeviceRCW( ) {
		var featureLevel = (D3D_FEATURE_LEVEL)this.FeatureLevel ;
		var adapter    = this.AdapterInterface ;
		
		var _ppvAdapter = ( (IComObjectRef< IDXGIAdapter >?)adapter )?.ComObject ;
		
		var guid = this.BestSupportedDeviceInterfaceGuid ?? Guid.Empty ;
		var bestInterface = this.BestSupportedDeviceInterface ?? null ;
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
		if( _ppvAdapter is null )
			throw new ArgumentNullException( nameof(adapter) ) ;
		if( bestInterface is null || guid == Guid.Empty ) {
			throw new FrameworkErrorException( $"{nameof(DeviceCaps<A>)} :: " +
											   $"Best device interface has not been queried." ) ;
		}
#endif

		unsafe {
			HResult hr = PInvoke.D3D12CreateDevice( _ppvAdapter, featureLevel,
													&guid, out var _rcwDevice ) ;
			
			if ( _rcwDevice is not null && hr.Succeeded ) {
				var d3dDevice = (_rcwDevice as ID3D12Device)!
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
								?? throw new FrameworkErrorException( $"{nameof(DeviceCaps<A>)} :: " +
												  $"Unexpected failure to convert device interface RCW!",
													new InvalidCastException($"Failed cast: {_rcwDevice.GetType()} -> {nameof(ID3D12Device)}") )
#endif 
					;

				return d3dDevice ;
			}
			
			throw new FrameworkErrorException( $"Unexpected failure at callsite: " +
											   $"\"{nameof( PInvoke.D3D12CreateDevice )}\" (HR: 0x{hr}).",
											   new NotSupportedException( $"{nameof( DeviceCaps< A > )} :: " +
																		  $"Requested device interface " +
																		  $"(\"{bestInterface.Name}\", IID: {guid}) is not supported." ) ) ;
		}
	}

	public IDevice CreateBestDevice( ) {
		var guid = this.BestSupportedDeviceInterfaceGuid 
				   ?? throw new InvalidOperationException( ) ;
		
		var _rcwDevice  = this.CreateBestDeviceRCW( ) ;
		var _creationFn = (Func<ID3D12Device, IDevice>?)IDevice.GetWrapperCreationFunction( in guid ) ;
		
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
		if( _creationFn is null )
			throw new FrameworkErrorException( $"{nameof(DeviceCaps<A>)} :: " +
											   $"Unexpected failure to get wrapper creation function." ) ;
#endif
		
		var device = _creationFn( _rcwDevice ) ;
#if !STRIP_CHECKS && (DEBUG || DEV_BUILD)
		if( device is null )
			throw new FrameworkErrorException( $"{nameof(DeviceCaps<A>)} :: " +
											   $"Unexpected failure to create wrapper." ) ;
#endif
		return device ;
	}
	
	
	// ----------------------------------------------------
	protected override async ValueTask DisposeUnmanaged( ) {
		
	}
	// =====================================================
} ;