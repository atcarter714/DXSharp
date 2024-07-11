// COPYRIGHT NOTICES:
// --------------------------------------------------------------------------------
// Copyright © 2022 DXSharp - ATC - Aaron T. Carter
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------

#pragma warning disable CS1591

#region Using Directives
using System.Diagnostics ;
using System.Runtime.Intrinsics ;
using System.Diagnostics.CodeAnalysis ;
using System.Numerics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;

using SysVec2 = System.Numerics.Vector2 ;
using SysVec3 = System.Numerics.Vector3 ;
using SysVec4 = System.Numerics.Vector4 ;
using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp ;


/// <summary>A general purpose 2D Euclidean vector.</summary>
/// <remarks>
/// Utilizes <see cref="System.Numerics"/> vector
/// operations for SIMD instruction enhancements.
/// </remarks>
[DebuggerDisplay( "ToString()" )]
[StructLayout(LayoutKind.Explicit, Size = SizeInBytes)]
public partial struct Vector2: IEquatable< Vector2 >,
							   IEquatable< SysVec2 >, 
							   IFormattable {
	#region Const & Readonly Values
	public const int ComponentCount = 2,
					 ComponentSize  = sizeof(float),
					 SizeInBytes    = ComponentSize * 2 ;
	
	/// <summary>A vector with all components set to 0.0f </summary>
	/// <remarks>Useful for initialization.</remarks>
	public static readonly Vector2 Zero = new( 0f ) ;
	/// <summary>The Unit X vector (right).</summary>
	public static readonly Vector2 UnitX = new( 1f ) ;
	/// <summary>The Unit Y vector (up).</summary>
	public static readonly Vector2 UnitY = new( y: 1f ) ;
	/// <summary>A vector with all components set to 1.0f </summary>
	/// <remarks>Useful for scaling.</remarks>
	public static readonly Vector2 One = new( 1f ) ;
	
	/// <summary>A vector with all components set to NaN.</summary>
	/// <remarks>Useful for debugging.</remarks>
	public static readonly Vector2 NaN = new( float.NaN, float.NaN ) ;
	#endregion
	
	
	/// <summary>
	/// Creates a new 2D vector
	/// </summary>
	/// <param name="x">X-coordinate</param>
	/// <param name="y">Y-coordinate</param>
	public Vector2( float x = 0f, float y = 0f ) {
		Unsafe.SkipInit( out this ) ;
		v = new( x, y ) ;
	}

	/// <summary>
	/// Creates a new 2D vector
	/// </summary>
	/// <param name="vec">A System.Numerics.Vector2 value</param>
	public Vector2( SysVec2 vec = default ) {
		Unsafe.SkipInit( out this ) ;
		this.v = vec ;
	}
	
	
	[FieldOffset(0)] internal SysVec2 v = default ;
	[FieldOffset(ComponentSize * 0)] internal float x ;
	[FieldOffset(ComponentSize * 1)] internal float y ;
	
	internal SysVec2 V => v ;
	
	
	/// <summary>Gets reference to the specified component of the vector.</summary>
	/// <param name="index">The index of the component to get a reference to.</param>
	public ref float this[ int index ] {
		[MethodImpl(_MAXOPT_)] get {
			unsafe { fixed ( Vector2* ptr = &this ) {
					float* valuePtr = (float*)ptr ;
					return ref valuePtr[ index ] ;
				}
			}
		}
	}
	
	/// <summary>Reference to the X component of the vector.</summary>
	public ref float X => ref this[ 0 ] ;
	/// <summary>Reference to the Y component of the vector.</summary>
	public ref float Y => ref this[ 1 ] ;


	/// <summary>
	/// Gets the string representation of this vector value
	/// </summary>
	/// <returns>Vector in string form</returns>
	public override string ToString( ) => ToString( format: null, formatProvider: null ) ;
	
	/// <inheritdoc />
	public string ToString( string? format, IFormatProvider? formatProvider ) {
		if( format is null && formatProvider is null )
			return $"{nameof( Vector2 )} [ {X:G}, {Y:G} ]" ;
		
		return $"{nameof( Vector2 )} [ " +
			   $"{nameof( X )} = {X.ToString( format, formatProvider )}, " +
			   $"{nameof( Y )} = {Y.ToString( format, formatProvider )} ] " ;
	}

	
	/// <summary>
	/// Indicates whether this vector and the specified object are equal
	/// </summary>
	/// <param name="obj">Object to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public override bool Equals( [NotNullWhen( true )] object? obj ) =>
		( obj is Vector2 vec ) ? vec == this : base.Equals( obj ) ;
	public bool Equals( Vector2 other ) => this.v == other.v ;
	public bool Equals( SysVec2 other ) => this.v == other ;
	
	/// <summary>
	/// Returns the hash code this this vector
	/// </summary>
	/// <returns>32-bit integer hash code</returns>
	public override int GetHashCode( ) => v.GetHashCode( ) ;
	
	
	/// <summary>Interprets this vector as a span of floats.</summary>
	/// <returns>
	/// A span of floats with a length equal to the number of components in the vector (i.e., 2 floats).
	/// </returns>
	[UnscopedRef] public Span< float > AsSpan( ) =>
		MemoryMarshal.CreateSpan( ref x, ComponentCount ) ;
	
	
	// -----------------------------------------------------------------------------------------------

	public static Vector2 Add( in Vector2 a, in Vector2 b ) => a.v + b.v ;
	public static Vector2 Add( in Vector2 a, in SysVec2 b ) => a.v + b ;
	public static Vector2 Add( in SysVec2 a, in Vector2 b ) => a + b.v ;
	
	public static Vector2 Subtract( in Vector2 a, in Vector2 b ) => a.v - b.v ;
	public static Vector2 Subtract( in Vector2 a, in SysVec2 b ) => a.v - b ;
	public static Vector2 Subtract( in SysVec2 a, in Vector2 b ) => a - b.v ;
	
	public static Vector2 Multiply( in Vector2 a, in Vector2 b ) => a.v * b.v ;
	public static Vector2 Multiply( in Vector2 a, in SysVec2 b ) => a.v * b ;
	public static Vector2 Multiply( in SysVec2 a, in Vector2 b ) => a * b.v ;
	public static Vector2 Multiply( in Vector2 vec, float value ) => vec.v * value ;
	public static Vector2 Multiply( float value, in Vector2 vec ) => value * vec.v ;
	 
	public static Vector2 Divide( in Vector2 a, in Vector2 b ) => a.v / b.v ;
	public static Vector2 Divide( in Vector2 a, in SysVec2 b ) => a.v / b ;
	public static Vector2 Divide( in SysVec2 a, in Vector2 b ) => a / b.v ;
	public static Vector2 Divide( in Vector2 vec, float value ) => vec.v / value ;
	
	public static Vector2 Negate( in Vector2 vec ) => -vec.v ;
	
	public static Vector2 Normalize( in Vector2 vec ) => SysVec2.Normalize( vec.v ) ;
	
	public static float Dot( in Vector2 a, in Vector2 b ) => SysVec2.Dot( a.v, b.v ) ;
	
	public static float Distance( in Vector2 a, in Vector2 b ) => SysVec2.Distance( a.v, b.v ) ;
	
	public static float DistanceSquared( in Vector2 a, in Vector2 b ) => SysVec2.DistanceSquared( a.v, b.v ) ;
	
	public static float Length( in Vector2 vec ) => vec.v.Length( ) ;
	
	public static float LengthSquared( in Vector2 vec ) => vec.v.LengthSquared( ) ;
	
	public static Vector2 Lerp( in Vector2 a, in Vector2 b, float t ) => SysVec2.Lerp( a.v, b.v, t ) ;
	
	public static Vector2 Clamp( in Vector2 vec, in Vector2 min, in Vector2 max ) =>
		SysVec2.Clamp( vec.v, min.v, max.v ) ;
	
	public static Vector2 Clamp( in Vector2 vec, float min, float max ) =>
	 		SysVec2.Clamp( vec.v, new( min ), new( max ) ) ;
	
	public static Vector2 Transform( in Vector2 vec, in Matrix4x4 mat ) =>
		SysVec2.Transform( vec.v, mat ) ;
	
	public static Vector2 Transform( in Vector2 vec, in Quaternion quat ) =>
		SysVec2.Transform( vec.v, quat ) ;
	
	// -----------------------------------------------------------------------------------------------
	
	
	#region Operator Overloads
	// Comparison:
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in Vector2 a, in Vector2 b ) => (a.v == b.v) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in Vector2 a, in Vector2 b ) => (a.v != b.v) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in Vector2 a, in SysVec2 b ) => (a.v == b) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in Vector2 a, in SysVec2 b ) => (a.v != b) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in SysVec2 a, in Vector2 b ) => (a == b.v) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in SysVec2 a, in Vector2 b ) => (a != b.v) ;

	// to Vector2:
	[MethodImpl( _MAXOPT_ )] public static implicit operator Vector2( in SysVec2 vec ) => new( vec ) ;
	[MethodImpl( _MAXOPT_ )] public static explicit operator Vector2( in SysVec3 vec ) => new( vec.X, vec.Y ) ;
	[MethodImpl( _MAXOPT_ )] public static explicit operator Vector2( in SysVec4 vec ) => new( vec.X, vec.Y ) ;
	[MethodImpl( _MAXOPT_ )] public static explicit operator Vector2( in Vector3 vec ) => new( vec.X, vec.Y ) ;

	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static implicit operator __float_2( in Vector2 vec ) {
		unsafe {
			Unsafe.SkipInit( out __float_2 result ) ;
			__float_2* pDst = &result ;
			
			fixed( Vector2* pVec = &vec ) {
				__float_2* pSrc = (__float_2 *)pVec ;
				*pDst = *pSrc ;
			}
			
			return result ;
		}
	}
	
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static implicit operator Vector2( in __float_2 vec ) {
		unsafe {
			Unsafe.SkipInit( out Vector2 result ) ;
			Vector2* pDst = &result ;
			
			fixed( __float_2* pVec = &vec ) {
				Vector2* pSrc = (Vector2 *)pVec ;
				*pDst = *pSrc ;
			}
			
			return result ;
		}
	}
	
	// Vector2 >> SysVec:
	[MethodImpl( _MAXOPT_ )] public static implicit operator SysVec2( in Vector2 vec ) => vec.v ;
	[MethodImpl( _MAXOPT_ )] public static implicit operator SysVec3( in Vector2 vec ) => new( vec.X, vec.Y, 0f ) ;
	
	// Unary operators:
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator +( Vector2 vec ) => vec.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator -( Vector2 vec ) => -(vec.v) ;
	
	// Addition:
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator +( Vector2 a, Vector2 b ) => a.v + b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator +( Vector2 a, SysVec2 b ) => a.v + b ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator +( SysVec2 a, Vector2 b ) => a + b.v ;
	
	public static Vector3 operator +( Vector2 a, in Vector3 b ) => new( (SysVec3)a + b.v ) ;
	public static Vector3 operator +( Vector2 a, in SysVec3 b ) => new( a.v + (Vector3)b ) ;
	public static Vector3 operator +( in Vector3 a, Vector2    b ) => new( a.v + (SysVec3)b ) ;
	public static Vector3 operator +( in SysVec3 a, Vector2    b ) => ( a + (SysVec3)b ) ;
	
	// Subtraction:
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator -( Vector2 a, Vector2 b ) => a.v - b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator -( Vector2 a, SysVec2 b ) => a.v - b ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator -( SysVec2 a, Vector2 b ) => a - b.v ;
	
	public static Vector3 operator -( in Vector3 a, Vector2 b ) => new( a.v - (SysVec3)b ) ;
	public static Vector3 operator -( in SysVec3 a, Vector2 b ) => new( a - (SysVec3)b ) ;
	
	// Multiplication:
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator *( Vector2 a, Vector2 b ) => a.v * b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator *( Vector2 a, SysVec2 b ) => a.v * b ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator *( SysVec2 a, Vector2 b ) => a * b.v ;
	
	public static Vector3 operator *( Vector2 a, in Vector3 b ) => new( (SysVec3)a * b.v ) ;
	public static Vector3 operator *( Vector2 a, in SysVec3 b ) => new( (SysVec3)a * b ) ;
	public static Vector3 operator *( in Vector3 a, Vector2 b ) => new( a.v * (SysVec3)b ) ;
	public static Vector3 operator *( in SysVec3 a, Vector2 b ) => new( a * (SysVec3)b ) ;
	
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator *( Vector2 vec,   float   value ) => vec.v * value ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator *( float   value, Vector2 vec )   => value * vec.v ;
	
	// Division:
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator /( Vector2 vec, float value ) => vec.v / value ;
	
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator /( Vector2 a, Vector2 b ) => a.v / b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator /( Vector2 a, SysVec2 b ) => a.v / b ;
	[MethodImpl( _MAXOPT_ )] public static Vector2 operator /( SysVec2 a, Vector2 b ) => a / b.v ;
	
	public static Vector3 operator /( in Vector3 a, Vector2 b ) => new( a.v / (SysVec3)b ) ;
	public static Vector3 operator /( in SysVec3 a, Vector2 b ) => ( a / (SysVec3)b ) ;
	#endregion
} ;



