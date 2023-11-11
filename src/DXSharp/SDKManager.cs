#region Using Directives
using System.Text.Json ;
using System.Collections ;
using System.Diagnostics ;
using System.Runtime.InteropServices ;
using System.Text.Json.Serialization ;
using DXSharp.Applications ;

#endregion
namespace DXSharp ;

//! TODO: Make some unit tests and try to get this working ...
// Not yet sure how good of an idea it is, but we will see ...

public static class SDKManager {
	const string _CONFIG_FILE = "sdkconf.json" ;
	
	static nint hD3D12Lib = nint.Zero ;
	static SDKManager( ) {
		string configPath = _CONFIG_FILE ;
		string currentDir = Directory.GetCurrentDirectory( ) ;
		if ( !File.Exists(_CONFIG_FILE) ) {
			string? path = _findSDKLibraries( currentDir ) ;
			if ( path is null ) {
				throw new FileNotFoundException( $"{nameof(SDKManager)} :: " +
												 $"Cannot find the Agility SDK libraries!" ) ;
			}

			if ( NativeLibrary.TryLoad( path, out hD3D12Lib ) ) {
				Debug.WriteLine( $"{nameof(SDKManager)} :: " +
								 $"Loaded DirectX Agility SDK libraries (Path: \"{path}\") ..." ) ;
			}
			else {
				throw new DllNotFoundException( $"{nameof(SDKManager)} :: " +
												  $"Cannot load the Agility SDK libraries!" ) ;
			}
			
			
			//! Register the ApplicationExit event to release module handle:
			Application.ApplicationExit += ( sender, args ) => {
				if ( hD3D12Lib != nint.Zero ) {
					NativeLibrary.Free( hD3D12Lib ) ;
					hD3D12Lib = nint.Zero ;
				} } ;
		}
	}
	
	
	static string? _findSDKLibraries( string currentDir ) {
		string configPath = _findSdkConfigFile( currentDir ) ;
		BindingConfigs[ ] configs = _loadBindingConfigsData( configPath ) ;

		// Find the first config that matches the current architecture:
		var config = configs.FirstOrDefault( cfg =>
												 cfg.packagePath.Contains( RuntimeInformation.ProcessArchitecture.ToString( ) ) ) ??
					 throw new NullReferenceException( "Cannot find config for current CPU architecture!" ) ;
		
		// Set the config file path to the first found config file:
		configPath = Path.Combine( Path.GetDirectoryName(configPath) ?? currentDir, config.fileName ) ;
		
#if DEBUG || DEV_BUILD
		Debug.WriteLine( $"{nameof(SDKManager)} :: " +
						 $"Loaded DXSharp SDK config file for current CPU architecture! (Path: \"{configPath}\")" ) ;
#endif
		
		// Get the SDK path from the config file:
		var sdkPath = config.packagePath ;
		return sdkPath ;
	}
	
	static BindingConfigs[ ] _loadBindingConfigsData( string configPath ) {
		// Load the .json config file and convert it to a .NET object:
		var json = File.ReadAllText( configPath ) ;
		
#if DEBUG || DEV_BUILD
		Debug.WriteLine( $"{nameof(SDKManager)} :: " +
									 $"Loaded DXSharp SDK config file (Path: \"{configPath}\") " +
										$"for {HardwareInfo.ProcessorArchitecture}." ) ;
#endif
		
		var configs = JsonSerializer.Deserialize< ConfigsList >( json )?.Configs ?? 
					  throw new NullReferenceException( "Cannot deserialize the config file." ) ;
		return configs ;
	}
	
	static string _findSdkConfigFile( string currentDir ) {
		//! If we cannot find the config file, we'll try to find it in subfolders:
		var allFolders = Directory.GetDirectories( currentDir ) ;
		if( allFolders is not { Length: > 0 } ) {
			throw new FileNotFoundException( "Cannot find the config file." ) ;
		}
			
		var configFiles = allFolders.Select( folder => Path.Combine( folder, _CONFIG_FILE ) )
									.Where( File.Exists )
									.ToArray( ) ;
		if( configFiles.Length is 0 )
			throw new FileNotFoundException( "Cannot find the config file." ) ;
		
		string configPath = configFiles[ 0 ] ;
#if DEBUG || DEV_BUILD
		if( configFiles.Length > 1 ) {
			Debug.WriteLine( $"{nameof(SDKManager)} :: " +
							 $"Found multiple config files -- using first found! (Path: \"{configPath}\")" ) ;
		}
#endif
		return configPath ;
	}

	public static bool IsLoaded => (hD3D12Lib != nint.Zero) ;
	public static bool UseAgilitySDK { get ; set ; } = true ;
} ;



file class AgilitySDKOptions {
	public const ProcessorArchitecture DefaultArchitecture = ProcessorArchitecture.AMD64 ;
	public static readonly Version InitialVersion = 
		new( 1, 711, 3 ) ;
	
	public string? Path { get ; set ; }
	public Version Version { get ; set ; }
	public bool UseLatest { get ; set ; } = true ;
	public bool IsPreview { get ; set ; } = true ;
	public ProcessorArchitecture Architecture { get ; set ; }
			= DefaultArchitecture ;

	
	public AgilitySDKOptions( ) {
		Path    = ".\\" ;
		Version = InitialVersion ;
	}

	public AgilitySDKOptions( string? name = null,
							  string? version = null,
							  bool useLatest = true,
							  ProcessorArchitecture architecture = 
												DefaultArchitecture ) {
		Path     = name ;
		UseLatest  = useLatest ;
		Architecture = architecture ;
		Version = Version.TryParse( version, out var ver )
											? ver : InitialVersion ;
	}
} ;



file class SDKConfig {
	public string? Name { get ; set ; }
	public string? Path { get ; set ; }
	public string? Version { get ; set ; }
	public bool UseLatest { get ; set ; } = true ;
	public bool IsPreview { get ; set ; } = true ;
	public ProcessorArchitecture Architecture { get ; set ; }
			= AgilitySDKOptions.DefaultArchitecture ;
} ;

public class ConfigsList: IEnumerable< BindingConfigs > {
	[JsonPropertyName("bindingConfigs")]
	public BindingConfigs[ ]? Configs { get ; set ; }
	
	public ConfigsList( ) { }
	public ConfigsList( IEnumerable< BindingConfigs > configs ) => 
			Configs = configs.ToArray( ) ;

	public IEnumerator< BindingConfigs > GetEnumerator( ) {
		if( Configs is null ) yield break ;
		foreach ( var config in Configs )
			yield return config ;
	}
	
	IEnumerator IEnumerable.GetEnumerator( ) => Configs?.GetEnumerator( ) ??
												Enumerable.Empty< BindingConfigs >( ).GetEnumerator( ) ;
} ;

public class BindingConfigs {
	public string packagePath { get ; set ; }
	public string loaderClass { get ; set ; }
	public string packageAlias { get ; set ; }
	public string fileName { get ; set ; }
	public string name { get ; set ; }
	public BindingType bindingType { get ; set ; }
	public string autoload { get ; set ; }
	public string version { get ; set ; }
	public string description { get ; set ; }
	public string remarks { get ; set ; }
} ;

public class BindingType {
	public string moduleType { get ; set ; }
	public string tag { get ; set ; }
} ;

