#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi.Common ;

#endregion

namespace DXSharp ;

[StructLayout(LayoutKind.Sequential)]
public struct RGB8 {
	public readonly byte Red, Green, Blue ;
	public RGB8( RGB8 rgb ) { Red = rgb.Red ; Green = rgb.Green ; Blue = rgb.Blue ; }
	public RGB8( byte red, byte green, byte blue ) { Red = red ; Green = green ; Blue = blue ; }
	public RGB8( Color color ) { Red = color.R ; Green = color.G ; Blue = color.B ; }
	
	#region Conversions
	public static implicit operator RGB8( (byte red, byte green, byte blue) value ) =>
		new RGB8( value.red, value.green, value.blue ) ;
	public static implicit operator (byte red, byte green, byte blue)( RGB8 value ) =>
		( value.Red, value.Green, value.Blue ) ;
	
	public static implicit operator RGB8( (int red, int green, int blue) value ) =>
	 		new RGB8( (byte)value.red, (byte)value.green, (byte)value.blue ) ;
	public static implicit operator (int red, int green, int blue)( RGB8 value ) =>
	 		( value.Red, value.Green, value.Blue ) ;
	
	public static implicit operator RGB8( (uint red, uint green, uint blue) value ) =>
	 		new RGB8( (byte)value.red, (byte)value.green, (byte)value.blue ) ;
	public static implicit operator (uint red, uint green, uint blue)( RGB8 value ) =>
	 		( value.Red, value.Green, value.Blue ) ;
	
	public static implicit operator RGB8( in RGB value ) =>
		new RGB8( (byte)( value.Red * 255 ), (byte)( value.Green * 255 ),
					(byte)( value.Blue * 255 ) ) ;
	public static implicit operator RGB( in RGB8 value ) =>
	 		new RGB( value.Red / 255f, value.Green / 255f, value.Blue / 255f ) ;
	#endregion
} ;

[StructLayout(LayoutKind.Sequential)]
public struct RGB {
 	public float Red, Green, Blue ;
	public RGB( RGB rgb ) { Red = rgb.Red ; Green = rgb.Green ; Blue = rgb.Blue ; }
	public RGB( float red, float green, float blue ) { Red = red ; Green = green ; Blue = blue ; }
	public RGB( Color color ) { Red = color.R / 255f ; Green = color.G / 255f ; Blue = color.B / 255f ; }
	 
	
	#region Conversions
	public static implicit operator RGB( (float red, float green, float blue) value ) =>
		new RGB( value.red, value.green, value.blue ) ;
	public static implicit operator (float red, float green, float blue)( RGB value ) =>
		( value.Red, value.Green, value.Blue ) ;
	
	public static implicit operator RGB( (byte red, byte green, byte blue) value ) =>
		new RGB( value.red / 255f, value.green / 255f, value.blue / 255f ) ;
	public static implicit operator (byte red, byte green, byte blue)( RGB value ) =>
		( (byte)( value.Red * 255 ), (byte)( value.Green * 255 ), (byte)( value.Blue * 255 ) ) ;
	
	public static implicit operator RGB( (int red, int green, int blue) value ) =>
		new RGB( value.red / 255f, value.green / 255f, value.blue / 255f ) ;
	public static implicit operator (int red, int green, int blue)( RGB value ) =>
		( (int)( value.Red * 255 ), (int)( value.Green * 255 ), (int)( value.Blue * 255 ) ) ;
	
	public static implicit operator RGB( (uint red, uint green, uint blue) value ) =>
		new RGB( value.red / 255f, value.green / 255f, value.blue / 255f ) ;
	public static implicit operator (uint red, uint green, uint blue)( RGB value ) =>
		( (uint)( value.Red * 255 ), (uint)( value.Green * 255 ), (uint)( value.Blue * 255 ) ) ;
	
	public static implicit operator RGB( (short red, short green, short blue) value ) =>
		new RGB( value.red / 255f, value.green / 255f, value.blue / 255f ) ;
	public static implicit operator (short red, short green, short blue)( RGB value ) =>
		( (short)( value.Red * 255 ), (short)( value.Green * 255 ), (short)( value.Blue * 255 ) ) ;
	
	public static implicit operator RGB( Vector3   vec )   => new( vec.X, vec.Y, vec.Z ) ;
	public static implicit operator Vector3( RGB   rgb )   => new( rgb.Red, rgb.Green, rgb.Blue ) ;
	
	public static explicit operator RGB( in ColorF color ) => new( color.r, color.g, color.b ) ;
	public static implicit operator ColorF( in RGB color )  => new( color.Red, color.Green, color.Blue ) ;

	public static implicit operator RGB( DXGI_RGB value ) => new( value.Red, value.Green, value.Blue ) ;
	public static implicit operator DXGI_RGB( RGB value ) => new DXGI_RGB {
		Red = value.Red, Green = value.Green, Blue = value.Blue,
	} ;

#endregion

} ;