/// <summary>A general purpose 3D Euclidean vector.</summary>
/// <remarks>
/// Utilizes <see cref="System.Numerics"/> vector
/// operations for SIMD instruction enhancements.
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = SizeInBytes)]
public partial struct Vector3: IEquatable< Vector3 >,
							   IEquatable< SysVec3 >,
							   IFormattable {
	#region Const & Readonly Values
	public const int ComponentCount = 3,
					 ComponentSize  = sizeof(float),
					 SizeInBytes    = ComponentSize * 3 ;
	/// <summary>A vector with all components set to 1.0f </summary>
	public static readonly Vector3 Zero = new( 0f, 0f, 0f ) ;
	/// <summary>The Unit X vector (right).</summary>
	public static readonly Vector3 UnitX = new( 1.0f ) ;
	/// <summary>The Unit Y vector (up).</summary>
	public static readonly Vector3 UnitY = new( y: 1.0f ) ;
	/// <summary>The Unit Z vector (forward).</summary>
	public static readonly Vector3 UnitZ = new( z: 1.0f ) ;
	/// <summary>A vector with all components set to 1.0f </summary>
	public static readonly Vector3 One = new( 1.0f, 1.0f, 1.0f ) ;
	/// <summary>A vector with all components set to NaN.</summary>
	/// <remarks>Useful for debugging.</remarks>
	public static readonly Vector3 NaN = new( float.NaN, float.NaN, float.NaN ) ;
	#endregion
	

	/// <summary>
	/// Creates a new 3D vector
	/// </summary>
	/// <param name="x">X-coordinate</param>
	/// <param name="y">Y-coordinate</param>
	/// <param name="z">Z-coordinate</param>
	public Vector3( float x = 0f, float y = 0f, float z = 0f ) {
		Unsafe.SkipInit( out this ) ;
		this.v = new( x, y, z ) ;
	}

	/// <summary>
	/// Creates a new 3D vector
	/// </summary>
	/// <param name="vec">A System.Numerics.Vector3 value</param>
	public Vector3( SysVec3 vec ) => this.v = vec;
	/// <summary>
	/// Creates a new 3D vector
	/// </summary>
	/// <param name="vec">A Vector2 value</param>
	public Vector3( Vector2 vec ) => this.v = new( vec.X, vec.Y, 0f );

	/// <summary>
	/// Creates a new 3D vector
	/// </summary>
	/// <param name="vec">A System.Numerics.Vector2 value</param>
	public Vector3( in SysVec2 vec ) {
		Unsafe.SkipInit( out this ) ;
		this.v = new( vec.X, vec.Y, 0f ) ;
	}
	
	public Vector3( in (float x, float y, float z) tuple ) {
		Unsafe.SkipInit( out this ) ;
		this.v = new( tuple.x, tuple.y, tuple.z ) ;
	}
	
	public Vector3( in Vector128< float > v128 ) {
		Unsafe.SkipInit( out this ) ;
		this.v = v128.AsVector3( ) ;
	}
	
	
	[FieldOffset(0)] internal SysVec3 v = default ;
	[FieldOffset(ComponentSize * 0)] internal float   x ;
	[FieldOffset(ComponentSize * 1)] internal float   y ;
	[FieldOffset(ComponentSize * 2)] internal float   z ;
	
	public ref float this[ int index ] {
		[MethodImpl(_MAXOPT_)]
		get { unsafe {
				fixed ( Vector3* ptr = &this ) {
					float* valuePtr = (float*)ptr ;
					return ref valuePtr[ index ] ;
				}
			}
		}
	}

	public ref float X => ref this[ 0 ] ;
	public ref float Y => ref this[ 1 ] ;
	public ref float Z => ref this[ 2 ] ;
	
	/// <summary>Interprets this vector as a span of floats.</summary>
	/// <returns>
	/// A span of floats with a length equal to the number of components in the vector (i.e., 3 floats).
	/// </returns>
	[UnscopedRef] public Span< float > AsSpan( ) => 
		MemoryMarshal.CreateSpan( ref x, ComponentCount ) ;
	
	
	/// <summary>
	/// Gets the string representation of this vector value
	/// </summary>
	/// <returns>Vector in string form</returns>
	public override string ToString( ) => ToString( format: null, formatProvider: null ) ;
	
	/// <inheritdoc />
	public string ToString( string? format, IFormatProvider? formatProvider ) {
		if( format is null && formatProvider is null )
			return $"{nameof(Vector3)} [ {X:G}, {Y:G}, {Z:G} ]" ;
		
		return $"{nameof(Vector3)} [ " +
			$"{nameof( X )} = {X.ToString( format, formatProvider )}, " +
			$"{nameof( Y )} = {Y.ToString( format, formatProvider )}, " +
			$"{nameof( Z )} = {Z.ToString( format, formatProvider )} ]" ;
	}

	/// <inheritdoc />
	public bool Equals( SysVec3 other ) => this.v == other ;

	/// <summary>
	/// Indicates whether this instance and the specified object are equal
	/// </summary>
	/// <param name="obj">Object to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public override bool Equals( [NotNullWhen( true )] object? obj ) =>
		(obj is Vector3 vec) ? vec == this : base.Equals( obj );
	
	/// <summary>
	/// Determines if this vector and the specified vector are equal
	/// </summary>
	/// <param name="other">Vector to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public bool Equals( Vector3 other ) => this.v == other.v;

	/// <summary>Returns the hash code this this vector</summary>
	/// <returns>32-bit integer hash code</returns>
	public override int GetHashCode( ) => v.GetHashCode( ) ;
	
	
	// -----------------------------------------------------------------------------------------------

	public static Vector3 Add( in Vector3 a, in Vector3 b ) => a.v + b.v ;
	public static Vector3 Add( in Vector3 a, in SysVec3 b ) => a.v + b ;
	public static Vector3 Add( in SysVec3 a, in Vector3 b ) => a + b.v ;
	
	public static Vector3 Subtract( in Vector3 a, in Vector3 b ) => a.v - b.v ;
	public static Vector3 Subtract( in Vector3 a, in SysVec3 b ) => a.v - b ;
	public static Vector3 Subtract( in SysVec3 a, in Vector3 b ) => a - b.v ;
	
	public static Vector3 Multiply( in Vector3 a, in Vector3 b ) => a.v * b.v ;
	public static Vector3 Multiply( in Vector3 a, in SysVec3 b ) => a.v * b ;
	public static Vector3 Multiply( in SysVec3 a, in Vector3 b ) => a * b.v ;
	public static Vector3 Multiply( in Vector3 vec, float value ) => vec.v * value ;
	public static Vector3 Multiply( float value, in Vector3 vec ) => value * vec.v ;
	 
	public static Vector3 Divide( in Vector3 a, in Vector3 b ) => a.v / b.v ;
	public static Vector3 Divide( in Vector3 a, in SysVec3 b ) => a.v / b ;
	public static Vector3 Divide( in SysVec3 a, in Vector3 b ) => a / b.v ;
	public static Vector3 Divide( in Vector3 vec, float value ) => vec.v / value ;
	
	public static Vector3 Negate( in Vector3 vec ) => -vec.v ;
	
	public static Vector3 Normalize( in Vector3 vec ) => SysVec3.Normalize( vec.v ) ;
	
	public static float Dot( in Vector3 a, in Vector3 b ) => SysVec3.Dot( a.v, b.v ) ;
	
	public static float Distance( in Vector3 a, in Vector3 b ) => SysVec3.Distance( a.v, b.v ) ;
	
	public static float DistanceSquared( in Vector3 a, in Vector3 b ) => SysVec3.DistanceSquared( a.v, b.v ) ;
	
	public static float Length( in Vector3 vec ) => vec.v.Length( ) ;
	
	public static float LengthSquared( in Vector3 vec ) => vec.v.LengthSquared( ) ;
	
	public static Vector3 Lerp( in Vector3 a, in Vector3 b, float t ) => SysVec3.Lerp( a.v, b.v, t ) ;
	
	public static Vector3 Clamp( in Vector3 vec, in Vector3 min, in Vector3 max ) =>
		SysVec3.Clamp( vec.v, min.v, max.v ) ;
	
	public static Vector3 Clamp( in Vector3 vec, float min, float max ) =>
	 		SysVec3.Clamp( vec.v, new( min ), new( max ) ) ;
	
	public static Vector3 Transform( in Vector3 vec, in Matrix4x4 mat ) =>
		SysVec3.Transform( vec.v, mat ) ;
	
	public static Vector3 Transform( in Vector3 vec, in Quaternion quat ) =>
		SysVec3.Transform( vec.v, quat ) ;
	
	public static Vector3 Cross( in Vector3 a, in Vector3 b ) => SysVec3.Cross( a.v, b.v ) ;
	
	// -----------------------------------------------------------------------------------------------
	
	
	#region Operator Overloads
	
	//! (NOTE: Trying out inlining/optimize flags on operators like this ...)
	
	// Comparison:
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in Vector3 a, in Vector3 b ) => ( a.v == b.v ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in Vector3 a, in Vector3 b ) => ( a.v != b.v ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in Vector3 a, in SysVec3 b ) => ( a.v == b ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in Vector3 a, in SysVec3 b ) => ( a.v != b ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in SysVec3 a, in Vector3 b ) => ( a == b.v ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in SysVec3 a, in Vector3 b ) => ( a != b.v ) ;
	
	// to Vector3:
	public static implicit operator Vector3( in Vector2 vec ) => new( vec ) ;
	public static implicit operator Vector3( in SysVec2 vec ) => new( vec ) ;
	public static implicit operator Vector3( in SysVec3 vec ) => new( vec ) ;
	public static explicit operator Vector3( in SysVec4 vec ) => new( vec.X, vec.Y, vec.Z ) ;
	public static implicit operator Vector3( in (float x, float y, float z) tuple ) => new( tuple ) ;

	// Vector3 >> SysVec:
	public static implicit operator SysVec3( in Vector3 vec ) => vec.v ;
	public static explicit operator SysVec2( in Vector3 vec ) => new( vec.X, vec.Y ) ;
	
	// Unary operators:
	public static Vector3 operator +( in Vector3 vec ) => vec ;
	public static Vector3 operator -( in Vector3 vec ) => -( vec.v ) ;
	
	// Addition:
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator +( in Vector3 a, SysVec2 b ) => a + (Vector3)b ;
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator +( in Vector3 a, Vector2 b ) => a.v + (SysVec3)b ;

	[MethodImpl( _MAXOPT_ )] public static Vector3 operator +( in Vector3 a, in Vector3 b ) => a.v + b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator +( in Vector3 a, in SysVec3 b ) => a.v + b ;
	
	
	// Subtraction:
	public static Vector3 operator -( in Vector3 a, Vector2 b ) => a.v - (SysVec3)b ;
	public static Vector3 operator -( in Vector3 a, SysVec2 b ) => a.v - ( (Vector3)b ).v ;

	[MethodImpl( _MAXOPT_ )] public static Vector3 operator -( in Vector3 a, in Vector3 b ) => a.v - b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator -( in Vector3 a, in SysVec3 b ) => a.v - b ;


	// Multiplication:
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator *( in Vector3 a, in Vector3 b ) => a.v * b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator *( in Vector3 a, in SysVec3 b ) => a.v * b ;
	public static Vector3 operator *( in Vector3 a, SysVec2 b ) => a.v * new SysVec3( b, 1f ) ;
	public static Vector3 operator *( in Vector3 a, Vector2 b ) => a * new SysVec3( b, 1f ) ;
	
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator *( in Vector3 vec, float value ) => vec.v * value ;
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator *( float value, in Vector3 vec ) => value * vec.v ;
	

	// Division:
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator /( Vector3 vec, float value ) => vec.v / value ;
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator /( in Vector3 a, Vector3 b ) => a.v / b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector3 operator /( in Vector3 a, SysVec3 b ) => a.v / b ;
	public static Vector3 operator /( in Vector3 a, Vector2 b ) => a.v / new SysVec3(b, 1f) ;
	public static Vector3 operator /( in Vector3 a, SysVec2 b ) => a.v / new SysVec3( b, 1f ) ;
	
	#endregion
} ;



