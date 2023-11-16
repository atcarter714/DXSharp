#region Using Directives
using System.Collections ;
using System.Drawing ;
using Windows.Win32.Foundation ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows ;

#endregion
namespace DXSharp.Framework.Graphics ;

//public class GraphicsModeState


/// <summary>
/// Represents the sets of options for graphics pipeline settings.
/// </summary>
public class GraphicsSettings {
	// -----------------------------------------------------
	/// <summary>A simple set of default graphics pipeline settings.</summary>
	public static readonly GraphicsSettings Default = new( ) {
		DisplayOptions    = new( DisplayOptions.DefaultModes ),
		RenderTargetProperties = RenderTargetProperties.Default,
		CurrentMode       = DisplayMode.Default,
	} ;
	// -----------------------------------------------------
	
	/// <summary>The active <see cref="CurrentMode"/> for the graphics pipeline settings.</summary>
	public required DisplayMode CurrentMode { get ; set ; }
	
	/// <summary>The available display options for the graphics pipeline settings.</summary>
	public DisplayOptions? DisplayOptions { get ; set ; }
	
	/// <summary>The backbuffer options for the graphics pipeline settings.</summary>
	public RenderTargetProperties RenderTargetProperties { get ; set ; }
	
	
	/// <summary>Indicates if the graphics pipeline settings support device removed events.</summary>
	public bool SupportDeviceRemoved { get ; set ; } = true ;
	
	/// <summary>Indicates if the graphics pipeline should ignore Alt+Enter input.</summary>
	public bool DisableAltEnter { get ; set ; } = true ;
	
	/// <summary>Indicates if the graphics pipeline should ignore the Windows key.</summary>
	/// <remarks>
	/// Tearing is a feature that allows the application to render a new frame without waiting for the
	/// previous frame to be presented on the screen. This allows the application to run at full speed
	/// (unlocked frame rate), while still maintaining a VSynced presentation to the user.
	/// </remarks>
	public bool TearingSupport { get ; set ; }
	
	

	// -----------------------------------------------------

	public GraphicsSettings( ) {
		DisplayOptions    = DisplayOptions.Default ;
		RenderTargetProperties = RenderTargetProperties.Default ;
	}
	public GraphicsSettings( DisplayOptions? displayOptions = null,
							 RenderTargetProperties? backBufferOptions = null ) {
		DisplayOptions    = displayOptions ;
		RenderTargetProperties = backBufferOptions 
								?? RenderTargetProperties.Default ;
	}


	public static GraphicsSettings[ ] GetSupportedSettings( IFactory2 factory2 ) {
		return null ;
	}
	
	// =====================================================
} ;

