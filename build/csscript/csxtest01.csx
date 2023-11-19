using System ;
using System.IO ;
using System.Linq ;

static DateTime startTime = DateTime.Now ;

var oldColor = Console.ForegroundColor ;
Console.ForegroundColor = ConsoleColor.Green ;
Console.WriteLine( "__________________________________" ) ;
Console.ForegroundColor = oldColor ;

Console.WriteLine( "\nC# Script engine started ..." ) ;
Console.WriteLine( "DXSharp post-build actions ..." ) ;
Console.WriteLine( $"Start Time: {startTime}" ) ;

var workingDir = Directory.GetCurrentDirectory( ) ;

const string outputDir = @"..\..\build\logs\" ;
const string logFile = "DXSharpPostBuild.log" ;
string logOutputPath = Path.Combine( outputDir, logFile ) ;

string text = string.Empty ;


// Construct collection of all DXSharp directories:
var DXSharpDirectories = Directory.GetDirectories( workingDir, "*", SearchOption.TopDirectoryOnly ) ;
text += "DXSharp Directories ::\n" ;
foreach( var dir in DXSharpDirectories ) {
    string folderName = dir.Replace( workingDir, string.Empty ) ;
    text += $"{dir}\n" ;
}
text += "\n" ;


// Save log file:
File.WriteAllText( logOutputPath, text ) ;