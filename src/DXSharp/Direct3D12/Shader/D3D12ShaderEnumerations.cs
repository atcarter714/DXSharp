using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12.Shader ;


[EquivalentOf(typeof(D3D_INCLUDE_TYPE))]
public enum IncludeType {
	/// <summary>The local directory.</summary>
	Local = 0,
	/// <summary>The system directory.</summary>
	System = 1,
	/// <summary>The local directory.</summary>
	D3D10_Local = 0,
	/// <summary>The system directory.</summary>
	D3D10_System = 1,
} ;


[EquivalentOf(typeof(D3D12_SHADER_VERSION_TYPE))]
public enum ShaderVersionType {
	/// <summary>Pixel shader.</summary>
	PixelShader = 0,
	/// <summary>Vertex shader.</summary>
	VertexShader = 1,
	/// <summary>Geometry shader.</summary>
	GeometryShader = 2,
	/// <summary>Hull shader.</summary>
	HullShader = 3,
	/// <summary>Domain shader.</summary>
	DomainShader = 4,
	
	/// <summary>Compute shader.</summary>
	ComputeShader = 5,
	Library             = 6,
	RayGenerationShader = 7,
	IntersectionShader  = 8,
	AnyHitShader        = 9,
	ClosestHitShader    = 10,
	MissShader          = 11,
	CallableShader      = 12,
	MeshShader          = 13,
	AmplificationShader = 14,
	
	/// <summary>Indicates the end of the enumeration.</summary>
	RESERVED0 = 65520,
} ;