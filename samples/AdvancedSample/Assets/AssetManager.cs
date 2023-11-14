using System.Runtime.CompilerServices ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;

namespace AdvancedDXS ;


public struct TextureResource {
	public const  int D3D12_REQ_MIP_LEVELS = 15 ;
	
	public uint   Width ;
	public uint   Height ;
	public uint   MipLevels ;
	public Format Format ;

	public struct DataProperties {
		public const int ElementCount = 3,
						 SizeInBytes  = sizeof( uint ) * 3 ;
		
		public uint Offset, Size, Pitch ;

		public DataProperties( uint offset = 0, uint size = 0, uint pitch = 0 ) {
			Offset = offset ; Size = size ; Pitch  = pitch ;
		}
		public static implicit operator DataProperties( in (uint offset, uint size, uint pitch) tuple ) =>
			new( tuple.offset, tuple.size, tuple.pitch ) ;
	} ;

	// Data[ D3D12_REQ_MIP_LEVELS ]
	unsafe fixed uint _data[ D3D12_REQ_MIP_LEVELS * DataProperties.ElementCount ] ;
	
	public ref DataProperties this[ int index ] {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			unsafe {
#if DEBUG || DEV_BUILD
				if ( index is < 0 or >= D3D12_REQ_MIP_LEVELS )
					throw new IndexOutOfRangeException( index.ToString( ) ) ;
#endif
				fixed ( uint* ptr = _data ) {
					return ref *( (DataProperties*)ptr + index ) ;
				}
			}
		}
	}
} ;


public struct DrawParameters {
	public int  DiffuseTextureIndex ;
	public int  NormalTextureIndex ;
	public int  SpecularTextureIndex ;
	public uint IndexStart ;
	public uint IndexCount ;
	public uint VertexBase ;
	
	public DrawParameters( int diffuseTextureIndex = 0, int normalTextureIndex = 0, 
						   int  specularTextureIndex = 0, uint indexStart = 0, 
						   uint indexCount = 0, uint vertexBase = 0 ) {
		DiffuseTextureIndex  = diffuseTextureIndex ;
		NormalTextureIndex   = normalTextureIndex ;
		SpecularTextureIndex = specularTextureIndex ;
		IndexStart           = indexStart ;
		IndexCount           = indexCount ;
		VertexBase           = vertexBase ;
	}
} ;


public class AssetManager {
	public const string DataFileName         = "SquidRoom.bin" ;
	public const uint   StandardVertexStride = 44 ;
	public const uint   VertexDataOffset     = 30277640 ;
	public const uint   VertexDataSize       = 9685808 ;
	public const uint   IndexDataOffset      = 39963448 ;
	public const uint   IndexDataSize        = 3056844 ;
	public const Format StandardIndexFormat  = Format.R32_UINT ;
	
	public static InputElementDescription[ ] StandardVertexDescription = {
		new( "POSITION", 0, Format.R32G32B32_FLOAT, 0, 0,  InputClassification.PerVertexData, 0 ),
		new( "NORMAL",   0, Format.R32G32B32_FLOAT, 0, 12, InputClassification.PerVertexData, 0 ),
		new( "TEXCOORD", 0, Format.R32G32_FLOAT,    0, 24, InputClassification.PerVertexData, 0 ),
		new( "TANGENT",  0, Format.R32G32B32_FLOAT, 0, 32, InputClassification.PerVertexData, 0 ),
	} ;

	public static TextureResource[ ]? Textures { get ; set ; }
	public static DrawParameters[ ]? Draws { get ; set ; }
} ;