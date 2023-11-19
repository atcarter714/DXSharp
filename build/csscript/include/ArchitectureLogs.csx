#load ".\DirectoryHelper.csx"
#load ".\LogHelpers.csx"
using System ;


/// <summary>
/// Helper script for reading and writing architecture report files.
/// </summary>
/// <remarks>
/// Use the <code>#load "path-to-script.csx"</code> directive.<para/>
/// <para>Contains defined <see cref="string"/> constants for the 
/// header tags used in the architecture report files.</para>
/// <para>Contains other utilities for architecture report files.</para>
/// </remarks>
public static class ArchitectureLogs {
	// ----------------------------------------------------
	// Declare header tag strings:
	// ----------------------------------------------------
	public const string ARCHITECTURE_HEADER = "[Architecture]",
				SLN_DIR_CONTENT_HEADER = "[SolutionDirContents]",
				SLN_FILE_HEADER        = "[SlnFiles]",
				YAML_FILES_HEADER      = "[YamlFiles]",
				MSBUILD_FILES_HEADER   = "[MSBuildFiles]",
				MARKDOWN_FILES_HEADER  = "[MarkdownFiles]",
				VERSION_CONTROL_FILES_HEADER = "[VCSFiles]",
				TEXT_FILES_HEADER      = "[TextFiles]",
				ASSETS_FILES_HEADER    = "[Assets]" ;
				
	public const string SDK_LIBRARY_HEADER = "[SDKLibraries]",
				PROJECT_FILES_HEADER   = "[Projects]",
				SOLUTION_FILES_HEADER  = "[Solutions]",
				TEST_FILES_HEADER      = "[Tests]" ;
				
	public const string FOLDER_HEADER  = "[Folder]",
				DIR_HEADER             = "[Directories]",
				FILES_CONTENT_HEADER   = "[Files]",
				CS_FILES_HEADER        = "[CSFiles]",
				CSX_FILES_HEADER       = "[CSXScripts]",
				SHADER_FILES_HEADER    = "[HLSL]",
				SHADER_INCLUDE_HEADER  = "[HLSLIncludes]",
				SHADER_LIB_HEADER      = "[HLSLLib]",
				ADDITIONAL_FILES_HEADER = "[AdditionalFiles]" ;
				
	// ----------------------------------------------------

	public static void CreateContentsReport( ref string reportText, string directoryFullPath ) {
		 var rootDir = DirectoryHelper.RootSolutionDir ;
		 DirectoryInfo dirInfo = new( directoryFullPath ) ;
		
		Logger.Display( $"Next folder: {directoryFullPath} " ) ;
		if( !dirInfo.Exists )
			throw new DirectoryNotFoundException( $"Could not find directory: {dirInfo.FullName}" ) ;
		
		var foldersInDir =
			dirInfo.EnumerateDirectories( "*", SearchOption.TopDirectoryOnly ) ;
		
		var filesInDir = 
			dirInfo.EnumerateFiles( "*", SearchOption.TopDirectoryOnly ) ;
		
		--Logger.IndentLevel ;
		Logger.LogLine( ref reportText, "---" ) ;
		Logger.LogLine( ref reportText, FOLDER_HEADER ) ;
		Logger.LogLine( ref reportText, $"//! {dirInfo.Name}: \"{dirInfo.FullName}\"\n" ) ;
		++Logger.IndentLevel ;

		Logger.LogLine( ref reportText, DIR_HEADER ) ;
		Logger.LogLine( ref reportText, $"//! Folders in \\{dirInfo.Name} directory: \"{dirInfo.FullName}\"" ) ;
		Logger.LogLines( ref reportText, foldersInDir.Select( f => @"\" + f.Name ) ) ;
		Logger.SkipLine( ref reportText ) ;

		Logger.LogLine( ref reportText, FILES_CONTENT_HEADER ) ;
		Logger.LogLine( ref reportText, $"//! Files in \\{dirInfo.Name} directory: \"{dirInfo.FullName}\"" ) ;
		Logger.LogLines( ref reportText, filesInDir.Select( f => $"{f.Name}\t//! Path: \"{f.FullName}\"" ) ) ;
		Logger.LogLine( ref reportText, "---\n" ) ;
	}

	public static void ReportGroup( ref string reportText, string header, 
										IEnumerable< string > list, string? comment = null ) {
		Logger.LogLine( ref reportText, header + 
						(comment is not null ? $"\n{comment}" : string.Empty) ) ;
		Logger.LogLines( ref reportText, list ) ;
		Logger.LogLine( ref reportText, "---\n" ) ;
	}
	
	// ====================================================
} ;