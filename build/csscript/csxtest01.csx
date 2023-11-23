#load ".\include\DirectoryHelper.csx"
#load ".\include\LogHelpers.csx"
#load ".\include\ArchitectureLogs.csx"
using System ;
using System.IO ;
using System.Linq ;
using static ArchitectureLogs ;

static DateTime startTime = DateTime.Now ;

// ----------------------------------------------------
// Welcome message:
Logger.WelcomeMessage( startTime,
                        "DXSharp Post-Build Events", "csxtest01.csx",
                        $"Generating reports of repository architecture and contents ..." ) ;

// Get current working directory:
var workingDir = Directory.GetCurrentDirectory( ) ;
var solutionDir = DirectoryHelper.RootSolutionDir ;
Logger.Display( $"Current working directory:\n\t{workingDir}" ) ;
Logger.Display( $"Solution directory:\n\t{solutionDir}" ) ;


// ----------------------------------------------------
// Declare local consts and variables:
// ----------------------------------------------------
// Determine log file output directory:
const string outputDir = @"..\..\build\logs\" ;
const string logFile   = "DXSharpPostBuild.log" ;
string logOutputPath   = Path.Combine( outputDir, logFile ) ;
// ----------------------------------------------------


// ----------------------------------------------------
// String to fill with log text:
string text = string.Empty ;
// ----------------------------------------------------


// Construct collection of all DXSharp directories:
Logger.Display( $"--------------------------------" ) ;
Logger.Display( $"\nEnumerating root solution directory items: \"{solutionDir}\" ..." ) ;
var DXSharpDirectories = Directory.GetDirectories( solutionDir, "*",
                                                    SearchOption.TopDirectoryOnly ) ;
Logger.Display( $"{DXSharpDirectories.Length} directories found (logging results) ..." ) ;
Logger.Display( $"--------------------------------\n" ) ;

// ----------------------------------------------------
// Construct a log report text of DXSharp architecture:
// ----------------------------------------------------
// Construct collection of all DXSharp files:
var DXSharpFiles = Directory.GetFiles( solutionDir, "*", SearchOption.TopDirectoryOnly )
                            .Select( f => f.Replace( solutionDir, string.Empty ) )
                                .ToList( ) ;
                                
var buildFiles     = DXSharpFiles.Where( f => f.Contains( ".Build." ) ) ;
var yamlFiles      = DXSharpFiles.Where( f => f.EndsWith( ".yaml" ) || f.EndsWith( ".yml" ) ) ;
var propsFiles     = buildFiles.Where( f => f.EndsWith( ".props" ) ) ;
var targetsFiles   = buildFiles.Where( f => f.EndsWith( ".targets" ) ) ;
var markdownFiles  = DXSharpFiles.Where( f => f.EndsWith( ".md" ) ) ;
var gitignoreFiles = DXSharpFiles.Where( f => f is ".gitignore" ) ;
var solutionFiles  = DXSharpFiles.Where( f => f.EndsWith( ".sln" ) ) ;
var textFiles      = DXSharpFiles.Where( f => f.EndsWith( ".txt" ) ) ;

//! Get all projects in solution:
var allProjects = DirectoryHelper.GetAllProjectsInSolution( ) ;
var allSDKProjects = DirectoryHelper.GetAllSDKProjects( ) ;
var allTestsProjects = DirectoryHelper.GetAllTestProjects( ) ;
var allSamplesProjects = DirectoryHelper.GetAllSampleProjects( ) ;
// ----------------------------------------------------
// Display \DXSharp contents:
// ----------------------------------------------------
Logger.LogLine( ref text, $"{ARCHITECTURE_HEADER}" ) ;
Logger.LogLine( ref text, @"//! Architecture Report Logs:" 
                            + startTime.ToString( "MM-dd-yy HH:mm:ss" ) ) ;
Logger.LogLine( ref text, "//! This file describes the repository file system architecture." ) ;
Logger.LogLine( ref text, "//! (It can be used for tools and AI ...)" ) ;
Logger.LogLine( ref text, $"_______________________________\n" ) ;

Logger.LogLine( ref text, $"{SLN_DIR_CONTENT_HEADER}" ) ;
Logger.LogLine( ref text, @"//! Root Solution Directory:" ) ;
Logger.LogLine( ref text, $"\n{DIR_HEADER}" ) ;

var DXSharpDirectoriesList = DXSharpDirectories.Select( s => s.Remove(0, solutionDir.Length) )
                                                    .ToList( ) ;
Logger.LogLines( ref text, DXSharpDirectoriesList ) ;
Logger.LogLine( ref text, "......." ) ;

// Log root solution files:
if( solutionFiles.Any( ) ) {
    ArchitectureLogs.ReportGroup( ref text,
                                    $"\n{SLN_FILE_HEADER}", 
                                    solutionFiles, @"//! Solution Files:" ) ;
}

// Log MSBuild files:
if( buildFiles.Any( ) ) {
    ArchitectureLogs.ReportGroup( ref text,
                                    $"\n{MSBUILD_FILES_HEADER}", 
                                    buildFiles, @"//! MSBuild Script Files:" ) ;
}

// Log .yaml pipeline files:
if( yamlFiles.Any( ) ) {
    ArchitectureLogs.ReportGroup( ref text,
                                    $"\n{YAML_FILES_HEADER}", 
                                    yamlFiles, @"//! Yaml Pipeline Files:" ) ;
}

