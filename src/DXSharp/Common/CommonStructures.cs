#region Using Directives
using System.Globalization ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;

using Windows.Win32 ;

using static DXSharp.InteropUtils ;
using SysVec4 = System.Numerics.Vector4 ;
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
[DebuggerDisplay("ToString()")]
public struct ColorF: IFormattable {
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
		get { fixed( float* fptr = &_color.X )
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
	
	public override bool Equals( object? obj ) => obj is ColorF other && this == other ;
	public override int GetHashCode( ) => _color.GetHashCode( ) ;
	
	public override string ToString( ) => $"ColorF: {{ R: {R}, G: {G}, B: {B}, A: {A} }}" ;
	
	public string ToString( string? format, IFormatProvider? formatProvider = null ) {
		if ( string.IsNullOrEmpty(format) ) format  = "G" ;
		if( formatProvider is null ) formatProvider = CultureInfo.CurrentCulture ;
		var str                                       = $"ColorF: {{ " +
																		  $"R: {R.ToString(format, formatProvider)}, " +
																		  $"G: {G.ToString(format, formatProvider)}, " +
																		  $"B: {B.ToString(format, formatProvider)}, " +
																		  $"A: {A.ToString(format, formatProvider)} }}" ;
		return str ;
	}

	public static ColorF FromArgb( float a = 0, float r = 0, float g = 0, float b = 0 ) => new( r, g, b, a ) ;
	public static ColorF FromArgb( float a = 0, Vector3 rgb = default ) => new( rgb.X, rgb.Y, rgb.Z, a ) ;
	
	
	public static implicit operator ColorF( in Vector4 color ) => new( color ) ;
	public static implicit operator Vector4( in ColorF color ) => color._color ;
	public static implicit operator ColorF( in Color color ) => new( color ) ;
	public static implicit operator Color( in ColorF color ) => color.Color ;
	public static implicit operator ColorF( in SysVec4 color ) => new( color ) ;
	
	public static implicit operator ColorF( in __float_4 color ) => new( color ) ;
	public static implicit operator __float_4( in ColorF color ) {
		unsafe {
			fixed ( ColorF* cptr = &color ) {
				__float_4* fptr = (__float_4 *)cptr ;
				return *fptr ;
			}
		}
	}
	
	public static implicit operator ColorF( in (float r, float g, float b, float a) tuple ) => 
		new( tuple.r, tuple.g, tuple.b, tuple.a ) ;
	public static implicit operator ColorF( in (Vector3 rgb, float a) tuple ) => 
		new( tuple.rgb.X, tuple.rgb.Y, tuple.rgb.Z, tuple.a ) ;
	
	[MethodImpl(_MAXOPT_)] public static bool operator ==( ColorF  left, ColorF right ) => left._color == right._color ;
	[MethodImpl(_MAXOPT_)] public static bool operator !=( ColorF  left, ColorF right ) => left._color != right._color ;
	[MethodImpl(_MAXOPT_)] public static bool operator ==( ColorF  left, Vector4 right ) => left._color == right.v ;
	[MethodImpl(_MAXOPT_)] public static bool operator !=( ColorF  left, Vector4 right ) => left._color != right.v ;
	[MethodImpl(_MAXOPT_)] public static bool operator ==( Vector4 left, ColorF right ) => left.v == right._color ;
	[MethodImpl(_MAXOPT_)] public static bool operator !=( Vector4 left, ColorF right ) => left.v != right._color ;
	
	[MethodImpl(_MAXOPT_)] public static ColorF operator +( ColorF left, ColorF right ) => new( left._color + right._color ) ;
	[MethodImpl(_MAXOPT_)] public static ColorF operator -( ColorF left, ColorF right ) => new( left._color - right._color ) ;
	[MethodImpl(_MAXOPT_)] public static ColorF operator *( ColorF left, ColorF right ) => new( left._color * right._color ) ;
	[MethodImpl(_MAXOPT_)] public static ColorF operator /( ColorF left, ColorF right ) => new( left._color / right._color ) ;
	
	[MethodImpl(_MAXOPT_)] public static ColorF operator *( ColorF left, float  right ) => left._color * right ;
	[MethodImpl(_MAXOPT_)] public static ColorF operator /( ColorF left, float  right ) => left._color.VectorRef / right ;
	[MethodImpl(_MAXOPT_)] public static ColorF operator *( float  left, ColorF right ) => new( left * right._color ) ;
} ;



[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct LargeInteger {
	[FieldOffset(0)]
	public uint LowPart ;
	
	[FieldOffset(4)]
	public int HighPart ;

	// This is the 64-bit integer representation of the LARGE_INTEGER.
	[FieldOffset(0)]
	public long QuadPart ;
}


/// <summary>Represents a range of memory or addresses.</summary>
public readonly struct AddressRange: IEquatable< AddressRange > {
	public readonly nint Start, End ;
	public ulong Size => (ulong)( End - Start ) ;
	
	public AddressRange( nint start = 0x0000,
						 nint end   = int.MaxValue ) {
		End   = end ;
		Start = start ;
	}
	
	// ------------------------------------------------------------------------------------------------------------
	
	public override string ToString( ) =>
		$"{nameof(AddressRange)}: {{ 0x{Start:X8} - 0x{End:X8} ({Size} bytes) }}" ;
	
	public override bool Equals( object? obj ) =>
		obj is AddressRange addrRange && Equals( addrRange ) ;
	
	public override int GetHashCode( ) => HashCode.Combine( Start, End ) ;

	public bool Equals( AddressRange other ) => Start == other.Start 
												&& End == other.End 
												&& Size == other.Size ;

	
	// ------------------------------------------------------------------------------------------------------------
	
	public static AddressRange From( nint start, nint end ) => new( start, end ) ;
	public static AddressRange From( ulong start, nuint size ) => new( (nint)start, (nint)(start + size) ) ;
	public static AddressRange From( nint start, ulong size ) => new( start, (nint)((ulong)start + size) ) ;
	public static AddressRange From( ulong start, ulong size ) => new( (nint)start, (nint)((ulong)start + size) ) ;
	public static unsafe AddressRange From( void* start, nuint size ) => new( (nint)start, (nint)((ulong)start + size) ) ;
	public static unsafe AddressRange From( void* start, void* size ) => new( (nint)start, (nint)((ulong)start + (ulong)size) ) ;
	
	public static AddressRange Offset( in AddressRange range, int offset = 0x0000 ) =>
								new( range.Start + offset, range.End + offset ) ;
	public static AddressRange Offset( in AddressRange range, nint offset = 0x0000 ) =>
								new( range.Start + offset, range.End + offset ) ;
	public static AddressRange Offset( in AddressRange range, ulong offset = 0x0000 ) =>
								new( range.Start + (nint)offset, range.End + (nint)offset ) ;
	
	// ------------------------------------------------------------------------------------------------------------
	// Operator Overloads ::
	// ------------------------------------------------------------------------------------------------------------
	public static implicit operator AddressRange( (nint start, nint end) value ) =>
														new( value.start, value.end ) ;
	
	public static bool operator ==( AddressRange left, AddressRange right ) => left.Equals( right ) ;
	public static bool operator !=( AddressRange left, AddressRange right ) => !left.Equals( right ) ;
	
	public static AddressRange operator +( AddressRange left, long right )  =>
		new( left.Start, (nint)((long)left.End + right) ) ;
	public static AddressRange operator +( AddressRange left, ulong right ) =>
		new( left.Start, (nint)((ulong)left.End + right) ) ;
	public static AddressRange operator +( AddressRange left, int right )  =>
		new( left.Start, (nint)(left.End + right) ) ;
	public static AddressRange operator +( AddressRange left, uint right ) =>
		new( left.Start, (nint)(left.End + right) ) ;
	
	public static AddressRange operator -( AddressRange left, long right )  =>
	 		new( left.Start, (nint)((long)left.End - right) ) ;
	public static AddressRange operator -( AddressRange left, ulong right ) =>
	 		new( left.Start, (nint)((ulong)left.End - right) ) ;
	public static AddressRange operator -( AddressRange left, int right )  =>
	 		new( left.Start, (nint)(left.End - right) ) ;
	public static AddressRange operator -( AddressRange left, uint right ) =>
	 		new( left.Start, (nint)(left.End - right) ) ;
	// ============================================================================================================
} ;
