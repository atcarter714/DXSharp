using System ;


// ----------------------------------------------------
// Helper class to generate log text:
// ----------------------------------------------------

/// <summary>A helper class for generating log file text and reports.</summary>
public static class Logger {
	/// <summary>Gets or sets the current log file text indentation level.</summary>
    public static int IndentLevel { get; set; } = 0 ;

    static Dictionary< int, string > _indentLevelCache = new( ) {
        { 0, string.Empty },
        { 1, "\t" },
        { 2, "\t\t" },
        { 3, "\t\t\t" },
        { 4, "\t\t\t\t" },
        { 5, "\t\t\t\t\t" },
        { 6, "\t\t\t\t\t\t" },
        { 7, "\t\t\t\t\t\t\t" },
        { 8, "\t\t\t\t\t\t\t\t" },
    } ;
    static string Indentation {
        get {
            if( _indentLevelCache.TryGetValue( IndentLevel, out string value ) ) {
                return value ;
            }
            string additionalIndent = 
                new( Enumerable.Repeat( '\t', IndentLevel ).ToArray( ) ) ;
            _indentLevelCache.Add( IndentLevel, additionalIndent ) ;
            return additionalIndent ;
        }
    }
    
    /// <summary>
	/// Appends a line of text to the existing log text.
	/// </summary>
	/// <param name="text">Reference to the existing <see cref="string"/> of text.</param>
	/// <param name="input">New string to append onto the existing text.</param>
    public static void LogLine( ref string text, string input ) => text += $"{Indentation}{input}\n" ;
    
	/// <summary>
	/// Appends a series of lines of text to the existing log text.
	/// </summary>
	/// <param name="text">Reference to the existing <see cref="string"/> of text.</param>
	/// <param name="list">Collection of strings to append onto the existing text.</param>
    public static void LogLines( ref string text, IEnumerable< string > list ) {
		if( list is null || list.Count() < 1 ) {
			text += $"{Indentation}<NONE>\n" ;
			return ;
		}
        foreach( var item in list ) {
            text += $"{Indentation}{item}\n" ;
        }
    }
    
	public static void SkipLine( ref string text ) => text += "\n" ;

	/// <summary>
	/// Displays a string of text to the output stream/console.
	/// </summary>
	/// <param name="input">String to write to output display.</param>
    public static void Display( string input ) => Console.WriteLine( input ) ;
    public static void DisplayLines( IEnumerable<string> input ) {
		foreach( var line in input )
			Console.WriteLine( input ) ;
	}
    

	/// <summary>
	/// Displays a welcome message to the output stream/console.
	/// This can include a start time, caller name, script name, and extra message.
	/// </summary>
	/// <param name="startTime">A start time for the process.</param>
	/// <param name="callerName">Display name for the process running the script.</param>
	/// <param name="scriptName">Name of the script file being invoked.</param>
	/// <param name="extraMessage">Additional info or text to display.</param>
    public static void WelcomeMessage( DateTime? startTime   = default,
										string? callerName   = null,
										string? scriptName   = null,
                                        string? extraMessage = null ) {
        startTime  ??= DateTime.Now ;
		scriptName ??= "(?*).csx script" ;
		callerName ??= "unknown caller (?)" ;
		
        ConsoleColor oldFGColor = Console.ForegroundColor, oldBGColor = Console.BackgroundColor ;
        Console.ForegroundColor = ConsoleColor.Green ; Console.BackgroundColor = ConsoleColor.Black ;
        Console.WriteLine( "________________________________________________________" ) ;
        Console.ForegroundColor = oldFGColor ; Console.BackgroundColor = oldBGColor ;
        
        Console.WriteLine( $"\nC# Script engine invoked with \"{scriptName}\" by {callerName} @: {startTime}" 
									+ (extraMessage is {Length: > 0} str 
                                        ? $"\n{str}\n" : " ...\n" ) ) ;
										
        oldFGColor = Console.ForegroundColor ; oldBGColor = Console.BackgroundColor ;
        Console.ForegroundColor = ConsoleColor.Green ; Console.BackgroundColor = ConsoleColor.Black ;
        Console.WriteLine( "________________________________________________________" ) ;
        Console.ForegroundColor = oldFGColor ; Console.BackgroundColor = oldBGColor ;
    }


	/// <summary>
	/// Saves the log file to the specified output path.
	/// </summary>
	/// <param name="logText">The text to save into the file.</param>
	/// <param name="logFilePath">The path to save the file to.</param>
	public static void SaveLogFile( string logFilePath, string logText, bool overwrite = true ) {
		if( !overwrite && File.Exists( logFilePath ) ) {
			File.AppendAllText( logFilePath, logText ) ;
		}
		else File.WriteAllText( logFilePath, logText ) ;
	}
}