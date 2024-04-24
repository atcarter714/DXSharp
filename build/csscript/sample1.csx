using System ;
using System.IO ;
using System.Net ;
using System.Text.Json ;

//! Simply call the Program.Main(...) method ...
Program.Main( args ) ; // ... and relay it our command-line arguments

internal class Program {
    internal record Repository( string Name ) ;
    internal record Content( string Type, string Name, string DownloadUrl ) ;


    static string GetDefaultDownloadFolder( ) {
        string userFolder = Environment.GetFolderPath( Environment.SpecialFolder.UserProfile ) ;
        return Path.Combine( userFolder, "Downloads" ) ;
    }


    static void Main( string[ ] args ) {
        const string DEFAULT_FOLDER = "Downloads";
        const string DEFAULT_URI = "https://api.github.com";
        const string E_CMDLINE_ARGS = "Invalid Command-line Arguments: ";
        ArgumentNullException.ThrowIfNull( args, nameof(args) ) ;
        ArgumentException.ThrowIfNullOrEmpty( args[0], E_CMDLINE_ARGS + "args[0]" ) ;
        ArgumentException.ThrowIfNullOrEmpty( args[1], E_CMDLINE_ARGS + "args[1]" ) ;
        if ( args.Length <= 1 ) {
            Console.WriteLine( E_CMDLINE_ARGS + "Insufficient arguments provided." ) ;
            return ;
        }

        string responseJson ;
        string orgOrUser = args[ 0 ], searchTopic = args[ 1 ] ;
        string apiUrl = $"{DEFAULT_URI}/users/{orgOrUser}/repos" ;
        
        using WebClient client = new( DEFAULT_URI ) ;
        responseJson = client.DownloadString( orgOrUser ) ;
        
        var repositories = JsonSerializer
                            .Deserialize< Repository[] >( responseJson ) ;
        
        foreach ( var repository in repositories ) {
            Console.WriteLine( $"Repository: {repository.Name}" ) ;

            // recycles the same WebClient instance:
            string repoResponseJson ;
            string repoUrl = $"/repos/{orgOrUser}/{repository.Name}/contents" ;
            repoResponseJson = client.DownloadString( repoUrl ) ;
            
            var contents = JsonSerializer
                            .Deserialize< Content[] >( repoResponseJson ) ;
            
            foreach ( var content in contents ) {
                if ( content.Type is "file" && content.Name.Contains(searchTopic) ) {
                    Console.WriteLine( $"File: {content.Name}" ) ;
                    Console.WriteLine( $"Download URL: {content.DownloadUrl}" ) ;
                }
            }
        }
    }
} ;


/*
        static void Main( string[ ] args ) {
            const string DEFAULT_FOLDER = "Downloads", DEFAULT_URI =
            const string GITHUB_API_URL = "https://api.github.com", 
                        E_CMDLINE_ARGS = "Invalid Command-line Arguments: " ;
            ArgumentNullException.ThrowIfNull( args, nameof(args) ) ;
            ArgumentException.ThrowIfNullOrEmpty( args[ 0 ], E_CMDLINE_ARGS+ "args[ 0 ]" ) ;
            ArgumentException.ThrowIfNullOrEmpty( args[ 1 ], E_CMDLINE_ARGS+ "args[ 1 ]" ) ;
            if ( args.Length <= 1 ) {
                Console.WriteLine( E_CMDLINE_ARGS+ "Insufficient arguments provided." ) ;
                return ;
            }
            
            
        }

        static string GetDefaultDownloadFolder( ) {
            string userFolder = Environment.GetFolderPath( Environment.SpecialFolder.UserProfile ) ;
            return Path.Combine( userFolder, "Downloads" ) ;
        }
    }
    xxx(...) {
        public string Type { get; set; }
        public string Name { get; set; }
        public string DownloadUrl { get; set; }
    } ;
 */