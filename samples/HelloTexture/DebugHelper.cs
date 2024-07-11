using System.Diagnostics ;
using DXSharp.Direct3D12 ;
using D3D12 = DXSharp.Direct3D12.D3D12 ;
using IDebug6 = DXSharp.Direct3D12.IDebug6 ;
namespace HelloTexture ;


public static class DebugHelper {
	const string LOG_OUTPUT_PATH = @"\logs",
				 LOG_FILE_NAME = $"{nameof(DebugHelper)}-" ;
	
	static readonly Stack< string > InfoLogStack = new( ) ;
	
	public static IDebug6 Init( bool EnableGPUBasedValidation = true, 
							 GPUBasedValidationFlags flags = 
								 GPUBasedValidationFlags.None, 
							 bool EnableAutoName = true ) {
		LogInfo( $"Initializing debug layer ..." ) ;
		var hr = D3D12.GetDebugInterface( out IDebug6? debug6 ) ;
		hr.ThrowOnFailure( ) ;
		ObjectDisposedException.ThrowIf( debug6 is null, typeof( IDebug6 ) ) ;

		//! Enable the debug layer:
		hr = debug6.EnableDebugLayer( ) ;
		LogInfo( $"Initialization Result: {hr} (0x{hr.Value:x}) - " +
				 $"{( hr.Succeeded ? "SUCCEEDED" : "FAILED" )}" ) ;
		hr.ThrowOnFailure( ) ;

		if( EnableAutoName )
			debug6.SetEnableAutoName( EnableAutoName ) ;
		LogInfo( $"AutoName is {( EnableAutoName ? "enabled" : "disabled" )}..." ) ;

		if ( EnableGPUBasedValidation ) {
			debug6.SetGPUBasedValidationFlags( flags ) ;
			debug6.SetEnableGPUBasedValidation( EnableGPUBasedValidation ) ;
		}
		LogInfo( $"GPU-based validation is {( EnableAutoName ? "enabled" : "disabled" )}..." ) ;
		return debug6 ;
	}

	
	public static void LogInfo( params string[] messages ) {
		foreach ( string msg in messages ) {
			LogInfo( msg ) ;
		}
	}
	public static void LogInfo( string message ) {
		ArgumentException.ThrowIfNullOrEmpty( message, nameof( message ) ) ;
		
		Debug.WriteLine( message ) ;
		Console.WriteLine( message ) ;
		InfoLogStack.Push( message ) ;
	}

	/// <summary>
	/// Gets all of the info log text lines as an array of <see cref="string"/>.
	/// </summary>
	/// <returns>All text from the info log stack as a <see cref="string"/> array.</returns>
	public static string[ ] GetInfoLogLines( ) => InfoLogStack.ToArray( ) ;
	
	public static async ValueTask SaveLogFileAsync( ) {
		string[ ] lines = GetInfoLogLines( ) ;
		if ( lines is { Length: < 1 } ) {
			LogInfo( "No log lines to save." ) ;
		}
		string timestamp = DateTime.Now.ToString( "yyyy-MM-dd-HH-mm-ss" ) ;
		string path = Path.Combine( Environment.CurrentDirectory, LOG_OUTPUT_PATH ) ;
		
		if ( !Directory.Exists(path) ) {
			Directory.CreateDirectory( path ) ;
		}
		
		string fileName = $"{LOG_FILE_NAME}{timestamp}.log" ;
		path = Path.Combine( path, fileName ) ;
		await File.WriteAllLinesAsync( path, lines ) ;
	}
} ;