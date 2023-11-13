#region Using Directives
using System.Numerics ;
using System.Runtime.CompilerServices ;

using DxMath = DXSharp.Math ;
using SysMath = System.Math ;
using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp ;


/// <summary>
/// Defines mathematical constants, functions and other operations
/// which are useful when performing mathematical operations in a
/// real-time DirectX application built with <see cref="DXSharp"/>.
/// </summary>
public static partial class Mathf {
	// ------------------------------------------------------------------------------------------------------------
	#region Const/ReadOnly Values:
	static readonly Half HalfEpisolon8x = Half.Epsilon * (Half)8 ;
	
	/// <summary>
	/// Represents the smallest positive <see cref="float"/> value
	/// that is greater than zero.
	/// </summary>
	/// <returns><c><value>1E-45F</value></c></returns>
	/// <remarks>This field is constant.</remarks>
	public const float Epsilon = float.Epsilon ;
	
	/// <summary>
	/// Represents the smallest positive <see cref="Half"/> value
	/// that is greater than zero.
	/// </summary>
	/// <returns><c><value>5.9604645E-08</value></c></returns>
	public static readonly Half HalfEpsilon = Half.Epsilon ;
	#endregion
	// ------------------------------------------------------------------------------------------------------------
	
	
	/// <summary>
	/// Returns the greater <see cref="Half"/> value of <paramref name="a"/> and <paramref name="b"/>.
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	[MethodImpl(_MAXOPT_)] public static Half Max( Half a, Half b ) => a >= b ? a : b ;
	[MethodImpl(_MAXOPT_)] public static Half Min( Half a, Half b ) => a <= b ? a : b ;
	[MethodImpl(_MAXOPT_)] public static float Max( float a, float b ) => a >= b ? a : b ;
	[MethodImpl(_MAXOPT_)] public static float Min( float a, float b ) => a <= b ? a : b ;
	[MethodImpl(_MAXOPT_)] public static double Max( double a, double b ) => a >= b ? a : b ;
	[MethodImpl(_MAXOPT_)] public static double Min( double a, double b ) => a <= b ? a : b ;
	[MethodImpl(_MAXOPT_)] public static decimal Max( decimal a, decimal b ) => a >= b ? a : b ;
	[MethodImpl(_MAXOPT_)] public static decimal Min( decimal a, decimal b ) => a <= b ? a : b ;
	
	public static T Max< T >( T a, T b ) where T: INumber< T >,
									 IComparisonOperators< T, T, bool > => a > b ? a : b ;
	
	public static T Min< T >( T a, T b ) where T: INumber< T >,
	 									 IComparisonOperators< T, T, bool > => a < b ? a : b ;
	
	
	// -----------------------------------------------------
	// Approximate Floating-Point Equality:
	// -----------------------------------------------------
	
	/// <summary>
	/// Indicates if two floating point values are approximately equal.
	/// </summary>
	/// <param name="a">The first floating-point value.</param>
	/// <param name="b">The second floating-point value.</param>
	/// <returns></returns>
	/// <remarks><para>Uses <see cref="Epsilon"/> as the tolerance.</para></remarks>
	[MethodImpl(_MAXOPT_)] public static bool Approximately( Half a, Half b ) =>
		Half.Abs(b - a) < Max( (Half)0.0001f * Max( Half.Abs(a), Half.Abs(b) ), HalfEpisolon8x ) ;
	
	/// <summary>
	/// Indicates if two floating point values are approximately equal.
	/// </summary>
	/// <param name="a">The first floating-point value.</param>
	/// <param name="b">The second floating-point value.</param>
	/// <returns></returns>
	/// <remarks><para>Uses <see cref="Epsilon"/> as the tolerance.</para></remarks>
	[MethodImpl(_MAXOPT_)] public static bool Approximately( float a, float b ) => 
		SysMath.Abs(b - a) < SysMath.Max( 0.000001f * SysMath.Max( SysMath.Abs(a), SysMath.Abs(b) ), Epsilon * 8 ) ;
	
	
	[MethodImpl(_MAXOPT_)] public static bool Approximately( in Vector2 a, in Vector2 b ) => 
		Approximately( a.X, b.X ) && Approximately( a.Y, b.Y ) ;
	
