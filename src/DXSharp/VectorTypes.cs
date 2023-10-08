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
using System.Runtime.Intrinsics ;
using System.Diagnostics.CodeAnalysis ;
using SysVec2 = System.Numerics.Vector2 ;
using SysVec3 = System.Numerics.Vector3 ;
using SysVec4 = System.Numerics.Vector4 ;
#endregion


namespace DXSharp;


/// <summary>
/// A 2D Euclidean vector.
/// </summary>
public struct Vector2 {
	#region Const & Readonly Values
	/// <summary>
	/// The Unit X vector (right)
	/// </summary>
	public static readonly Vector2 UnitX = new( 1f );
	/// <summary>
	/// The Unit Y vector (up) 
	/// </summary>
	public static readonly Vector2 UnitY = new( y: 1f );
	#endregion


	/// <summary>
	/// Creates a new 2D vector
	/// </summary>
	/// <param name="x">X-coordinate</param>
	/// <param name="y">Y-coordinate</param>
	public Vector2( float x = 0f, float y = 0f ) =>
		v = new( x, y );

	/// <summary>
	/// Creates a new 2D vector
	/// </summary>
	/// <param name="vec">A System.Numerics.Vector2 value</param>
	public Vector2( SysVec2 vec ) => this.v = vec;


	SysVec2 v = default ;
	internal SysVec2 V => v;


	/// <summary>
	/// Gets or sets the X component of the vector
	/// </summary>
	public float X { get => v.X; set => v.X = value; }
	/// <summary>
	/// Gets or sets the Y component of the vector
	/// </summary>
	public float Y { get => v.Y; set => v.Y = value; }



	/// <summary>
	/// Gets the string representation of this vector value
	/// </summary>
	/// <returns>Vector in string form</returns>
	public override string ToString() => $"Vec2[{v.X}, {v.Y}]";

	/// <summary>
	/// Indicates whether this vector and the specified object are equal
	/// </summary>
	/// <param name="obj">Object to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public override bool Equals( [NotNullWhen( true )] object? obj ) =>
		(obj is Vector2 vec) ? vec == this : base.Equals( obj );

	/// <summary>
	/// Returns the hash code this this vector
	/// </summary>
	/// <returns>32-bit integer hash code</returns>
	public override int GetHashCode() => v.GetHashCode();



	// Comparison:
	public static bool operator ==( Vector2 a, Vector2 b ) => (a.v == b.v);
	public static bool operator !=( Vector2 a, Vector2 b ) => (a.v != b.v);
	public static bool operator ==( Vector2 a, SysVec2 b ) => (a.v == b);
	public static bool operator !=( Vector2 a, SysVec2 b ) => (a.v != b);
	public static bool operator ==( SysVec2 a, Vector2 b ) => (a == b.v);
	public static bool operator !=( SysVec2 a, Vector2 b ) => (a != b.v);


	// to Vector2:
	public static implicit operator Vector2( SysVec2 vec ) => new( vec );
	public static explicit operator Vector2( SysVec3 vec ) => new( vec.X, vec.Y );
	public static explicit operator Vector2( SysVec4 vec ) => new( vec.X, vec.Y );

	public static explicit operator Vector2( Vector3 vec ) => new( vec.X, vec.Y );


	// Vector2 >> SysVec:
	public static implicit operator SysVec2( Vector2 vec ) => vec.v;
	public static implicit operator SysVec3( Vector2 vec ) => new( vec.X, vec.Y, 0f );
} ;



/// <summary>
/// A 3D Euclidean vector.
/// </summary>
public struct Vector3: IEquatable< Vector3 >, IFormattable {
	#region Const & Readonly Values
	/// <summary>
	/// The Unit X vector (right)
	/// </summary>
	public static readonly Vector3 UnitX = new( 1.0f );
	/// <summary>
	/// The Unit Y vector (up) 
	/// </summary>
	public static readonly Vector3 UnitY = new( y: 1.0f );
	/// <summary>
	/// The Unit Z vector (forward) 
	/// </summary>
	public static readonly Vector3 UnitZ = new( z: 1.0f );
	#endregion


	/// <summary>
	/// Creates a new 3D vector
	/// </summary>
	/// <param name="x">X-coordinate</param>
	/// <param name="y">Y-coordinate</param>
	/// <param name="z">Z-coordinate</param>
	public Vector3( float x = 0f, float y = 0f, float z = 0f ) =>
		this.v = new( x, y, z );

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
	public Vector3( SysVec2 vec ) => this.v = new( vec.X, vec.Y, 0f );

	public Vector3( Vector128<float> v128 ) => this.v = v128.AsVector3();


	SysVec3 v = default;
	internal SysVec3 V => v;


	/// <summary>
	/// Gets or sets the X component of the vector
	/// </summary>
	public float X { get => v.X; set => v.X = value; }
	/// <summary>
	/// Gets or sets the Y component of the vector
	/// </summary>
	public float Y { get => v.Y; set => v.Y = value; }
	/// <summary>
	/// Gets or sets the Z component of the vector
	/// </summary>
	public float Z { get => v.Z; set => v.Z = value; }



	/// <summary>
	/// Gets the string representation of this vector value
	/// </summary>
	/// <returns>Vector in string form</returns>
	public override string ToString() => ToString( format: null, formatProvider: null );

	/// <inheritdoc />
	public string ToString( string? format, IFormatProvider? formatProvider ) =>
		$"{this.GetType().Name} {{ " +
		$"{nameof( X )} = {X.ToString( format, formatProvider )}, " +
		$"{nameof( Y )} = {Y.ToString( format, formatProvider )}, " +
		$"{nameof( Z )} = {Z.ToString( format, formatProvider )} }}";

