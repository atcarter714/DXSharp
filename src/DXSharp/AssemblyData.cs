using System.Runtime.Versioning ;
[assembly: SupportedOSPlatform("windows")]
namespace DXSharp ;



/// <summary>Assembly data for DXSharp.</summary>
public static class AssemblyData {
	/// <summary>Display name of the assembly.</summary>
	public const string Name = "DXSharp" ;
	
	/// <summary>Platforms the assembly can target.</summary>
	public static readonly PlatformID[ ] Platforms
		= { PlatformID.Win32NT, PlatformID.Xbox, } ;
	
	/// <summary>Names/versions of frameworks the assembly can target.</summary>
	public static readonly FrameworkName[ ] Frameworks
		= { new FrameworkName( ".NET", new Version( 7, 0, 0 ) ), } ;
} ;