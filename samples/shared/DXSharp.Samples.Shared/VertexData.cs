#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using DXSharp ;
using Vector3 = DXSharp.Vector3 ;
using Vector4 = DXSharp.Vector4 ;
using static DXSharp.InteropUtils ;
#endregion
namespace BasicSample ;


/// <summary>
/// Represents a simple vertex data type
/// with a position and a color component.
/// </summary>
/// <remarks>
/// A <see cref="VertexPosCol"/> can be created conveniently by
/// making a tuple of the position and color components or
/// of all the floating-point values ...
/// </remarks>
[StructLayout(LayoutKind.Sequential, Size = SizeInBytes)]
public partial struct VertexPosCol: IEquatable< VertexPosCol >,
									IEquatable< Vector3 >,
									IEquatable< Vector4 > {
	#region Constant & ReadOnly Values
	public const int ComponentCount = 7,
					 ComponentSize  = sizeof(float),
					 SizeInBytes    = ComponentCount * ComponentSize ;
	
	/// <summary>Empty vertex with all data initialized to zero.</summary>
	public static readonly VertexPosCol Zero = 
		new( Vector3.Zero, Vector4.Zero ) ;
	
	/// <summary>Empty vertex with all data initialized to NaN.</summary>
	/// <remarks>Useful for debugging.</remarks>
	public static readonly VertexPosCol NaN = 
		new( Vector3.NaN, Vector4.NaN ) ;
	#endregion
	
	public Vector3 Position ;
	public ColorF Color ;

	/// <summary>Reference to the vertex's position value (<see cref="Vector3"/>).</summary>
	public ref Vector3 PositionRef {
		[MethodImpl(_MAXOPT_)] get { unsafe {
			fixed ( VertexPosCol* ptr = &this ) {
				return ref ptr->Position ;
			}
		}}
	}
	/// <summary>Reference to the vertex's color value (<see cref="Vector4"/>).</summary>
	public ref ColorF ColorRef {
		[MethodImpl(_MAXOPT_)] get { unsafe {
			fixed ( VertexPosCol* ptr = &this ) {
				return ref ptr->Color ;
			}
		}}
	}
	
	/// <summary>Gets a reference to the specified float component of the vertex.</summary>
	/// <param name="index">The index of the float component to get a reference to.</param>
	public ref float this[ int index ] {
		[MethodImpl(_MAXOPT_)] get { unsafe {
			fixed ( VertexPosCol* ptr = &this ) {
				float* valuePtr = (float*)ptr ;
				return ref valuePtr[ index ] ;
			}
		}}
	}
	
	
	/// <summary>Creates a new vertex with the specified position and color.</summary>
	/// <param name="position">The position of the vertex. Defaults to <see cref="Vector3.Zero"/>.</param>
	/// <param name="color">The color of the vertex. Defaults to <see cref="Vector4.Zero"/>.</param>
	public VertexPosCol( Vector3 position = default, Vector4 color = default ) {
		Position = position ; Color = color ;
	}
	
	public VertexPosCol( float x, float y, float z, float r, float g, float b, float a ) {
		Position = new Vector3( x, y, z ) ;
		Color = new Vector4( r, g, b, a ) ;
	}
	
	public VertexPosCol( in (float x, float y, float z) posTuple, 
				   in (float r, float g, float b, float a) colorTuple ) {
		Position = posTuple ; Color = colorTuple ;
	}
	
	
	public override int GetHashCode( ) => HashCode.Combine( Position, Color ) ;
	public override string ToString( ) => $"Vertex: {{ Position: {Position}, Color: {Color} }}" ;
	public bool Equals( VertexPosCol other ) => Position == other.Position && Color == other.Color ;
	public bool Equals( Vector3 other ) => Position == other ;
	public bool Equals( Vector4 other ) => Color == other ;

	
	public static implicit operator VertexPosCol( in (Vector3 pos, Vector4 col) tuple ) => new( tuple.pos, tuple.col ) ;
	public static implicit operator VertexPosCol( in (float x, float y, float z, float r, float g, float b, float a) tuple ) => 
		new( tuple.x, tuple.y, tuple.z, tuple.r, tuple.g, tuple.b, tuple.a ) ;
} ;