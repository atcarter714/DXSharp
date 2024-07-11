using System.Diagnostics ;
namespace BasicTests ;

[TestFixture]
public class Validate_Directory_Helper {
	[Test] public void Paths_Are_Correct( ) {
		Assert.That( DirectoryHelper.RootSolutionDir.EndsWith( "DXSharp" ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.RootSolutionDir ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.RootSolutionBinDir ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.TestingMainBinDir ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.TestProjectMainOutputDir ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.TestProjectPlatformConfigBinDir ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.TestProjectConfigurationBinDir ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.SourceFolder ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.DocsFolder ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.FilesFolder ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.TestsSourceFolder ) ) ;
		Assert.That( Directory.Exists( DirectoryHelper.IntermediateOutputFolder ) ) ;
	}
}


public static class DirectoryHelper {
	// ----------------------------------------------------
	// Constants & ReadOnly Values:
	// ----------------------------------------------------
	const string SolutionName = "DXSharp" ;
	const string AnyCPU = "AnyCPU",
				 x64    = "x64", x86   = "x86",
				 ARM    = "arm", ARM64 = "arm64" ;
	static readonly string[  ] _allPlatforms = {
		AnyCPU, x64, x86, ARM, ARM64,
	} ;
	static readonly char[ ] _separatorChars = {
		Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar,
	} ;

	public const string BuildFolderName              = "build",
						OutputFolderName             = "bin",
						SourceFolderName             = "src",
						FilesFolderName              = "files",
						TestsFolderName              = "tests",
						SamplesFolderName            = "samples",
						DocumentationFolderName      = "doc",
						IntermediateOutputFolderName = "obj" ;
	static readonly string[ ] _mainSolutionFolders = {
		BuildFolderName, OutputFolderName, SourceFolderName,
		FilesFolderName, TestsFolderName, SamplesFolderName,
		DocumentationFolderName, IntermediateOutputFolderName,
	} ;
	
	// ----------------------------------------------------
	//! Relative Directory Levels:
	// ----------------------------------------------------
	const string outputDirConfig    = @"..\..\" ; // The last subfolder of a test build is the config name
	const string outputDirPlatform  = @"..\..\..\" ; // The platform name is one level up from the config name
	const string outputDirProject   = @"..\..\..\..\" ; // The project name is one level up from the platform name
	const string outputDirTesting   = @"..\..\..\..\..\" ; // The builds for all tests live in "\bin\Tests" ...
	const string outputDirMainBin   = @"..\..\..\..\..\..\" ; // ... inside the main project's "\bin" folder ...
	const string rootSolutionFolder = @"..\..\..\..\..\..\" ; // ... which is in the root solution folder.
	// ----------------------------------------------------------------------------------------------------------
	// Example Path:
	// C:\Users\<username>\source\repos\DXSharp\bin\Tests\BasicTests\AnyCPU\Debug\net7.0-windows10.X.XXXXX.X
	// ----------------------------------------------------------------------------------------------------------
	
	/// <summary>The root directory of the solution.</summary>
	public static readonly string RootSolutionDir =
			Path.GetFullPath( rootSolutionFolder ).TrimEnd(_separatorChars) ;
	
	/// <summary>The directory of the main project's build output.</summary>
	public static readonly string RootSolutionBinDir =
			Path.GetFullPath( outputDirMainBin ).TrimEnd(_separatorChars) ;
	
	/// <summary>The directory of the test project's build output.</summary>
	/// <remarks>Contains subfolders for each unit testing project's build outputs.</remarks>
	public static readonly string TestingMainBinDir =
			Path.GetFullPath( outputDirTesting ).TrimEnd(_separatorChars) ;
	
	/// <summary>The directory of the test project's build outputs inside the <i>"bin\Tests\"</i></summary>
	/// <remarks>Contains subfolders of builds for each platform and configuration combination.</remarks>
	public static readonly string TestProjectMainOutputDir =
			Path.GetFullPath( outputDirProject ).TrimEnd(_separatorChars) ;
	
	/// <summary>The directory of the test project's build output for the current platform.</summary>
	/// <remarks>Contains subfolders of builds for each configuration.</remarks>
	public static readonly string TestProjectPlatformConfigBinDir =
			Path.GetFullPath( outputDirPlatform ).TrimEnd(_separatorChars) ;

