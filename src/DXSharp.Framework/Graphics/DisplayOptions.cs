#region Using Directives
using System.Collections ;
using DXSharp.DXGI ;

#endregion
namespace DXSharp.Framework.Graphics ;


/// <summary>A set of <see cref="DisplayMode"/> options for the graphics pipeline.</summary>
public sealed class DisplayOptions: IEnumerable< DisplayMode >,
									IEquatable< DisplayOptions > {
	// ----------------------------------------------------------------------------------
	/// <summary>
	/// A default set of common <see cref="DisplayMode"/> options
	/// which are generally supported by most graphics adapters and displays.
	/// </summary>
	/// <remarks>
	/// This is a good starting point for most applications, but you <i>cannot</i> rely
	/// on these modes being supported by local machine's graphics adapters and displays.
	/// </remarks>
	public static readonly IReadOnlyList< DisplayMode > DefaultModes = new DisplayMode[ ] {
		new ( new(1920, 1080),
						 ( 60, 1 ),
						 false, Format.R8G8B8A8_UNORM ),
		new ( new(1280, 720),
						 ( 60, 1 ),
						 false, Format.R8G8B8A8_UNORM ),
		new ( new(1920, 1080),
			  ( 60, 1 ),
			  false, Format.R8G8B8A8_UNORM_SRGB ),
		new ( new(1280, 720),
			  ( 60, 1 ),
			  false, Format.R8G8B8A8_UNORM_SRGB ),
		new ( new(1920, 1080),
			  ( 60, 1 ),
			  false, Format.B8G8R8A8_UNORM ),
	} ;
	/// <summary>The default set of <see cref="DisplayOptions"/> options.</summary>
	public static readonly DisplayOptions Default = new( DefaultModes ) ;
	// ----------------------------------------------------------------------------------
	
	
	/// <summary>Gets a list of available <see cref="DisplayMode"/> options.</summary>
	public List< DisplayMode > AvailableDisplayModes { get ; }
	/// <summary>Gets the <see cref="DisplayMode"/> at the specified index.</summary>
	public DisplayMode this[ int index ] => AvailableDisplayModes[ index ] ;
	/// <summary>Gets the number of <see cref="DisplayMode"/> options.</summary>
	public int Count => AvailableDisplayModes.Count ;
	
	
	public DisplayOptions( ) =>
		AvailableDisplayModes = new( ) ;
	public DisplayOptions( IEnumerable< DisplayMode > options ) =>
		AvailableDisplayModes = new( options ) ;
	public DisplayOptions( params DisplayMode[ ] options ) =>
		AvailableDisplayModes = new( options ) ;
	public DisplayOptions( DisplayMode one ) {
		AvailableDisplayModes = new( ) ;
		AvailableDisplayModes.Add( one ) ;
	}
	
	// ----------------------------------------------------------------------------------
	public IEnumerator< DisplayMode > GetEnumerator( ) => 
		AvailableDisplayModes.GetEnumerator( ) ;
	IEnumerator IEnumerable.GetEnumerator( ) => 
		((IEnumerable)AvailableDisplayModes).GetEnumerator( ) ;
	
	public override int GetHashCode( ) => HashCode.Combine( AvailableDisplayModes.GetHashCode( ), 
															AvailableDisplayModes.Count ) ;

	public override bool Equals( object? obj ) => 
		obj is DisplayOptions other && Equals(other) ;
	
	public bool Equals( DisplayOptions? other ) => 
		( (ReferenceEquals( other?.AvailableDisplayModes, this.AvailableDisplayModes ) ||
		   (other?.AvailableDisplayModes?.SequenceEqual(AvailableDisplayModes) ?? false)) ) ;
	// ==================================================================================
} ;