// Log .md files:
if( markdownFiles.Any( ) ) {
    ArchitectureLogs.ReportGroup( ref text,
                                    $"\n{MARKDOWN_FILES_HEADER}", 
                                    markdownFiles, @"//! Markdown/ReadMe Files:" ) ;
}

// Log .txt files:
if( textFiles.Any( ) ) {
    ArchitectureLogs.ReportGroup( ref text,
                                    $"\n{TEXT_FILES_HEADER}", 
                                    textFiles, @"//! Text Files:" ) ;
}

// Additional Files/Content:
var AdditionalFiles = DXSharpFiles.Where( 
                    f => !buildFiles.Contains( f ) &&
                         !yamlFiles.Contains( f ) &&
                         !markdownFiles.Contains( f ) &&
                         !solutionFiles.Contains( f ) &&
                         !gitignoreFiles.Contains( f ) ) ;
if( AdditionalFiles.Any( ) ) {
    ArchitectureLogs.ReportGroup( ref text, $"\n{ADDITIONAL_FILES_HEADER}", 
                                    AdditionalFiles.ToArray( ),
                                    @"//! Additional Files/Content:" ) ;
}


//! Log list of projects in solution:
Logger.IndentLevel = 0 ;
Logger.SkipLine( ref text ) ;
Logger.LogLine( ref text,  "// -------------------" ) ;
Logger.LogLine( ref text, $"// Projects List: //! {solutionDir}" ) ;
Logger.LogLine( ref text,  "// -------------------" ) ;

// Report all project files in log:
++Logger.IndentLevel ;
ArchitectureLogs.ReportGroup( ref text, $"\n{PROJECT_FILES_HEADER}",
                                allProjects.Select( p => p.FullName.Replace( solutionDir, string.Empty ) ), 
                                    $"//! Projects in solution: \"{solutionDir}\"\n" ) ;
                                     
Logger.SkipLine( ref text ) ;
++Logger.IndentLevel ;   
// Show SDK projects:
ArchitectureLogs.ReportGroup( ref text, $"\n{SDK_LIBRARY_HEADER}",
                                allSDKProjects.Select( p => p.FullName.Replace( solutionDir, string.Empty ) ), 
                                    $"//! SDK Projects in solution: \"{solutionDir}\"\n" ) ;
                                     
Logger.SkipLine( ref text ) ;
// Show Test projects:
ArchitectureLogs.ReportGroup( ref text, $"\n{TEST_FILES_HEADER}",
                                allTestsProjects.Select( p => p.FullName.Replace( solutionDir, string.Empty ) ), 
                                    $"//! Test Projects in solution: \"{solutionDir}\"\n" ) ;
                                     
Logger.SkipLine( ref text ) ;
// Show Sample projects:
ArchitectureLogs.ReportGroup( ref text, $"\n{SAMPLE_FILES_HEADER}",
                                allSamplesProjects.Select( p => p.FullName.Replace( solutionDir, string.Empty ) ), 
                                    $"//! Sample Projects in solution: \"{solutionDir}\"\n" ) ; 
Logger.IndentLevel = 0 ; // Reset indent level


// ---------------------------------------------------------------------------------------------


// -----------------------------------------------------------------
//! Generate Breakdown of Folder Contents
// -----------------------------------------------------------------
Logger.Display( $"\nScanning \"main\" solution directories ..." ) ;
Logger.Display( "Generating architectural log report ..." ) ;
var MainSlnFolders = DirectoryHelper.MainSolutionFolders ;

Logger.SkipLine( ref text ) ;
Logger.LogLine( ref text,  "// -------------------" ) ;
Logger.LogLine( ref text, $"// Directory Contents: //! {solutionDir}" ) ;
Logger.LogLine( ref text,  "// -------------------" ) ;
++Logger.IndentLevel ;

foreach( var dir in MainSlnFolders ) {
    string dirFullPath = Path.Combine( solutionDir, dir ) ;
    DirectoryInfo info = new( dirFullPath ) ;
    
    // Skip special folders:
    if( DirectoryHelper.IsSpecialFolder( info.Name ) ) {
        continue ;
    }
    
    // Report folder contents in log:
    ArchitectureLogs.CreateContentsReport( ref text, dirFullPath ) ;
}


// -----------------------------------------------------------------
//! End of log:
Logger.IndentLevel = 0 ;
Logger.LogLine( ref text, $"\n_______________________________\n<EOF/>" ) ;
Logger.Display( $"\nScript tool execution completed ..." ) ;
Logger.Display( $"\nSaving architecture report log ..." ) ;
// -----------------------------------------------------------------


// ----------------------------------------------------
// Save text into the target log file:
// ----------------------------------------------------
var _saveFilePath = logOutputPath ;
if( !Directory.Exists( outputDir ) ) {
    var targetFolder = Path.Combine( DirectoryHelper.RootSolutionDir, 
        "build", "logs" ) ;
    Directory.CreateDirectory( targetFolder ) ;
    _saveFilePath = new FileInfo( Path.Combine( targetFolder, logFile ) ) 
                            .FullName ;
}
// ----------------------------------------------------
Logger.SaveLogFile( _saveFilePath, text, true ) ;
Logger.Display( $"\nSaved architecture report log to:\n\"{_saveFilePath}\"\n..." ) ;
//! End of script execution:
// ======================================================================================