	/// <summary>
	/// Indicates whether this instance and the specified object are equal
	/// </summary>
	/// <param name="obj">Object to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public override bool Equals( [NotNullWhen( true )] object? obj ) =>
		(obj is Vector3 vec) ? vec == this : base.Equals( obj );

	/// <summary>
	/// Returns the hash code this this vector
	/// </summary>
	/// <returns>32-bit integer hash code</returns>
	public override int GetHashCode() => v.GetHashCode();

	/// <summary>
	/// Determines if this vector and the specified vector are equal
	/// </summary>
	/// <param name="other">Vector to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public bool Equals( Vector3 other ) => this.v == other.v;



	#region Operator Overloads
	//! TODO: Tag all operators with inlining attribute!
	// [MethodImpl(MethodImplOptions.AggressiveInlining)]

	// Comparison:
	public static bool operator ==( Vector3 a, Vector3 b ) => (a.v == b.v);
	public static bool operator !=( Vector3 a, Vector3 b ) => (a.v != b.v);
	public static bool operator ==( Vector3 a, SysVec3 b ) => (a.v == b);
	public static bool operator !=( Vector3 a, SysVec3 b ) => (a.v != b);
	public static bool operator ==( SysVec3 a, Vector3 b ) => (a == b.v);
	public static bool operator !=( SysVec3 a, Vector3 b ) => (a != b.v);


	// to Vector3:
	public static implicit operator Vector3( SysVec2 vec ) => new( vec );
	public static implicit operator Vector3( SysVec3 vec ) => new( vec );
	public static explicit operator Vector3( SysVec4 vec ) => new( vec.X, vec.Y, vec.Z );

	public static implicit operator Vector3( Vector2 vec ) => new( vec );


	// Vector3 >> SysVec:
	public static implicit operator SysVec3( Vector3 vec ) => vec.v;
	public static explicit operator SysVec2( Vector3 vec ) => new( vec.X, vec.Y );


	// Unary operators:
	public static Vector3 operator +( Vector3 vec ) => vec;
	public static Vector3 operator -( Vector3 vec ) => -(vec.v);


	// Addition:
	public static Vector3 operator +( Vector3 a, Vector3 b ) => a.v + b.v;
	public static Vector3 operator +( Vector3 a, Vector2 b ) => a.v + b;
	public static Vector3 operator +( Vector3 a, SysVec3 b ) => a.v + b;
	public static Vector3 operator +( Vector3 a, SysVec2 b ) =>
		a.v + new SysVec3( b.X, b.Y, 0f );


	// Subtraction:
	public static Vector3 operator -( Vector3 a, Vector3 b ) => a.v - b.v;
	public static Vector3 operator -( Vector3 a, Vector2 b ) => a.v - b;
	public static Vector3 operator -( Vector3 a, SysVec3 b ) => a.v - b;
	public static Vector3 operator -( Vector3 a, SysVec2 b ) =>
		a.v - new SysVec3( b.X, b.Y, 0f );


	// Multiplication:
	public static Vector3 operator *( Vector3 a, Vector3 b ) => a.v * b.v;
	public static Vector3 operator *( Vector3 a, Vector2 b ) => a.v * b;
	public static Vector3 operator *( Vector3 a, SysVec3 b ) => a.v * b;
	public static Vector3 operator *( Vector3 a, SysVec2 b ) =>
		a.v * new SysVec3( b.X, b.Y, 1f );

	public static Vector3 operator *( Vector3 vec, float value ) => vec.v * value;
	public static Vector3 operator *( float value, Vector3 vec ) => value * vec.v;


	// Division:
	public static Vector3 operator /( Vector3 a, Vector3 b ) => a.v / b.v;
	public static Vector3 operator /( Vector3 a, Vector2 b ) => a.v / b;
	public static Vector3 operator /( Vector3 a, SysVec3 b ) => a.v / b;
	public static Vector3 operator /( Vector3 a, SysVec2 b ) =>
		a.v / new SysVec3( b.X, b.Y, 1f );

	public static Vector3 operator /( Vector3 vec, float value ) => vec.v / value;


	//! TODO: Matrix ops:
	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static Vector3 operator *( Vector3 left, Matrix4x4 right ) => Transform( left, right );

	#endregion
};


/*
	/// <summary>Transforms a vector using a matrix.</summary>
	/// <param name="value">The vector to transform.</param>
	/// <param name="matrix">The transformation matrix.</param>
	/// <returns><paramref name="value" /> transformed by <paramref name="matrix" />.</returns>
	//public static Vector3 Transform( Vector3 value, Matrix4x4 matrix ) {
	//	var vValue = value.v.AsVector128();

	//	var result = matrix.W.Value;
	//	result = MultiplyAddByX( result, matrix.X.Value, vValue );
	//	result = MultiplyAddByY( result, matrix.Y.Value, vValue );
	//	result = MultiplyAddByZ( result, matrix.Z.Value, vValue );
	//	return Create( result );
	//}
	/// <summary>Transforms a vector using a normalized matrix.</summary>
	/// <param name="value">The vector to transform.</param>
	/// <param name="matrix">The normalized transformation matrix.</param>
	/// <returns><paramref name="value" /> transformed by <paramref name="matrix" />.</returns>
	//public static Vector3 TransformNormal( Vector3 value, Matrix4x4 matrix ) {
	//	var vValue = value.v.AsVector128();

	//	var result = MultiplyByX(matrix.X.Value, vValue);
	//	result = MultiplyAddByY( result, matrix.Y.Value, vValue );
	//	result = MultiplyAddByZ( result, matrix.Z.Value, vValue );
	//	return Create( result );
	//}
	 */