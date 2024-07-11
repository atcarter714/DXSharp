#region Using Directives
using System.Collections ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Dxgi.Common ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>
/// Represents a color in the RGB (Red, Green, Blue) color space
/// with single-precision (<see cref="float"/>) floating-point values.
/// </summary>
[StructLayout(LayoutKind.Explicit,
			  Size = SizeInBytes)]
[EquivalentOf(typeof(DXGI_RGB))]
public partial struct RGB: IEnumerable<float> {
	public const int ElementCount = 3,
					 ElementSize  = sizeof(float),
					 SizeInBytes  = ElementCount * ElementSize ;
	/// <summary>Fixed-size buffer of <see cref="ElementCount"><value>3</value></see> in length.</summary>
	[FieldOffset(0)] public unsafe fixed float _data[ 3 ] ;
	[FieldOffset(sizeof(float) * 0)] public float Red ;
	[FieldOffset(sizeof(float) * 1)] public float Green ; 
	[FieldOffset(sizeof(float) * 2)] public float Blue ;
	
	/// <summary>
	/// Gets a reference to the floating-point value (<see cref="float"/>) element at the specified index.
	/// </summary>
	/// <param name="index">Reference to the value at the specified index.</param>
	public unsafe ref float this[ int index ] => ref _data[ index ] ;
	
	/// <summary>Reference to the <see cref="RGB"/>'s <see cref="Red"/> ("<see cref="R"/>") value.</summary>
	public unsafe ref float R => ref _data[ 0 ] ;
	/// <summary>Reference to the <see cref="RGB"/>'s <see cref="Green"/> ("<see cref="G"/>") value.</summary>
	public unsafe ref float G => ref _data[ 1 ] ;
	/// <summary>Reference to the <see cref="RGB"/>'s <see cref="Blue"/> ("<see cref="B"/>") value.</summary>
	public unsafe ref float B => ref _data[ 2 ] ;

	public RGB( in RGB rgb ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = rgb.Red ;
			_data[ 1 ] = rgb.Green ;
			_data[ 2 ] = rgb.Blue ;
		}
	}
	public RGB( in Color color ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = color.R ;
			_data[ 1 ] = color.G ;
			_data[ 2 ] = color.B ;
		}
	}
	public RGB( float red, float green, float blue ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = red ;
			_data[ 1 ] = green ;
			_data[ 2 ] = blue ;
		}
	}
	public RGB( (byte red, byte green, byte blue) value ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = value.red / 255f ;
			_data[ 1 ] = value.green / 255f ;
			_data[ 2 ] = value.blue / 255f ;
		}
	}
	public RGB( in (short red, short green, short blue) value ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = value.red / 255f ;
			_data[ 1 ] = value.green / 255f ;
			_data[ 2 ] = value.blue / 255f ;
		}
	}
	
	
	// -------------------------------------------------------------
	//! IEnumerable Implementation:
	public IEnumerator< float > GetEnumerator( ) {
		for ( int i = 0; i < ElementCount; ++i )
			yield return this[ i ] ;
	}
	IEnumerator IEnumerable.GetEnumerator( ) => GetEnumerator( ) ;
	// -------------------------------------------------------------
	
	
	// ------------------------------------------------------------------------------------------------------------
	#region Conversions
	public static implicit operator RGB( (float red, float green, float blue) value ) =>
		new RGB( value.red, value.green, value.blue ) ;
	public static implicit operator (float red, float green, float blue)( RGB value ) =>
		( value.Red, value.Green, value.Blue ) ;
	
	public static implicit operator RGB( (int red, int green, int blue) value ) =>
		new RGB( value.red / 255f, value.green / 255f, value.blue / 255f ) ;
	public static implicit operator (int red, int green, int blue)( RGB value ) =>
		( (int)( value.Red * 255 ), (int)( value.Green * 255 ), (int)( value.Blue * 255 ) ) ;
	
	public static implicit operator RGB( (uint red, uint green, uint blue) value ) =>
		new RGB( value.red / 255f, value.green / 255f, value.blue / 255f ) ;
	public static implicit operator (uint red, uint green, uint blue)( RGB value ) =>
		( (uint)( value.Red * 255 ), (uint)( value.Green * 255 ), (uint)( value.Blue * 255 ) ) ;
	
	public static implicit operator RGB( Vector3   vec )   => new( vec.X, vec.Y, vec.Z ) ;
	public static implicit operator Vector3( RGB   rgb )   => new( rgb.Red, rgb.Green, rgb.Blue ) ;
	
	public static explicit operator RGB( in ColorF color ) => new( color.r, color.g, color.b ) ;
	public static implicit operator ColorF( in RGB color )  => new( color.Red, color.Green, color.Blue ) ;
	
	public static implicit operator RGB( DXGI_RGB value ) => new( value.Red, value.Green, value.Blue ) ;
	public static implicit operator DXGI_RGB( RGB value ) => new DXGI_RGB {
		Red = value.Red, Green = value.Green, Blue = value.Blue,
	} ;
	#endregion
	// ============================================================================================================
} ;


