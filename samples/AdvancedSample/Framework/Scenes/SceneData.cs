#region Using Directives
using System.Numerics ;
using System.Runtime.InteropServices ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;

using DXSharp ;
using Vector3 = DXSharp.Vector3 ;
using Vector4 = DXSharp.Vector4 ;
#endregion
namespace AdvancedDXS.Framework.Scenes ;


[StructLayout(LayoutKind.Sequential, Pack = 16)]
public struct DirLightData {
	public const int SizeInBytes = 64 ;
	
	public Vector4 Position ;
	public Vector4 Direction ;
	public Vector4 Color ;
	public Vector4 Falloff ; //! 16x floats +
	
	public ref Vector4 this [ int index ] {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			unsafe {
#if DEBUG || DEV_BUILD
				if( index is < 0 or > 3 )
					throw new IndexOutOfRangeException(index.ToString() ) ;
#endif
				
				fixed ( DirLightData* ptr = &this ) {
					return ref *( (Vector4 *)ptr + index ) ;
				}
			}
		}
	}
	
	
	public DirLightData ( in Vector4 position  = default,
								  in Vector4 direction = default,
								  in Vector4 color     = default,
								  in Vector4 falloff   = default ) {
		Position = position ;
		Direction = direction ;
		Color = color ;
		Falloff = falloff ;
	}
	
	public static implicit operator DirLightData ( in (Vector4 position,
													   Vector4 direction,
													   Vector4 color,
													   Vector4 falloff) tuple ) => 
														new( tuple.position, tuple.direction,
																	tuple.color, tuple.falloff ) ;
} ; //! 16x floats (64 bytes)


[StructLayout(LayoutKind.Sequential, Pack = 16)]
public struct LightState {
	public const int SizeInBytes = 192 ;
	
 	public DirLightData Light ;		//! 16x floats +
	
	public Matrix4x4 View ;         //! 16x floats +
	public Matrix4x4 Projection ;   //! 16x floats =
									//! 48x floats (192 bytes)
	
	
	public LightState ( in Vector4   position   = default,
						in Vector4   direction  = default,
						in Vector4   color      = default,
						in Vector4   falloff    = default,
						in Matrix4x4 view       = default,
						in Matrix4x4 projection = default ) {
		Light = new( position, direction, color, falloff ) ;
		View  = view ;
		Projection = projection ;
	}
	
	public LightState ( in DirLightData light,
						in Matrix4x4   view,
						in Matrix4x4   projection ) {
		Light = light ;
		View  = view ;
		Projection = projection ;
	}
	
} ; //! 48x floats (192 bytes)


[StructLayout(LayoutKind.Sequential, Pack = 16)]
[SuppressMessage( "ReSharper", "PrivateFieldCanBeConvertedToLocalVariable" )]
public struct SceneConstBuffer {
	public const int LightCount  = 4 ;
	public const int SizeInBytes = 826 ;
	
	public Matrix4x4 Model ;        //! 16x floats +
	public Matrix4x4 View ;         //! 16x floats +
	public Matrix4x4 Projection ;   //! 16x floats +
	public Vector4   AmbientColor ; //! 4x floats +
	
	public bool SampleShadowMap ;   //! 1x byte +
	unsafe fixed byte _pad0[ 3 ];   //! 3x bytes +
	public Vector3 ExtraData ;      //! 3x floats +		(we'll find a use for this!)
	
	public LightState Lights0,
					  Lights1,
					  Lights2,
					  Lights3 ;		//! 4x 48x floats (192 bytes) = 768 bytes
									//! Total: 768 bytes + 48 bytes + 4 bytes + 3 bytes + 3 bytes = 826 bytes
									
	public readonly LightState[ ] LightStates => 
		new[ ] { Lights0, Lights1, Lights2, Lights3 } ;
	

	public SceneConstBuffer ( in Matrix4x4  model           = default,
							  in Matrix4x4  view            = default,
							  in Matrix4x4  projection      = default,
							  in Vector4    ambientColor    = default,
							  bool          sampleShadowMap = false,
							  in LightState lights0         = default,
							  in LightState lights1         = default,
							  in LightState lights2         = default,
							  in LightState lights3         = default ) {
		Unsafe.SkipInit( out this ) ;
		Model = model ; View = view ; Projection = projection ;
		AmbientColor = ambientColor ;
		
		SampleShadowMap = sampleShadowMap ;
		Lights0 = lights0 ;
		Lights1 = lights1 ;
		Lights2 = lights2 ;
		Lights3 = lights3 ;
	}
	
} ;