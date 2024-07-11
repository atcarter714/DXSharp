using System.Runtime.Versioning;
namespace DXSharp.Applications ;


/// <summary>
/// A record containing important settings for DXSharp applications.
/// These settings are used to easily configure the application to run in a specific way
/// or to change the default behavior or appearance of the application.
/// </summary>
/// <param name="Title">A "title" or "caption" for the application.</param>
/// <param name="WindowSize">The window client size area (width and height).</param>
/// <param name="StyleSettings">Additional settings for the visual style/appearance of the app.</param>
/// <param name="AdvancedSettings">Advanced settings for the behavior and capabilities of the app.</param>
[SupportedOSPlatform( "windows7.0" )]
public record AppSettings( string Title, USize WindowSize = default,
						     AppSettings.Style? StyleSettings = default,
								AppSettings.Advanced? AdvancedSettings = default ) {
	// ----------------------------------------------------------------------------------------------------
	public static readonly USize DEFAULT_WINDOW_SIZE = (DEFAULT_WIDTH, DEFAULT_HEIGHT) ;
	public const uint DEFAULT_WIDTH  = 1280, DEFAULT_HEIGHT = 720 ;
	public const string DEFAULT_APP_NAME = "DXSharp" ;
	
	/// <summary>A set of basic default settings for the app.</summary>
	public static readonly AppSettings Default = new() {
		Title = DEFAULT_APP_NAME,
		WindowSize = DEFAULT_WINDOW_SIZE,
		StyleSettings = Style.DefaultStyle,
		AdvancedSettings = Advanced.AdvancedDefaults,
	} ;
	
	// ----------------------------------------------------------------------------------------------------
	
	
	/// <summary>Additional settings for app style/appearance.</summary>
	public AppSettings.Style StyleSettings { get ; protected init ; } 
									= StyleSettings ?? Style.DefaultStyle ;
	
	/// <summary>Advanced settings for the behavior and capabilities of the app.</summary>
	public AppSettings.Advanced AdvancedSettings { get ; protected init ; }
									= AdvancedSettings ?? Advanced.AdvancedDefaults ;


	// -----------------------------------------------------
	// Constructors:
	// -----------------------------------------------------
	
	public AppSettings( ): 
		this( DEFAULT_APP_NAME, (DEFAULT_WIDTH, DEFAULT_HEIGHT) ) { }
	
	public AppSettings( string title ): 
		this( title, (DEFAULT_WIDTH, DEFAULT_HEIGHT) ) { }
	
	public AppSettings( string title, USize windowSize ): 
		this( title, windowSize, Style.DefaultStyle ) { }
	
	
	// ----------------------------------------------------------------------------------------------------
	public class Style {
		// -----------------------------------------------------
		// Default Style Settings:
		// -----------------------------------------------------
		public const float DEFAULT_FONT_SIZE = 14.0f ; public const string DEFAULT_FONT_NAME = "Arial" ;
		public static readonly Font  DEFAULT_FONT = new( DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE ) ;
		public static readonly Color DEFAULT_BUFFER_COLOR = Color.Black ,
									 DEFAULT_BACKGROUND_COLOR = SystemColors.Window,
									 DEFAULT_FOREGROUND_COLOR = SystemColors.Control ;
		
		/// <summary>Default style settings for DXSharp applications.</summary>
		public static readonly Style DefaultStyle = new( ) {
			FontSize = DEFAULT_FONT_SIZE,
			FontName = DEFAULT_FONT_NAME,
			BackBufferColor = DEFAULT_BUFFER_COLOR,
			BackgroundColor = DEFAULT_BACKGROUND_COLOR,
			ForegroundColor = DEFAULT_FOREGROUND_COLOR,
		} ;
		// -----------------------------------------------------
		
		
		public float FontSize { get ; set ; } = DEFAULT_FONT_SIZE ;
		public string? FontName { get ; set ; } = DEFAULT_FONT_NAME ;
		public Font? Font => new( FontName ?? DEFAULT_FONT_NAME, FontSize ) ;
		public Color BackBufferColor { get ; set ; } = DEFAULT_BUFFER_COLOR ;
		public Color BackgroundColor { get ; set ; } = DEFAULT_BACKGROUND_COLOR ;
		public Color ForegroundColor { get ; set ; } = DEFAULT_FOREGROUND_COLOR ;
		
		
		// -----------------------------------------------------
		// Constructors:
		// -----------------------------------------------------
		public Style( ) { }
		public Style( float fontSize      = 10f,
					  string? fontName     = null,
					  Color backBufferColor = default,
					  Color  backgroundColor = default,
					  Color foregroundColor   = default ) {
			FontSize        = fontSize ;
			FontName        = fontName ;
			BackBufferColor = backBufferColor ;
			BackgroundColor = backgroundColor ;
			ForegroundColor = foregroundColor ;
		}
		// -----------------------------------------------------
		
	} ;


	public class Advanced {
		public static readonly ParallelOptions DefaultParallelismSettings = new ParallelOptions {
			MaxDegreeOfParallelism = HardwareInfo.MaxParallelism,
			CancellationToken      = DXWinformApp.AppCancelTokenSource.Token,
			TaskScheduler          = null,
		} ;
		public static readonly Advanced AdvancedDefaults = new() {
			UseWarpDevice = false,
			EnableDebugLayer = false,
			EnableParallelSim = false,
			EnableParallelJobs = false,
			EnableParallelDraw = false,
			EnableParallelCompute = false,
			ParallelismSettings = DefaultParallelismSettings,
		} ;
		
		// ---------------------------------------------------------------------------
		
		public bool UseWarpDevice = false,
					EnableDebugLayer = false ;
		public ParallelOptions ParallelismSettings = DefaultParallelismSettings ;
		public bool EnableParallelJobs    = false,
					EnableParallelDraw    = false,
					EnableParallelSim     = false,
					EnableParallelCompute = false ;
		
		// ---------------------------------------------------------------------------
		// ---------------------------------------------------------------------------
		
		public Advanced( ) { }
		
		public Advanced( bool enableDebugLayer = false ) =>
						EnableDebugLayer = enableDebugLayer ;

		public Advanced( bool useWarpDevice = false,
						 bool enableDebugLayer = false,
						 bool enableParallelism = false ) {
			UseWarpDevice = useWarpDevice ;
			EnableDebugLayer = enableDebugLayer ;
			EnableParallelJobs = enableParallelism ;
			EnableParallelDraw = enableParallelism ;
			EnableParallelSim  = enableParallelism ;
			EnableParallelCompute = enableParallelism ;
		}
		
		public Advanced( bool useWarpDevice = false, 
								 bool enableDebugLayer = false, 
								 bool enableParallelJobs = false,
								 bool enableParallelDraw = false, 
								 bool enableParallelSim = false,
								 bool enableParallelCompute = false,
								 ParallelOptions? parallelismSettings = null ) {
			UseWarpDevice = useWarpDevice ;
			EnableDebugLayer = enableDebugLayer ;
			EnableParallelJobs = enableParallelJobs ;
			EnableParallelDraw = enableParallelDraw ;
			EnableParallelSim  = enableParallelSim ;
			EnableParallelCompute = enableParallelCompute ;
			ParallelismSettings = parallelismSettings 
									?? DefaultParallelismSettings ;
		}
	} ;
	// ====================================================================================================
} ;