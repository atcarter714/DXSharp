using System.IO ;
using System.Linq ;
using System.Diagnostics ;
using System.Collections ;
using System.Collections.Generic ;


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
	public const string BuildFolderName              = "build",
						OutputFolderName             = "bin",
						SourceFolderName             = "src",
						FilesFolderName              = "file",
						TestsFolderName              = "tests",
						SamplesFolderName            = "samples",
						DocumentationFolderName      = "doc",
						IntermediateOutputFolderName = "obj" ;
	public const string CSharpProjectFileExtensions = ".csproj",
						CPPProjectFileExtensions = ".vcxproj" ;
	public const string CSharpSourceFileExtension = ".cs", CSharpScriptFileExtension = ".csx",
						CPPSourceFileExtension = ".cpp", CSourceFileExtension = ".c",
						HeaderFileExtension = ".h", PreCompiledHeaderFileExtension = ".pch",
						PowershellScriptFileExtension = ".ps1", BatchScriptFileExtension = ".bat",
						HLSLFileExtension = ".hlsl", VertexShaderFileExtension = ".vs",
						PixelShaderFileExtension = ".ps", GeometryShaderFileExtension = ".geo",
						ComputeShaderFileExtension = ".comp", DomainShaderFileExtension = ".domain",
						HullShaderFileExtension = ".hull", DotShaderFileExtension = ".shader",
						ShaderObjectFileExtension = ".obj", ShaderCompiledFileExtension = ".cso" ;

	static readonly string[  ] _allPlatforms = { AnyCPU, x64, x86, ARM, ARM64, } ;
	static readonly char[ ]    _separatorChars = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, } ;
	static readonly string[ ]  _projectFileExtensions = { CSharpProjectFileExtensions, CPPProjectFileExtensions,	} ;
	static readonly string[ ] _mainSolutionFolders = {
		BuildFolderName, OutputFolderName, SourceFolderName,
		FilesFolderName, TestsFolderName, SamplesFolderName,
		DocumentationFolderName, IntermediateOutputFolderName,
	} ;
	
	static FileInfo[ ]? _allProjectsInSolutionCache, _allSDKProjectsCache,
					 _allTestProjectsCache, _allSampleProjectsCache ;
    
	static readonly string? rootSolutionFolder 
								= Resolve_SLN_DIR( ) ;


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

	/// <summary>The directory containing project folders and source code for DXSharp Sample Projects.</summary>
    public static string SamplesFolder => Path.Combine( RootSolutionDir, SamplesFolderName ) ;
	
	/// <summary>The directory containing intermediate build output files for DXSharp.</summary>
    public static string IntermediateOutputFolder => Path.Combine( RootSolutionDir, IntermediateOutputFolderName) ;


	// ----------------------------------------------------
	// Build Directory Paths Navigation:
	// ----------------------------------------------------

	/// <summary>The <i>"\build"</i> directory containing build scripts, files and logs for DXSharp.</summary>
	public static string BuildFolder => Path.Combine( RootSolutionDir, BuildFolderName ) ;

	/// <summary>The <i>"\build\pwsh"</i> directory containing Powershell scripts/tools for DXSharp.</summary>
	public static string BuildPowershellScriptsFolder => Path.Combine( BuildFolder, "pwsh" ) ;
	
	/// <summary>The <i>"\build\logs"</i> directory containing build logs and build tool/script output for DXSharp.</summary>
	public static string BuildLogsFolder => Path.Combine( BuildFolder, "logs" ) ;
	
	/// <summary>The <i>"\build\tools"</i> directory containing reusable tools and scripts for DXSharp.</summary>
	public static string BuildToolsFolder => Path.Combine( BuildFolder, "tools" ) ;

	/// <summary>The <i>"\build\csscript"</i> directory containing reusable Powershell scripts for DXSharp.</summary>
	public static string BuildCSXScriptsFolder => Path.Combine( BuildFolder, "csscript" ) ;
	
	/// <summary>The <i>"\build\msbuild"</i> directory containing reusable Powershell scripts for DXSharp.</summary>
	/// <remarks>These scripts are used to generate the architecture report files.</remarks>
	public static string MSBuildScriptsFolder => Path.Combine( BuildFolder, "msbuild" ) ;
	


    /// <summary>The names of all build platforms for DXSharp.</summary>
    public static string[ ] AllPlatformNames => 
    							_allPlatforms.ToArray( ) ;
    
    /// <summary>The names of all primary solution content folders for DXSharp.</summary>
    public static string[ ] MainSolutionFolders => 
    					_mainSolutionFolders.ToArray( ) ;


	// ----------------------------------------------------
	// Directory Info Get-Only Properties:
	// ----------------------------------------------------

	/// <summary>Directory info for <see cref="RootSolutionDir"/>.</summary>
	public static DirectoryInfo RootSolutionDirInfo => new( RootSolutionDir ) ;
	/// <summary>Directory info for <see cref="RootSolutionBinDir"/>.</summary>
	public static DirectoryInfo RootSolutionBinDirInfo => new( RootSolutionBinDir ) ;
	/// <summary>Directory info for <see cref="IntermediateOutputFolder"/>.</summary>
	public static DirectoryInfo IntermediateOutputDirInfo => new( IntermediateOutputFolder ) ;
	/// <summary>Directory info for <see cref="SourceFolder"/>.</summary>
	public static DirectoryInfo SourceDirInfo => new( SourceFolder ) ;
	/// <summary>Directory info for <see cref="TestsSourceFolder"/>.</summary>
	public static DirectoryInfo TestsDirInfo => new( TestsSourceFolder ) ;
	/// <summary>Directory info for <see cref="SamplesFolder"/>.</summary>
	public static DirectoryInfo SamplesDirInfo => new( SamplesFolder ) ;
	/// <summary>Directory info for <see cref="DocsFolder"/>.</summary>
	public static DirectoryInfo DocsDirInfo => new( DocsFolder ) ;
	/// <summary>Directory info for <see cref="FilesFolder"/>.</summary>
	public static DirectoryInfo FilesDirInfo => new( FilesFolder ) ;
	/// <summary>Directory info for <see cref="BuildFolder"/>.</summary>
	public static DirectoryInfo BuildsDirInfo => new( BuildFolder ) ;


	// ----------------------------------------------------
	// Static Methods & Helper Functions:
	// ----------------------------------------------------
	
	/// <summary>
	/// Indicates if the folder is a "special" folder 
	/// (i.e., hidden, system, reserved, etc.).
	/// </summary>
	/// <param name="folderName">Path or name of the folder</param>
	/// <returns>True if the folder is a "special folder", otherwise false</returns>
	public static bool IsSpecialFolder( string folderName ) => 
									folderName.Contains( "git" )
										|| folderName.StartsWith( "." ) ;

	/// <summary>
	/// Gets an array of all SDK library project files in the solution.
	/// </summary>
	public static FileInfo[ ] GetAllSDKProjects( ) {
		_allSDKProjectsCache ??= SourceDirInfo.EnumerateFiles( "*.csproj", 
															SearchOption.AllDirectories ).ToArray( ) ;
		return _allSDKProjectsCache.ToArray( ) ;
	}

	/// <summary>
	/// Gets an array of all test project files in the solution.
	/// </summary>
	/// <returns>An array of all test project file paths.</returns>
	public static FileInfo[ ] GetAllTestProjects( ) {
		_allTestProjectsCache ??= TestsDirInfo.EnumerateFiles( "*.csproj", 
															SearchOption.AllDirectories ).ToArray( ) ;
		return _allTestProjectsCache.ToArray( ) ;
	}
	/// <summary>
	/// Gets an array of all sample project files in the solution.
	/// </summary>
	/// <returns>An array of all project file paths.</returns>
	public static FileInfo[ ] GetAllSampleProjects( ) {
		_allSampleProjectsCache ??= SamplesDirInfo.EnumerateFiles( "*.*proj",
													SearchOption.AllDirectories ).ToArray( ) ;
		return _allSampleProjectsCache.ToArray( ) ;
	}
	
	/// <summary>
	/// Gets an array of all project files in the solution.
	/// </summary>
	/// <returns>An array of all project file paths in the entire solution.</returns>
	public static FileInfo[ ] GetAllProjectsInSolution( ) {
		_allProjectsInSolutionCache ??= RootSolutionDirInfo.EnumerateFiles( "*.*proj",
													SearchOption.AllDirectories ).ToArray( ) ;
		return _allProjectsInSolutionCache.ToArray( ) ;
	}
	
   // Takes same patterns, and executes in parallel
   public static IEnumerable< FileInfo > GetFiles( string path, 
											   IEnumerable< string > searchPatterns, 
											   SearchOption searchOption = 
											   	SearchOption.TopDirectoryOnly ) {
		var info = new DirectoryInfo( path ) ;
      	return searchPatterns.AsParallel( )
             .SelectMany( searchPattern =>
                    info.EnumerateFiles( searchPattern, searchOption) ) ;
   }
	

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