	[MethodImpl(_MAXOPT_)] public static bool Approximately( in Vector3 a, in Vector3 b ) =>
	 		Approximately( a.X, b.X ) && Approximately( a.Y, b.Y ) && Approximately( a.Z, b.Z ) ;
	
	[MethodImpl(_MAXOPT_)] public static bool Approximately( in Vector4 a, in Vector4 b ) =>
	 		Approximately( a.X, b.X ) && Approximately( a.Y, b.Y ) && Approximately( a.Z, b.Z ) && 
	 		Approximately( a.W, b.W ) ;
	
	[MethodImpl(_MAXOPT_)] public static bool Approximately( in Quaternion a, in Quaternion b ) =>
	 		Approximately( a.X, b.X ) && Approximately( a.Y, b.Y )
			&& Approximately( a.Z, b.Z ) && Approximately( a.W, b.W ) ;
	
	[MethodImpl(_MAXOPT_)] public static bool Approximately( in Matrix4x4 a, in Matrix4x4 b ) =>
	 		Approximately( a.M11, b.M11 ) && Approximately( a.M12, b.M12 ) && Approximately( a.M13, b.M13 ) && Approximately( a.M14, b.M14 ) &&
	 		Approximately( a.M21, b.M21 ) && Approximately( a.M22, b.M22 ) && Approximately( a.M23, b.M23 ) && Approximately( a.M24, b.M24 ) &&
	 		Approximately( a.M31, b.M31 ) && Approximately( a.M32, b.M32 ) && Approximately( a.M33, b.M33 ) && Approximately( a.M34, b.M34 ) &&
	 		Approximately( a.M41, b.M41 ) && Approximately( a.M42, b.M42 ) && Approximately( a.M43, b.M43 ) && Approximately( a.M44, b.M44 ) ;
	
	[MethodImpl(_MAXOPT_)] public static bool Approximately( in Plane a, in Plane b ) =>
	 		Approximately( a.Normal, b.Normal ) && Approximately( a.D, b.D ) ;

	
	[MethodImpl(_MAXOPT_)] 
	public static bool Approximately< T >( in T a, in T b ) 
										where T: INumber< T >, IFloatingPoint< T >,
														IFloatingPointIeee754< T >, 
												  IBinaryFloatingPointIeee754< T >,
												 IMultiplyOperators< T, float, T > {
		T maxVal     = T.Max( T.Abs(a), T.Abs(b) ) ;
		T absDiff    = T.Abs( b - a ) ;
		T epsilon    = T.Epsilon ;
		T epsilon8   = epsilon * 8.0f ;
		T maxEpsilon = T.Max( maxVal * 0.000001f, epsilon8 ) ;
		return absDiff < maxEpsilon ;
	}
	
	
	// -----------------------------------------------------
	// (to be continued ...):
	// -----------------------------------------------------
	
	
	
	// ============================================================================================================
} ;
	



	/*public static bool Approximately< T >(in T a, in T b) where T : IFloatingPoint<T>, 
		IFloatingPointIeee754< T >, 
		IBinaryFloatingPointIeee754< T > {
		var epsilon     = T.Epsilon;
		var epsilon8x   = ( epsilon * T.ScaleB(T.One, 8) ) ;
		var maxAB       = T.Max(T.Abs(a), T.Abs(b));
		var minEpsMaxAB = T.Min( (epsilon8x * maxAB), epsilon8x ) ;
		var diffAB      = T.Abs( a - b ) ;

		return diffAB < minEpsMaxAB ;
	}*/
	/*public static bool Approximately< T >( in T a, in T b ) where T: IFloatingPoint< T >, 
																IFloatingPointIeee754< T >, 
															IBinaryFloatingPointIeee754< T > {
		
		var epsilon = T.Epsilon ;
		return T.Abs(b - a) < T.Max( T.Radix( 0.000001f ) * T.Max( T.Abs(a), T.Abs(b) ), epsilon * 8 ) ;
	}*/
	
	