#warning Requires unit tests ...
//! TODO: Write tests/documentation for this type and ensure it works as expected.
//! Code was AI-generated using above RGB type as a template/example ...
/// <summary>
/// Represents a color in the RGB (Red, Green, Blue) color space
/// with 8-bit unsigned integer values.
/// </summary>
[StructLayout( LayoutKind.Explicit,
			   Size = SizeInBytes )]
public partial struct RGB8: IEnumerable< byte > {
	public const int ElementCount = 3, ElementSize = 1,
					 SizeInBytes  = ElementCount * ElementSize ;
	 
	[FieldOffset( 0 )] public unsafe fixed byte _data[ 3 ] ;
	[FieldOffset( 0 )] public byte Red ;
	[FieldOffset( 1 )] public byte Green ;
	[FieldOffset( 2 )] public byte Blue ;
	
	public unsafe ref byte this[ int index ] => ref _data[ index ] ;
	public unsafe ref byte R => ref _data[ 0 ] ;
	public unsafe ref byte G => ref _data[ 1 ] ;
	public unsafe ref byte B => ref _data[ 2 ] ;
	
	public RGB8( in RGB8 rgb ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = rgb.Red ;
			_data[ 1 ] = rgb.Green ;
			_data[ 2 ] = rgb.Blue ;
		}
	}
	public RGB8( in Color color ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = (byte)( color.R * 255 ) ;
			_data[ 1 ] = (byte)( color.G * 255 ) ;
			_data[ 2 ] = (byte)( color.B * 255 ) ;
		}
	}
	public RGB8( byte red, byte green, byte blue ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = red ;
			_data[ 1 ] = green ;
			_data[ 2 ] = blue ;
		}
	}
	public RGB8( (byte red, byte green, byte blue) value ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = value.red ;
			_data[ 1 ] = value.green ;
			_data[ 2 ] = value.blue ;
		}
	}
	public RGB8( in (short red, short green, short blue) value ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			_data[ 0 ] = (byte)( value.red / 255 ) ;
			_data[ 1 ] = (byte)( value.green / 255 ) ;
			_data[ 2 ] = (byte)( value.blue / 255 ) ;
		}
	}
	
	
	// -------------------------------------------------------------
	//! IEnumerable Implementation:
	public IEnumerator< byte > GetEnumerator( ) {
		for ( int i = 0; i < ElementCount; ++i )
			yield return this[ i ] ;
	}
	IEnumerator IEnumerable.GetEnumerator( ) => GetEnumerator( ) ;
	// -------------------------------------------------------------
	
	
	// ------------------------------------------------------------------------------------------------------------
	#region Conversions
	public static implicit operator RGB8( (byte red, byte green, byte blue) value ) =>
		new RGB8( value.red, value.green, value.blue ) ;
	public static implicit operator (byte red, byte green, byte blue)( RGB8 value ) =>
		( value.Red, value.Green, value.Blue ) ;
	
	public static implicit operator RGB8( (int red, int green, int blue) value ) =>
	 		new RGB8( (byte)( value.red / 255 ), (byte)( value.green / 255 ), (byte)( value.blue / 255 ) ) ;
	public static implicit operator (int red, int green, int blue)( RGB8 value ) =>
	 		( (int)( value.Red * 255 ), (int)( value.Green * 255 ), (int)( value.Blue * 255 ) ) ;
	
	public static implicit operator RGB8( (uint red, uint green, uint blue) value ) =>
	 		new RGB8( (byte)( value.red / 255 ), (byte)( value.green / 255 ), (byte)( value.blue / 255 ) ) ;
	
	public static implicit operator (uint red, uint green, uint blue)( RGB8 value ) =>
	 		( (uint)( value.Red * 255 ), (uint)( value.Green * 255 ), (uint)( value.Blue * 255 ) ) ;
	
	public static implicit operator RGB8( in Color color ) => new( color.R, color.G, color.B ) ;
	public static implicit operator Color( in RGB8 rgb ) => Color.FromArgb( rgb.Red, rgb.Green, rgb.Blue ) ;
	#endregion
	// ============================================================================================================
} ;