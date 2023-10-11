namespace DXSharp.Applications ;


public record AppSettings( string Title, USize WindowSize = default,
						   AppSettings.Style? StyleSettings = default,
						   AppSettings.Advanced? AdvancedSettings = default ) {
	public static readonly USize DEFAULT_WINDOW_SIZE = (DEFAULT_WIDTH, DEFAULT_HEIGHT) ;
	public const uint DEFAULT_WIDTH  = 1280, DEFAULT_HEIGHT = 720 ;
	public const string DEFAULT_APP_NAME = "DXSharp" ;
	
	public static readonly AppSettings Default = new AppSettings {
		Title = DEFAULT_APP_NAME,
		WindowSize = DEFAULT_WINDOW_SIZE,
		AdvancedSettings = new AppSettings.Advanced {
			UseWarpDevice = false,
			EnableDebugLayer = false,
			EnableParallelJobs = false,
			EnableParallelDraw = false,
			EnableParallelSim = false,
			EnableParallelCompute = false,
		},
	} ;
	
	public AppSettings( ): this( DEFAULT_APP_NAME, (DEFAULT_WIDTH, DEFAULT_HEIGHT) ) { }
	public AppSettings( string title ): this( title, (DEFAULT_WIDTH, DEFAULT_HEIGHT) ) { }
	public AppSettings( string title, USize windowSize ): this( title, windowSize, default ) { }
	
	
	public class Style {
		public const float  DEFAULT_FONT_SIZE = 14.0f ;
		public const string DEFAULT_FONT_NAME = "Arial" ;
		public static readonly Color DEFAULT_BUFFER_COLOR = Color.Black ;
		public static readonly Color DEFAULT_BACKGROUND_COLOR = SystemColors.Window ;
		public static readonly Color DEFAULT_FOREGROUND_COLOR = SystemColors.Control ;
		public static readonly Style DefaultStyle = new( ) ;
		
		public float FontSize { get ; set ; } = DEFAULT_FONT_SIZE ;
		public string? FontName { get ; set ; } = DEFAULT_FONT_NAME ;
		public Color BackBufferColor { get ; set ; } = DEFAULT_BUFFER_COLOR ;
		public Color BackgroundColor { get ; set ; } = DEFAULT_BACKGROUND_COLOR ;
		public Color ForegroundColor { get ; set ; } = DEFAULT_FOREGROUND_COLOR ;
		
		public Style( ) { }
		public Style( float fontSize, string fontName, 
					  Color backBufferColor, Color backgroundColor, Color foregroundColor ) {
			FontSize        = fontSize ;
			FontName        = fontName ;
			BackBufferColor = backBufferColor ;
			BackgroundColor = backgroundColor ;
			ForegroundColor = foregroundColor ;
		}
	} ;
	
	public class Advanced {
		public static readonly ParallelOptions DefaultParallelismSettings = new ParallelOptions {
			MaxDegreeOfParallelism = HardwareInfo.MaxParallelism,
			CancellationToken      = DXWinformApp.AppCancelTokenSource.Token,
			TaskScheduler          = null,
		} ;
		public static readonly Advanced AdvancedDefaults = new Advanced {
			UseWarpDevice = false,
			EnableDebugLayer = false,
			EnableParallelSim = false,
			EnableParallelJobs = false,
			EnableParallelDraw = false,
			EnableParallelCompute = false,
			ParallelismSettings = DefaultParallelismSettings,
		} ;
		
		// ---------------------------------------------------------------------------
		
		public bool UseWarpDevice = false, EnableDebugLayer = false ;
		public ParallelOptions ParallelismSettings = DefaultParallelismSettings ;
		public bool EnableParallelJobs = false,
					EnableParallelDraw = false,
					EnableParallelSim  = false,
					EnableParallelCompute = false ;
		
		// ---------------------------------------------------------------------------
		// ---------------------------------------------------------------------------
		
		public Advanced( ) { }
		
		public Advanced( bool useWarpDevice, bool enableDebugLayer ) {
			UseWarpDevice = useWarpDevice ;
			EnableDebugLayer = enableDebugLayer ;
		}
		
		public Advanced( bool useWarpDevice, bool enableDebugLayer, bool enableParallelism ) {
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
			ParallelismSettings = parallelismSettings ?? DefaultParallelismSettings ;
		}
	} ;
} ;