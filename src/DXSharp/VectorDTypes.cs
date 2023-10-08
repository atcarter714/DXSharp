
#region Using Directives
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

using SysVec4 = System.Numerics.Vector4;
#endregion


namespace DXSharp;


/// <summary>
/// A 4-dimensional double-precision floating-point vector with
/// SIMD-enabled arithmetic and operations
/// </summary>
/// <remarks>
/// <h3><b>NOTE:</b></h3>
/// <para>
/// This structure type is only available for .NET 7 for Windows
/// </para>
/// </remarks>
public struct Vector4D: IEquatable<Vector4D>, IFormattable
{
	#region Const & Readonly Values

	/// <summary>
	/// Number of items in the vector
	/// </summary>
	const int nITEMS = 0x4;

	const int X_INDEX = 0x0;
	const int Y_INDEX = 0x1;
	const int Z_INDEX = 0x2;
	const int W_INDEX = 0x3;

	/// <summary>
	/// A vector with all zero values
	/// </summary>
	public static readonly Vector4D Zero = new( Vector256<double>.Zero );
	/// <summary>
	/// A vector with all 1.0 values
	/// </summary>
	public static readonly Vector4D One = new( 1, 1, 1, 1 );
	/// <summary>
	/// The Unit X vector (right)
	/// </summary>
	public static readonly Vector4D UnitX = new( 1.0f );
	/// <summary>
	/// The Unit Y vector (up) 
	/// </summary>
	public static readonly Vector4D UnitY = new( y: 1.0f );
	/// <summary>
	/// The Unit Z vector (forward) 
	/// </summary>
	public static readonly Vector4D UnitZ = new( z: 1.0f );
	/// <summary>
	/// The Unit W vector (forward) 
	/// </summary>
	public static readonly Vector4D UnitW = new( w: 1.0f );

	#endregion Const & Readonly Values


	/// <summary>
	/// Creates a new Vector4D
	/// </summary>
	/// <param name="vec">A System.Numerics.Vector256</param>
	public Vector4D( Vector256<double> vec ) => this.v = vec;
	/// <summary>
	/// Creates a new Vector4D
	/// </summary>
	/// <param name="lower">A System.Numerics.Vector128 for the lower memory chunk</param>
	/// <param name="upper">A System.Numerics.Vector128 for the upper memory chunk</param>
	public Vector4D( Vector128<double> lower, Vector128<double> upper ) => SetMem( lower, upper );
	/// <summary>
	/// Creates a new Vector4D
	/// </summary>
	/// <param name="vec">A System.Numerics.Vector4</param>
	/// <remarks>
	/// The Vector4 will be expanded to a double-precision vector
	/// </remarks>
	public Vector4D( SysVec4 vec ) {
		SetMem( X_INDEX, vec.X );
		SetMem( Y_INDEX, vec.Y );
		SetMem( Z_INDEX, vec.Z );
		SetMem( W_INDEX, vec.W );
	}
	/// <summary>
	/// Creates a new Vector4D
	/// </summary>
	/// <param name="x">The X coordinate/value</param>
	/// <param name="y">The Y coordinate/value</param>
	/// <param name="z">The Z coordinate/value</param>
	/// <param name="w">The W coordinate/value</param>
	public Vector4D( double x = 0, double y = 0, double z = 0, double w = 0 ) {
		SetMem( X_INDEX, x );
		SetMem( Y_INDEX, y );
		SetMem( Z_INDEX, z );
		SetMem( W_INDEX, w );
	}
	/// <summary>
	/// Creates a new Vector4D
	/// </summary>
	/// <param name="values">Tuple of the X and Y values</param>
	public Vector4D( (double x, double y) values ) :
	this( values.x, values.y, 0, 0 ) { }
	/// <summary>
	/// Creates a new Vector4D
	/// </summary>
	/// <param name="values">Tuple of the X, Y and Z values</param>
	public Vector4D( (double x, double y, double z) values ) :
		this( values.x, values.y, values.z, 0 ) { }
	/// <summary>
	/// Creates a new Vector4D
	/// </summary>
	/// <param name="values">Tuple of the X, Y, Z and W values</param>
	public Vector4D( (double x, double y, double z, double w) values ) :
		this( values.x, values.y, values.z, values.w ) { }



	Vector256< double > v = default;
	internal Vector256< double > V => v;

	/// <summary>
	/// Gets or sets the element at the specified index
	/// </summary>
	/// <param name="index">Index/offset</param>
	/// <returns>Element of the vector</returns>
	/// <exception cref="IndexOutOfRangeException">
	/// Thrown if the given index is out of range
	/// </exception>
	public double this[ int index ] {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			return index switch
			{
				0 => v.GetElement( X_INDEX ),
				1 => v.GetElement( Y_INDEX ),
				2 => v.GetElement( Z_INDEX ),
				3 => v.GetElement( W_INDEX ),
				_ => throw new IndexOutOfRangeException( $"Vector4D[ {index} ] Accessor: " +
										$"The index {index} is not within the valid range of 0 to {nITEMS}!" ),
			};
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		set {
			switch( index ) {
				case 0: SetMem( X_INDEX, value ); break;
				case 1: SetMem( Y_INDEX, value ); break;
				case 2: SetMem( Z_INDEX, value ); break;
				case 3: SetMem( W_INDEX, value ); break;
				default:
					throw new IndexOutOfRangeException( $"Vector4D[ {index} ] Accessor: " +
						$"The index {index} is not within the valid range of 0 to {nITEMS}!" );
			}
		}
	}


