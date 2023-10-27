#region Using Directives
using System ;
using System.Diagnostics.CodeAnalysis ;
using System.Numerics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
#endregion
namespace DXSharp ;


/// <summary>Represents a size using unsigned (32-bit) integers.</summary>
/// <remarks>Convenient in some interop scenarios with Win32/COM and D3D.</remarks>
public struct USize {
	public uint Width, Height ;
	public USize( (uint width, uint height) size ) => ( Width, Height ) = size ;
	public USize( Size size ) => ( Width, Height ) = ( (uint)size.Width, (uint)size.Height ) ;
	public USize( uint width = 0x0000U, uint height = 0x0000U ) => ( Width, Height ) = ( width, height ) ;
	public static implicit operator USize( (uint width, uint height) size ) => new( size.width, size.height ) ;
	public static implicit operator (uint width, uint height)( USize size ) => ( size.Width, size.Height ) ;
	public static implicit operator USize( Size size ) => new( (uint)size.Width, (uint)size.Height ) ;
	public static implicit operator Size( USize size ) => new( (int)size.Width, (int)size.Height ) ;
} ;


[StructLayout( LayoutKind.Explicit )]
public struct ColorF {
	[FieldOffset(0)]   Vector4 _color ;
	
	[FieldOffset(0)] public float r ;
	[FieldOffset(sizeof(float))] public float g ;
	[FieldOffset(sizeof(float) * 2)] public float b ;
	[FieldOffset(sizeof(float) * 3)] public float a ;
	
	public Vector4 Color4 => _color ;
	public Color Color => Color.FromArgb( (int)( _color.W * 255 ),
										  (int)( _color.X * 255 ),
										  (int)( _color.Y * 255 ),
										  (int)( _color.Z * 255 ) ) ;
	
	
	public ref float R => ref this[ 0 ] ;
	public ref float G => ref this[ 1 ] ;
	public ref float B => ref this[ 2 ] ;
	public ref float A => ref this[ 3 ] ;

	public unsafe ref float this[ int index ] {
		get { 
			fixed( float* fptr = &_color.X )
				return ref fptr[ index ] ;
		}
	}
	
	
	public ColorF( Vector4 color ) => _color = color ;

	public ColorF( __float_4 color ) {
		unsafe {
			fixed( float* fptr = &_color.X )
				*( (__float_4*)fptr ) = color ;
		}
	}
	public ColorF( float r, float g, float b, float a = 1.0f ) => _color = new( r, g, b, a ) ;
	public ColorF( Color color ) => _color = new( color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f ) ;
	public ColorF( byte r, byte g, byte b, byte a = 255 ) => _color = new( r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f ) ;

	
	public unsafe float[ ] ToArray( ) {
		float[ ] arr = new float[ 4 ] ;
		fixed ( float* fptr = arr ) {
			*((Vector4*)fptr) = _color ;
		}
		return arr ;
	}
	
	[UnscopedRef] public unsafe Span< float > AsSpan( ) {
		fixed( float* fptr = &_color.X )
			return new( fptr, 4 ) ;
	}
	
	[UnscopedRef] public ref __float_4 As_float4( ) {
		unsafe {
			fixed( float* fptr = &_color.X )
				return ref Unsafe.AsRef< __float_4 >( fptr ) ;
		}
	}
	
	
	public ColorF WithAlpha( float alpha ) => new( R, G, B, alpha ) ;
	public ColorF WithRed( float red ) => new( red, G, B, A ) ;
	public ColorF WithGreen( float green ) => new( R, green, B, A ) ;
	public ColorF WithBlue( float blue ) => new( R, G, blue, A ) ;
	
	public static ColorF FromArgb( float a = 0, float r = 0, float g = 0, float b = 0 ) => new( r, g, b, a ) ;
	public static ColorF FromArgb( float a = 0, Vector3 rgb = default ) => new( rgb.X, rgb.Y, rgb.Z, a ) ;
	
	public static implicit operator ColorF( Vector4 color ) => new( color ) ;
	public static implicit operator Vector4( ColorF color ) => color._color ;
	public static implicit operator ColorF( Color color ) => new( color ) ;
	public static implicit operator Color( ColorF color ) => color.Color ;
} ;