#region Using Directives
using System.Drawing ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Framework.Graphics ;


/// <summary>
/// Represents options for the rendering surfaces or "backbuffer(s)".
/// </summary>
public partial struct BackBufferOptions {
	// -----------------------------------------------------
	/// <summary>A simple set of default backbuffer options.</summary>
	/// <remarks>
	/// Double-buffering (<see cref="Count"/>: <c>2</c>) with the default clear color of
	/// <see cref="ClearValue.TextureDefault"/> (i.e., <see cref="Color.Black"/>).
	/// </remarks>
	public static readonly BackBufferOptions Default 
		= new( 2, ClearValue.TextureDefault ) ;
	// -----------------------------------------------------
	
	
	/// <summary>The number of backbuffers to use.</summary>
	public uint Count { get ; set ; } = 2 ;
	/// <summary>Whether or not to allow tearing.</summary>
	/// <remarks><b>WARNING</b>:
	/// <para>Be sure to first check for the presence of the <see cref="Feature.AllowTearing"/> flag
	/// before setting this value to ensure it is supported by the hardware.</para>
	/// </remarks>
	public bool AllowTearing { get ; set ; } = false ;
	/// <summary>The clear color for the backbuffer(s).</summary>
	public ClearValue ClearColor { get ; set ; } = new( ) {
		Format         = Format.R8G8B8A8_UNORM,
		ClearValueData = new( Color.Black ),
	} ;
	
	
	
	// -----------------------------------------------------
	
	public BackBufferOptions( uint       count      = 2, 
							  ClearValue clearValue = default ) {
		Count      = count ;
		ClearColor = clearValue ;
	}
	
	// =====================================================
} ;