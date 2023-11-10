using System.Collections ;
using Windows.Graphics.Display.Core ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp ;
using DXSharp.DXGI ;

namespace AdvancedDXS.Framework.Graphics ;


/// <summary>Represents a screen or image resolution.</summary>
public readonly struct Resolution {
	public USize Size { get ; }
	public uint Width => Size.Width ;
	public uint Height => Size.Height ;
	
	public Resolution( USize size ) => Size = size ;
	public static implicit operator USize( Resolution res ) => res.Size ;
	public static implicit operator Resolution( USize size ) => new( size ) ;
} ;


/// <summary>Represents a display output refresh rate.</summary>
public readonly struct RefreshRate {
	public Rational Hz { get ; }
	public uint Numerator => Hz.Numerator ;
	public uint Denominator => Hz.Denominator ;
	
	public RefreshRate( Rational hz ) => Hz = hz ;
	public RefreshRate( uint numerator, uint denominator ) {
		Hz = ( numerator, denominator ) ;
	}
	
	public static implicit operator Rational( RefreshRate rate ) => rate.Hz ;
	public static implicit operator RefreshRate( Rational hz ) => new( hz ) ;
} ;


public readonly struct DisplayMode {
	public RefreshRate RefreshRate { get ; }
	public Resolution Resolution { get ; }
	public bool IsStereo { get ; }
	
	public DisplayMode( Resolution resolution,
							RefreshRate refreshRate = default,
								bool isStereo = false ) {
		Resolution = resolution ;
		RefreshRate = refreshRate ;
		IsStereo = isStereo ;
	}
} ;


public sealed class DisplayOptions: IEnumerable< DisplayMode > {
	public List< DisplayMode > AvailableDisplayModes { get ; }
	
	public DisplayOptions( ) =>
		AvailableDisplayModes = new( ) ;
	public DisplayOptions( IEnumerable<DisplayMode> options ) =>
		AvailableDisplayModes = new( options ) ;
	public DisplayOptions( params DisplayMode[ ] options ) =>
		AvailableDisplayModes = new( options ) ;

	public IEnumerator< DisplayMode > GetEnumerator( ) => AvailableDisplayModes.GetEnumerator( ) ;
	IEnumerator IEnumerable.GetEnumerator( ) => ( (IEnumerable)AvailableDisplayModes ).GetEnumerator( ) ;
} ;



public class GraphicsSettings {
	public static readonly GraphicsSettings Default = new( ) {
		BufferCount = 2,
	} ;
	
	public uint BufferCount { get ; set ; } = 2 ;
	public DisplayOptions? DisplayOptions { get ; set ; }
	
	
	public GraphicsSettings( ) { }
	public GraphicsSettings( uint bufferCount ) {
		BufferCount = bufferCount ;
	}
} ;