	/// <summary>
	/// The directory of the test project's build output for the
	/// current build configuration. This is the directory that
	/// contains the test's executable(s) and any other files and
	/// assets that were output from the build.
	/// </summary>
	/// <remarks>
	/// This is often <b><c>Debug</c></b> or <b><c>Release</c></b>,
	/// but can be any custom configuration.
	/// </remarks>
	public static readonly string TestProjectConfigurationBinDir =
		Path.GetFullPath( outputDirConfig ).TrimEnd(_separatorChars) ;

	
	/// <summary>The directory the main SDK library project folders and source are found in for DXSharp.</summary>
	public static string SourceFolder => Path.Combine( RootSolutionDir, SourceFolderName ) ;

	/// <summary>The directory containing documents and documentation for DXSharp.</summary>
	public static string DocsFolder => Path.Combine( RootSolutionDir, DocumentationFolderName ) ;

	/// <summary>The directory containing global files and content for DXSharp.</summary>
	public static string FilesFolder => Path.Combine( RootSolutionDir, DocumentationFolderName ) ;

	/// <summary>The directory containing project folders and source code for DXSharp Unit Testing.</summary>
	public static string TestsSourceFolder => Path.Combine( RootSolutionDir, TestsFolderName ) ;
	
	/// <summary>The directory containing intermediate build output files for DXSharp.</summary>
	public static string IntermediateOutputFolder => Path.Combine( RootSolutionDir, IntermediateOutputFolderName) ;

	
	
	// ----------------------------------------------------
	// Static Constructor & Initialization:
	// ----------------------------------------------------
	
	static string? Resolve_SLN_DIR( ) {
		var currentDir = Path.GetFullPath( Directory.GetCurrentDirectory( ) ) ;
		var drive	   = Path.GetPathRoot( currentDir ) ;
		var folders    = currentDir.TrimEnd( _separatorChars )
										.Split( _separatorChars )
										.SkipWhile( s => s.Contains(Path.VolumeSeparatorChar) ) ;

		int index = 0, nFolders = folders.Count( ) ;
		foreach ( var folder in folders ) {
			Debug.WriteLine( folder ) ;
			if( folder is SolutionName || folder.Contains(SolutionName) ) {
				Debug.WriteLine( $"Potential Solution Directory: \"{folder}\"" ) ;
				var    _candidateFolders = folders.SkipLast( nFolders - index - 1 )
													.ToArray(  ) ;
				string _candidate        = Path.Combine( drive,
														 Path.Combine(_candidateFolders) ) ;
				var    _contents         = Directory.EnumerateDirectories( _candidate ) ;
				int    _count            = _contents.Count( ) ;
				
				//! Check that the candidate directory really exists:
				Assert.That( Directory.Exists( _candidate ),
							 $"The candidate directory \"{_candidate}\" does not exist." ) ;
				
				string relativePath = Path.GetRelativePath( currentDir, _candidate ) ;
				Debug.WriteLine( $"Candidate contains {_count} folders: " ) ;
				Debug.WriteLine( $"Relative path:  \"{relativePath}\"" ) ;
				Debug.WriteLine( $"Candidate path: \"{_candidate}\"" ) ;
				
				// List the contents of the candidate directory:
				int slnFolderCount = 0 ;
				Debug.WriteLine( "\nList of contents: ... " ) ;
				foreach ( var item in _contents ) {
					string name = Path.GetDirectoryName( item ) ;
					Debug.Write( $"\"{name}\"" ) ;
					
					if( _mainSolutionFolders.Contains( name ) ) {
						Debug.Write( " (main solution folder)" ) ;
						++slnFolderCount ;
					}
				}
				if( slnFolderCount == _mainSolutionFolders.Length ) {
					Debug.WriteLine( "\n\nCandidate directory contains all main solution folders." ) ;
				}
				else {
					Debug.WriteLine( "\n\nCandidate directory does not contain all main solution folders." ) ;
				}
				
				string potentialSlnFile = Path.Combine( _candidate, 
														$"{SolutionName}.sln" ) ;
				bool containsSolution = File.Exists( potentialSlnFile ) ;
				if( containsSolution ) {
					return _candidate ;
					Debug.WriteLine( $"Candidate directory contains the solution file: \"{potentialSlnFile}\"" ) ;
				}
				else {
					Debug.WriteLine( $"Candidate directory does not contain the solution file: \"{potentialSlnFile}\"" ) ;
				}
				break ;
			}
			++index ;
		}

		return null ;
	}
	
	// ====================================================
} ;