	/// <summary>
	/// The X component of the vector
	/// </summary>
	public double X { get => v.GetElement( X_INDEX ); set => SetMem( X_INDEX, value ); }
	/// <summary>
	/// The Y component of the vector
	/// </summary>
	public double Y { get => v.GetElement( Y_INDEX ); set => SetMem( Y_INDEX, value ); }
	/// <summary>
	/// The Z component of the vector
	/// </summary>
	public double Z { get => v.GetElement( Z_INDEX ); set => SetMem( Z_INDEX, value ); }
	/// <summary>
	/// The W component of the vector
	/// </summary>
	public double W { get => v.GetElement( W_INDEX ); set => SetMem( W_INDEX, value ); }



	/// <inheritdoc />
	public override string ToString() => ToString( format: null, formatProvider: null );

	/// <inheritdoc />
	public string ToString( string? format, IFormatProvider? formatProvider ) =>
		$"{this.GetType().Name} {{ " +
		$"{nameof( X )} = {X.ToString( format, formatProvider )}, " +
		$"{nameof( Y )} = {Y.ToString( format, formatProvider )}, " +
		$"{nameof( Z )} = {Z.ToString( format, formatProvider )}, " +
		$"{nameof( W )} = {W.ToString( format, formatProvider )} }}";

	/// <inheritdoc />
	public override bool Equals( [NotNullWhen( true )] object? obj ) =>
		obj is Vector4D vec ? vec.v == this.v : base.Equals( obj );

	/// <inheritdoc />
	public bool Equals( Vector4D other ) => other.v == this.v;

	/// <inheritdoc />
	public override int GetHashCode() => v.GetHashCode();



	#region Internal Helper Methods

	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal void SetMem( int offset, double value ) {
#if DEBUG || !STRIP_CHECKS
		if( offset is < 0 or > nITEMS ) {
			throw new IndexOutOfRangeException( $"WriteMem({offset}, {value}): " +
				$"The specified memory offset ({offset}) is out of range!" );
		}
#endif
		unsafe {
			fixed( Vector256<double>* ptr = &v ) {
				double* pItems = (double*)ptr;
				*(pItems + offset) = value;
			}
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal void SetMem( Vector128<double> lower, Vector128<double> upper ) {
		unsafe {
			fixed( Vector256<double>* ptr = &v ) {
				Unsafe.Write( (Vector128<double>*)ptr, lower );
				Unsafe.Write( ((Vector128<double>*)ptr) + 0x1, upper );
			}
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal double GetMem( int offset ) => v[ offset ];

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	internal Vector4D NegateAll() => new( v = Vector256.Negate( v ) );

	#endregion


	#region Operator Overloads

	public static bool operator ==( Vector4D a, Vector4D b ) => a.v == b.v;
	public static bool operator !=( Vector4D a, Vector4D b ) => a.v != b.v;


	public static Vector4D operator +( Vector4D vec ) => vec;
	public static Vector4D operator -( Vector4D vec ) => new( Vector256.Negate( vec.v ) );


	public static Vector4D operator +( Vector4D a, Vector4D b ) => a.v + b.v;
	public static Vector4D operator -( Vector4D a, Vector4D b ) => a.v - b.v;
	public static Vector4D operator *( Vector4D a, Vector4D b ) => a.v * b.v;
	public static Vector4D operator /( Vector4D a, Vector4D b ) => a.v / b.v;

	public static Vector4D operator *( Vector4D vec, double value ) => vec.v * value;
	public static Vector4D operator *( double value, Vector4D vec ) => value * vec.v;
	public static Vector4D operator /( Vector4D vec, double value ) => vec.v / Vector256.Create( value );


	public static Vector4D operator +( Vector4D left, Vector256<double> right ) => new( left.v + right );
	public static Vector4D operator -( Vector4D left, Vector256<double> right ) => new( left.v - right );
	public static Vector4D operator *( Vector4D left, Vector256<double> right ) => new( left.v * right );
	public static Vector4D operator /( Vector4D left, Vector256<double> right ) => new( left.v / right );

	public static Vector256<double> operator +( Vector256<double> left, Vector4D right ) => left + right.v;
	public static Vector256<double> operator -( Vector256<double> left, Vector4D right ) => left - right.v;
	public static Vector256<double> operator *( Vector256<double> left, Vector4D right ) => left * right.v;
	public static Vector256<double> operator /( Vector256<double> left, Vector4D right ) => left / right.v;

	public static implicit operator Vector4D( double x ) => new( x );
	public static implicit operator Vector4D( (double x, double y) v ) => new( v );
	public static implicit operator Vector4D( (double x, double y, double z) v ) => new( v );
	public static implicit operator Vector4D( (double x, double y, double z, double w) v ) => new( v );
	public static implicit operator Vector4D( Vector256<double> v ) => new( v );
	public static implicit operator Vector4D( (Vector128<double> lower, Vector128<double> upper) vectors ) =>
		new( vectors.lower, vectors.upper );

	public static implicit operator Vector256<double>( Vector4D vec ) => vec.v;

	#endregion Operator Overloads

};