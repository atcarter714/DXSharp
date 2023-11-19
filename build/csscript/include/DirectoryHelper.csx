using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics ;


/// <summary>
/// CSX (C# Script) files can import this static helper class to have access
/// to the root solution directory and important paths of subfolders and items.
/// </summary>
/// <remarks>Use the <code>#load "path-to-script.csx"</code> directive.</remarks>
public static class DirectoryHelper {
	// ----------------------------------------------------
	// Constants & ReadOnly Values:
	// ----------------------------------------------------
	public const string SolutionName = "DXSharp" ;
	public const string AnyCPU = "AnyCPU",
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
	
    
	static readonly string? rootSolutionFolder = Resolve_SLN_DIR( ) ;


	/// <summary>The root directory of the solution.</summary>
	public static string RootSolutionDir => rootSolutionFolder 
    ?? throw new DirectoryNotFoundException( "Could not find the root solution directory." ) ;
	
	/// <summary>The directory of the main project's build output.</summary>
	public static string RootSolutionBinDir => Path.Combine( RootSolutionDir, OutputFolderName ) ;
	
    
	
	/// <summary>The directory of for test projects' build outputs (i.e., <i>"bin\Tests\"</i></summary>
	/// <remarks>Contains subfolders of builds for each platform and configuration combination.</remarks>
	public static string TestProjectMainOutputDir =>
            Path.Combine( RootSolutionBinDir, "Tests" ) ;
	
    
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


    /// <summary>The names of all build platforms for DXSharp.</summary>
    public static string[ ] AllPlatformNames => 
    _allPlatforms.ToArray( ) ;
    
    /// <summary>The names of all primary solution content folders for DXSharp.</summary>
    public static string[ ] MainSolutionFolders => 
    _mainSolutionFolders.ToArray( ) ;


	// ----------------------------------------------------
	// Static Constructor & Initialization:
	// ----------------------------------------------------
	
	public static string? Resolve_SLN_DIR( ) {
        // This can be written much better but I had to do it 
        // with no debugging or error messages the first time lol ...
		var currentDir = Environment.CurrentDirectory ;
		var drive	   = Path.GetPathRoot( currentDir ) ;
		var folders    = currentDir.TrimEnd( _separatorChars )
										.Split( _separatorChars )
										.SkipWhile( s => s.Contains(Path.VolumeSeparatorChar) ) ;

		int index = 0, nFolders = folders.Count( ) ;
		foreach ( var folder in folders ) {
            bool filterCondition = folder is SolutionName 
                                    || folder.Contains( SolutionName ) ;
            
			if( filterCondition ) {
				var    _candidateFolders = folders.Take( index + 1 )
													.ToArray(  ) ;
                var endingFolders = folders.Skip( index + 1 )
                                            .ToArray( ) ;
                string _endOfPath = Path.Combine( endingFolders ) ;
				string _candidate        = currentDir.Remove( currentDir.Length - _endOfPath.Length ) ;
                
                
				if( !Directory.Exists( _candidate )  )
                    throw new DirectoryNotFoundException( $"Directory \"{_candidate}\" does not exist." + 
                                                            "Unexpected failure in script." ) ;
				
				
				// List the contents of the candidate directory:
				var    _contents         = Directory.EnumerateDirectories( _candidate ) ;
				int    _count            = _contents.Count( ) ;
                
				int slnFolderCount = 0 ;
				foreach ( var item in _contents ) {
					string name = item.Remove( 0, _candidate.Length )
                                       .Trim( _separatorChars ) ;
					
					if( _mainSolutionFolders.Count( s => s.Contains(name) ) > 0 ) {
						++slnFolderCount ;
					}
				}


                //! Are all the expected "main" folders there?
				if( slnFolderCount < _mainSolutionFolders.Length ) {
                    continue ;
				}
				
                // Construct path where solution should live:
				string potentialSlnFile = Path.Combine( _candidate, 
														$"{SolutionName}.sln" ) ;
				
                //! Does the DXSharp solution file exist here?
                bool containsSolution = File.Exists( potentialSlnFile ) ;
                
				if( containsSolution ) {
                    return _candidate ;
				}
			}

			++index ;
		}
		return null ;
	}
	
	// ====================================================
} ;