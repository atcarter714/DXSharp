#region Using Directives
using System.Text.Json ;
using System.Collections ;
using System.Diagnostics ;
using System.Reflection ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;
using System.Text.Json.Serialization ;
using DXSharp.Applications ;

#endregion
namespace DXSharp ;


[SupportedOSPlatform("windows6.0.6000")]
public static class SDKManager {
	const string _D3D12CORE_NAME      = "d3d12core.dll",
				 _D3D12SDKLAYERS_NAME = "d3d12SDKLayers.dll",
				 _NATIVE_DIR_NAME_0   = "native",
				 _NATIVE_DIR_NAME_1   = "bin" ;
	
	const string _SDK_REL_PATH = _NATIVE_DIR_NAME_0 +
								 "\\" + _NATIVE_DIR_NAME_1 ;
	
	const string _X64_PLATFORM_NAME   = "x64",
				 _X86_PLATFORM_NAME   = "win32",
				 _ARM_PLATFORM_NAME   = "arm",
				 _ARM64_PLATFORM_NAME = "arm64" ;
	
	const string _d3d12CoreSystemPath      = @"C:\Windows\System32\D3D12Core.dll" ;
	const string _d3d12SDKLayersSystemPath = @"C:\Windows\System32\d3d12SDKLayers.dll" ;
	
	static string _platformName ;
	static nint _d3d12coreHandle = nint.Zero ;
	static string _d3d12corePath = string.Empty ;
	
	static nint _d3d12SDKLayersHandle = nint.Zero ;
	static string _d3d12SDKLayersPath = string.Empty ;
	public static bool UsingAgilitySDK { get ; private set ; }
	
	static SDKManager( ) {
		_platformName = _getFolderNameForCPUArchitecture( ) ;

		// ---------------------------------------------------------
		// Load the native DLLs:
		// ---------------------------------------------------------
		//LoadNativeDLLs( ) ;
		// ---------------------------------------------------------
	}


	[ModuleInitializer]
	internal static void LoadNativeDLLs( ) {
		string sdkSearchPath = Environment.CurrentDirectory ;
		string d3d12corePath = Path.Combine( sdkSearchPath, _D3D12CORE_NAME ) ;
		string d3d12SDKLayersPath = Path.Combine( sdkSearchPath, _D3D12SDKLAYERS_NAME ) ;
		
		bool d3d12coreExists = File.Exists( d3d12corePath ) ;
		bool d3d12SDKLayersExists = File.Exists( d3d12SDKLayersPath ) ;

		if ( d3d12coreExists ) {
			_d3d12corePath   = d3d12corePath ;
			_d3d12coreHandle = NativeLibrary.Load( _d3d12corePath ) ;
		}
		else { _d3d12corePath   = _d3d12CoreSystemPath ; }
		
		if ( d3d12SDKLayersExists ) {
			_d3d12SDKLayersPath   = d3d12SDKLayersPath ;
			_d3d12SDKLayersHandle = NativeLibrary.Load( _d3d12SDKLayersPath ) ;
		}
		else { _d3d12SDKLayersPath = _d3d12SDKLayersSystemPath ; }
	}
	
	public static bool ReleaseNativeDLLs( ) {
		if ( _d3d12coreHandle != nint.Zero ) {
			NativeLibrary.Free( _d3d12coreHandle ) ;
			_d3d12coreHandle = nint.Zero ;
		}
		
		if ( _d3d12SDKLayersHandle != nint.Zero ) {
			NativeLibrary.Free( _d3d12SDKLayersHandle ) ;
			_d3d12SDKLayersHandle = nint.Zero ;
		}
		
		return true ;
	}
	
	
	/*internal static void LoadNativeDLLs( ) {
		string sdkSearchPath = Path.Combine( Environment.CurrentDirectory,
										   _SDK_REL_PATH, _platformName ) ;
		string d3d12corePath = Path.Combine( sdkSearchPath, _D3D12CORE_NAME ) ;
		string d3d12SDKLayersPath = Path.Combine( sdkSearchPath, _D3D12SDKLAYERS_NAME ) ;
		
		bool d3d12coreExists = File.Exists( d3d12corePath ) ;
		bool d3d12SDKLayersExists = File.Exists( d3d12SDKLayersPath ) ;
		
		if ( !d3d12coreExists ) {
#if DEBUG || DEV_BUILD
			Debug.WriteLine( "DXSharp: No Agility SDK libraries found. " +
							 "Reverting to built-in Windows SDK." ) ;
#endif
			
			UsingAgilitySDK = false ;
			string sysDir = Environment.SystemDirectory ;
			_d3d12corePath  = Path.Combine( sysDir, _D3D12CORE_NAME ) ;
			_d3d12SDKLayersPath = Path.Combine( sysDir, _D3D12SDKLAYERS_NAME ) ;
			return ;
		}
		
		UsingAgilitySDK = true ;
		_d3d12corePath = d3d12corePath ;
		_d3d12SDKLayersPath = d3d12SDKLayersPath ;
		
		_d3d12coreHandle = NativeLibrary.Load( _d3d12corePath ) ;
		
#if DEBUG || DEV_BUILD
		if( d3d12SDKLayersExists )
			_d3d12SDKLayersHandle = NativeLibrary.Load( _d3d12SDKLayersPath ) ;
		else
			Debug.WriteLine( "DXSharp: No D3D12 SDK Layers DLL found. " +
							 "Skipping load." ) ;
#endif
	}*/
	
	
	[SupportedOSPlatform("windows6.0.6000")]
	static string _getFolderNameForCPUArchitecture( ) =>
		HardwareInfo.ProcessorArchitecture switch {
			ProcessorArchitecture.AMD64 => _X64_PLATFORM_NAME,
			ProcessorArchitecture.Intel => _X86_PLATFORM_NAME,
			ProcessorArchitecture.ARM64 => _ARM64_PLATFORM_NAME,
			ProcessorArchitecture.ARM   => _ARM_PLATFORM_NAME,
			_                           => throw new PlatformNotSupportedException( )
		} ;
} ;