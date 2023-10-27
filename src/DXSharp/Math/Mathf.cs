using System.Numerics ;
using System.Runtime.CompilerServices ;
using static DXSharp.InteropUtils ;
namespace DXSharp ;

public static class Mathf {
	public const float Epsilon = float.Epsilon ;
	public static readonly Half HalfEpsilon = Half.Epsilon ;
	static readonly Half HalfEpisolon8x = Half.Epsilon * (Half)8 ;
	
	
	/// <summary>
	/// Returns the greater <see cref="Half"/> value of <paramref name="a"/> and <paramref name="b"/>.
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	[MethodImpl(_MAXOPT_)] public static Half Max( Half a, Half b ) => a > b ? a : b ;
	
	
	
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
		Math.Abs(b - a) < Math.Max( 0.000001f * Math.Max( Math.Abs(a), Math.Abs(b) ), Epsilon * 8 ) ;
	
	
	
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
	
	
} ;