/// <summary>A general purpose 4D Euclidean vector.</summary>
/// <remarks>
/// Utilizes <see cref="System.Numerics"/> vector operations for SIMD instruction enhancements.
/// </remarks>
[DebuggerDisplay("ToString()")]
[StructLayout(LayoutKind.Explicit, Size = SizeInBytes)]
public partial struct Vector4: IEquatable< Vector4 >,
							   IEquatable< SysVec4 >,
							   IFormattable {
	#region Const & Readonly Values
	public const int ComponentCount = 4,
					 ComponentSize  = sizeof( float ),
					 SizeInBytes    = ComponentSize * 4 ;

	/// <summary>A vector with all components set to 1.0f </summary>
	public static readonly Vector4 Zero = new( 0f, 0f, 0f, 0f ) ;
	
	/// <summary>The Unit X vector (right).</summary>
	public static readonly Vector4 UnitX = new( 1.0f ) ;
	/// <summary>The Unit Y vector (up).</summary>
	public static readonly Vector4 UnitY = new( y: 1.0f ) ;
	/// <summary>The Unit Z vector (forward).</summary>
	public static readonly Vector4 UnitZ = new( z: 1.0f ) ;
	/// <summary>The Unit W vector (up).</summary>
	public static readonly Vector4 UnitW = new( w: 1.0f ) ;
	
	/// <summary>A vector with all components set to 1.0f </summary>
	public static readonly Vector4 One = new( 1.0f, 1.0f, 1.0f, 1.0f ) ;
	
	/// <summary>A vector with all components set to NaN.</summary>
	/// <remarks>Useful for debugging.</remarks>
	public static readonly Vector4 NaN = new( float.NaN, float.NaN, float.NaN, float.NaN ) ;
	#endregion

	
	[FieldOffset(0)] internal SysVec4 v = default ;
	[FieldOffset(ComponentSize * 0)] internal float   x ;
	[FieldOffset(ComponentSize * 1)] internal float   y ;
	[FieldOffset(ComponentSize * 2)] internal float   z ;
	[FieldOffset(ComponentSize * 3)] internal float   w ;
	
	internal SysVec4 V => v ;

	[UnscopedRef] internal unsafe ref SysVec4 VectorRef {
		get {
			fixed(Vector4* ptr = &this) {
				return ref *( (SysVec4 *)ptr ) ;
			}
		}
	}
	
	
	/// <summary>Gets reference to the specified component of the vector.</summary>
	/// <param name="index">The index of the component to get a reference to.</param>
	public ref float this[ int index ] {
		[MethodImpl(_MAXOPT_)] get {
			unsafe { fixed ( Vector4* ptr = &this ) {
					float* valuePtr = (float*)ptr ;
					return ref valuePtr[ index ] ;
				}
			}
		}
	}
	
	/// <summary>Reference to the X component of the vector.</summary>
	public ref float X => ref this[ 0 ] ;
	/// <summary>Reference to the Y component of the vector.</summary>
	public ref float Y => ref this[ 1 ] ;
	/// <summary>Reference to the Z component of the vector.</summary>
	public ref float Z => ref this[ 2 ] ;
	/// <summary>Reference to the W component of the vector.</summary>
	public ref float W => ref this[ 3 ] ;


	/// <summary>
	/// Creates a new 4D vector
	/// </summary>
	/// <param name="x">X-coordinate</param>
	/// <param name="y">Y-coordinate</param>
	/// <param name="z">Z-coordinate</param>
	/// <param name="w">W-coordinate</param>
	public Vector4( float x = 0f, float y = 0f, float z = 0f, float w = 0f ) {
		Unsafe.SkipInit( out this ) ;
		this.v = new( x, y, z, w ) ;
	}

	/// <summary>Creates a new 4D vector.</summary>
	/// <param name="vec">A System.Numerics.Vector4 value</param>
	public Vector4( in SysVec4 vec ) {
		Unsafe.SkipInit( out this ) ;
		this.v = vec ;
	}

	/// <summary>Creates a new 4D vector.</summary>
	/// <param name="vec">A 3D vector.</param>
	/// <param name="w">An (optional) W coordinate value.</param>
	public Vector4( in SysVec3 vec, float w = 0.0f ) {
		Unsafe.SkipInit( out this ) ;
		this.v = new( vec, w ) ;
	}
	
	/// <summary>Creates a new 4D vector.</summary>
	/// <param name="vec">A Vector2 value</param>
	/// <param name="z">Z-coordinate</param>
	/// <param name="w">W-coordinate</param>
	public Vector4( Vector2 vec, float z = 0f, float w = 0f ) {
		Unsafe.SkipInit( out this ) ;
		this.v = new( vec.X, vec.Y, z, w ) ;
	}
	
	/// <summary>Creates a new 4D vector.</summary>
	/// <param name="a">A <see cref="Vector2"/> value for the first (X, Y) components.</param>
	/// <param name="b">A <see cref="Vector2"/> value for the first (Z, W) components.</param>
	public Vector4( Vector2 a, Vector2 b ) {
		Unsafe.SkipInit( out this ) ;
		this.v = new( a.X, a.Y, b.X, b.Y ) ;
	}

	public Vector4( in (float x, float y, float z, float w) tuple ) {
		Unsafe.SkipInit( out this ) ;
		this.v = new( tuple.x, tuple.y, tuple.z, tuple.w ) ;
	}
	
	
	/// <summary>Interprets this vector as a span of floats.</summary>
	/// <returns>
	/// A span of floats with a length equal to the number of components in the vector (i.e., 3 floats).
	/// </returns>
	[UnscopedRef] public Span< float > AsSpan( ) => 
		MemoryMarshal.CreateSpan( ref x, ComponentCount ) ;
	
	
	/// <summary>
	/// Gets the string representation of this vector value
	/// </summary>
	/// <returns>Vector in string form</returns>
	public override string ToString( ) => ToString( format: null, formatProvider: null ) ;
	
	/// <inheritdoc />
	public string ToString( string? format, IFormatProvider? formatProvider ) {
		if( format is null && formatProvider is null )
			return $"{nameof(Vector4)} [ {X:G}, {Y:G}, {Z:G}, {W:G} ]" ;
		
		return $"{nameof(Vector4)} [ " +
			   $"{nameof( X )} = {X.ToString( format, formatProvider )}, " +
			   $"{nameof( Y )} = {Y.ToString( format, formatProvider )}, " +
			   $"{nameof( Z )} = {Z.ToString( format, formatProvider )}, " +
			   $"{nameof( W )} = {W.ToString( format, formatProvider )} ]" ;
	}

	
	/// <summary>
	/// Indicates whether this instance and the specified object are equal
	/// </summary>
	/// <param name="obj">Object to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public override bool Equals( [NotNullWhen( true )] object? obj ) =>
		(obj is Vector4 vec) ? vec.v == this.v : base.Equals( obj ) ;
	
	/// <summary>
	/// Determines if this vector and the specified vector are equal
	/// </summary>
	/// <param name="other">Vector to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public bool Equals( Vector4 other ) => this.v == other.v ;
	
	/// <summary>
	/// Determines if this vector and the specified vector are equal
	/// </summary>
	/// <param name="other">Vector to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public bool Equals( SysVec4 other ) => this.v == other ;
	
	/// <summary>Returns the hash code this this vector</summary>
	/// <returns>32-bit integer hash code</returns>
	public override int GetHashCode( ) => v.GetHashCode( ) ;
	
	// -----------------------------------------------------------------------------------------------

	public static Vector4 Add( in Vector4 a, in Vector4 b ) => a.v + b.v ;
	public static Vector4 Add( in Vector4 a, in SysVec4 b ) => a.v + b ;
	public static Vector4 Add( in SysVec4 a, in Vector4 b ) => a + b.v ;
	
	public static Vector4 Subtract( in Vector4 a, in Vector4 b ) => a.v - b.v ;
	public static Vector4 Subtract( in Vector4 a, in SysVec4 b ) => a.v - b ;
	public static Vector4 Subtract( in SysVec4 a, in Vector4 b ) => a - b.v ;
	
	public static Vector4 Multiply( in Vector4 a, in Vector4 b ) => a.v * b.v ;
	public static Vector4 Multiply( in Vector4 a, in SysVec4 b ) => a.v * b ;
	public static Vector4 Multiply( in SysVec4 a, in Vector4 b ) => a * b.v ;
	public static Vector4 Multiply( in Vector4 vec, float value ) => vec.v * value ;
	public static Vector4 Multiply( float value, in Vector4 vec ) => value * vec.v ;
	 
	public static Vector4 Divide( in Vector4 a, in Vector4 b ) => a.v / b.v ;
	public static Vector4 Divide( in Vector4 a, in SysVec4 b ) => a.v / b ;
	public static Vector4 Divide( in SysVec4 a, in Vector4 b ) => a / b.v ;
	public static Vector4 Divide( in Vector4 vec, float value ) => vec.v / value ;
	
	public static Vector4 Negate( in Vector4 vec ) => -vec.v ;
	
	public static Vector4 Normalize( in Vector4 vec ) => SysVec4.Normalize( vec.v ) ;
	
	public static float Dot( in Vector4 a, in Vector4 b ) => SysVec4.Dot( a.v, b.v ) ;
	
	public static float Distance( in Vector4 a, in Vector4 b ) => SysVec4.Distance( a.v, b.v ) ;
	
	public static float DistanceSquared( in Vector4 a, in Vector4 b ) => SysVec4.DistanceSquared( a.v, b.v ) ;
	
	public static float Length( in Vector4 vec ) => vec.v.Length( ) ;
	
	public static float LengthSquared( in Vector4 vec ) => vec.v.LengthSquared( ) ;
	
	public static Vector4 Lerp( in Vector4 a, in Vector4 b, float t ) => SysVec4.Lerp( a.v, b.v, t ) ;
	
	public static Vector4 Clamp( in Vector4 vec, in Vector4 min, in Vector4 max ) =>
		SysVec4.Clamp( vec.v, min.v, max.v ) ;
	
	public static Vector4 Clamp( in Vector4 vec, float min, float max ) =>
	 		SysVec4.Clamp( vec.v, new( min ), new( max ) ) ;
	
	public static Vector4 Transform( in Vector4 vec, in Matrix4x4 mat ) =>
		SysVec4.Transform( vec.v, mat ) ;
	
	public static Vector4 Transform( in Vector4 vec, in Quaternion quat ) =>
		SysVec4.Transform( vec.v, quat ) ;
	
	// -----------------------------------------------------------------------------------------------
	
	#region Operator Overloads
	
	// Comparison:
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in Vector4 a, in Vector4 b ) => ( a.v == b.v ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in Vector4 a, in Vector4 b ) => ( a.v != b.v ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in Vector4 a, in SysVec4 b ) => ( a.v == b ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in Vector4 a, in SysVec4 b ) => ( a.v != b ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator ==( in SysVec4 a, in Vector4 b ) => ( a == b.v ) ;
	[MethodImpl( _MAXOPT_ )] public static bool operator !=( in SysVec4 a, in Vector4 b ) => ( a != b.v ) ;
	 
	// to Vector4:
	public static implicit operator Vector4( Vector2 vec ) => new( vec ) ;
	public static implicit operator Vector4( SysVec2 vec ) => new( vec ) ;
	public static implicit operator Vector4( in SysVec3 vec ) => new( vec ) ;
	public static implicit operator Vector4( in Vector3 vec ) => new( vec ) ;
	public static implicit operator Vector4( in SysVec4 vec ) => new( vec ) ;
	public static implicit operator Vector4( in (Vector2 a, Vector2 b) tuple ) => new( tuple.a, tuple.b ) ;
	public static implicit operator Vector4( in (Vector3 a, float w) tuple ) => new( tuple.a, tuple.w ) ;
	public static implicit operator Vector4( in (float x, float y, float z, float w) tuple ) => new( tuple ) ;
	
	public static explicit operator Vector3( in Vector4 vec ) => new( vec.v.X, vec.v.Y, vec.v.Z ) ;
	public static explicit operator Vector2( in Vector4 vec ) => new( vec.v.X, vec.v.Y ) ;
	
	// Vector4 >> SysVec:
	public static implicit operator SysVec4( in Vector4 vec ) => vec.v ;
	public static explicit operator SysVec2( in Vector4 vec ) => new( vec.X, vec.Y ) ;
	public static explicit operator SysVec3( in Vector4 vec ) => new( vec.X, vec.Y, vec.Z ) ;
	
	// Unary operators:
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator +( in Vector4 vec ) => vec ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator -( in Vector4 vec ) => -( vec.v ) ;
	 
	// Addition:
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator +( in Vector4 a, in SysVec4 b ) => a.v + b ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator +( in Vector4 a, in Vector4 b ) => a.v + b.v ;
	
	public static Vector4 operator +( in Vector4 a, in SysVec3 b ) => a.v + new SysVec4( b, 0f ) ;
	public static Vector4 operator +( in Vector4 a, in Vector3 b ) => a.v + new SysVec4( b, 0f ) ;
	public static Vector4 operator +( in Vector4 a, SysVec2 b ) => a + new SysVec4( b, 0f, 0f ) ;
	public static Vector4 operator +( in Vector4 a, Vector2 b ) => a.v + new SysVec4( b, 0f, 0f ) ;
	
	 
	// Subtraction:
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator -( in Vector4 a, in Vector4 b ) => a.v - b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator -( in Vector4 a, in SysVec4 b ) => a.v - b ;
	
	public static Vector4 operator -( in Vector4 a, SysVec3 b ) => a.v - new SysVec4( b, 0f ) ;
	public static Vector4 operator -( in Vector4 a, Vector3 b ) => a.v - new SysVec4( b, 0f ) ;
	public static Vector4 operator -( in Vector4 a, SysVec2 b ) => a - new SysVec4( b, 0f, 0f )  ;
	public static Vector4 operator -( in Vector4 a, Vector2 b ) => a.v - new SysVec4( b, 0f, 0f )  ;
	
	
	// Multiplication:
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator *( in Vector4 vec, float value ) => vec.v * value ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator *( float value, in Vector4 vec ) => value * vec.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator *( in Vector4 a, in Vector4 b ) => a.v * b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator *( in Vector4 a, in SysVec4 b ) => a.v * b ;
	
	public static Vector4 operator *( in Vector4 a, SysVec2 b ) => a * new SysVec4( b, 1f, 1f ) ;
	public static Vector4 operator *( in Vector4 a, Vector2 b ) => a.v * new SysVec4( b, 1f, 1f ) ;
	
	public static Vector4 operator *( in Vector4 a, SysVec3 b ) => a.v * new SysVec4( b, 1f ) ;
	public static Vector4 operator *( in Vector4 a, Vector3 b ) => a.v * new SysVec4( b, 1f ) ;
	
	
	// Division:
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator /( in Vector4 a, float b ) => a.v / b ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator /( in Vector4 a, in Vector4 b ) => a.v / b.v ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator /( in Vector4 a, in SysVec4 b ) => a.v / b ;
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator /( in Vector4 a, in SysVec3 b ) => a / (Vector4)b ;
	
	// Matrix Operations:
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator *( in Vector4 vec, in Matrix4x4 mat ) =>
		SysVec4.Transform( vec.v, mat ) ;
	
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator *( in Matrix4x4 mat, in Vector4 vec ) =>
	 		SysVec4.Transform( vec.v, mat ) ;
	
	// Quaternion Operations:
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator *( in Vector4 vec, in Quaternion quat ) =>
		SysVec4.Transform( vec.v, quat ) ;
	
	[MethodImpl( _MAXOPT_ )] public static Vector4 operator *( in Quaternion quat, in Vector4 vec ) =>
	 		SysVec4.Transform( vec.v, quat ) ;
	
	#endregion
	
	// ===============================================================================================
} ;