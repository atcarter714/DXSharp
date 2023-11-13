#region Using Directives
using System.Collections ;
using System.Drawing ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Framework.Graphics ;


/// <summary>
/// Represents the sets of options for graphics pipeline settings.
/// </summary>
public class GraphicsSettings {
	// -----------------------------------------------------
	/// <summary>A simple set of default graphics pipeline settings.</summary>
	public static readonly GraphicsSettings Default = new( ) {
		DisplayOptions    = new( DisplayOptions.DefaultModes ),
		BackBufferOptions = BackBufferOptions.Default,
		DisplayMode       = DisplayMode.Default,
	} ;
	// -----------------------------------------------------
	
	/// <summary>The active <see cref="DisplayMode"/> for the graphics pipeline settings.</summary>
	public required DisplayMode DisplayMode { get ; set ; }
	
	/// <summary>The available display options for the graphics pipeline settings.</summary>
	public DisplayOptions? DisplayOptions { get ; set ; }
	/// <summary>The backbuffer options for the graphics pipeline settings.</summary>
	public BackBufferOptions BackBufferOptions { get ; set ; }
	
	
	// -----------------------------------------------------

	public GraphicsSettings( ) {
		DisplayOptions    = DisplayOptions.Default ;
		BackBufferOptions = BackBufferOptions.Default ;
	}
	public GraphicsSettings( DisplayOptions? displayOptions = null,
							 BackBufferOptions? backBufferOptions = null ) {
		DisplayOptions    = displayOptions ;
		BackBufferOptions = backBufferOptions 
								?? BackBufferOptions.Default ;
	}
	
	
	// =====================================================
} ;

