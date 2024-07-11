#region Using Directives
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12.Shader ;


[EquivalentOf(typeof(D3D_INCLUDE_TYPE))]
public enum IncludeType {
	/// <summary>The local directory.</summary>
	Local = 0,
	/// <summary>The system directory.</summary>
	System = 1,
	/// <summary>The local directory.</summary>
	D3D10Local = 0,
	/// <summary>The system directory.</summary>
	D3D10System = 1,
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
	Reserved0 = 65520,
} ;


[EquivalentOf(typeof(D3D_SHADER_VARIABLE_TYPE))]
public enum ShaderVariableType {
	/// <summary>The variable is a void pointer.</summary>
	Void = 0,
	/// <summary>The variable is a boolean.</summary>
	Bool = 1,
	/// <summary>The variable is an integer.</summary>
	Int = 2,
	/// <summary>The variable is a floating-point number.</summary>
	Float = 3,
	/// <summary>The variable is a string.</summary>
	String = 4,
	/// <summary>The variable is a texture.</summary>
	Texture = 5,
	/// <summary>The variable is a 1D texture.</summary>
	Texture1D = 6,
	/// <summary>The variable is a 2D texture.</summary>
	Texture2D = 7,
	/// <summary>The variable is a 3D texture.</summary>
	Texture3D = 8,
	/// <summary>The variable is a texture cube.</summary>
	TextureCube = 9,
	/// <summary>The variable is a sampler.</summary>
	Sampler = 10,
	/// <summary>The variable is a 1D sampler.</summary>
	Sampler1D = 11,
	/// <summary>The variable is a 2D sampler.</summary>
	Sampler2D = 12,
	/// <summary>The variable is a 3D sampler.</summary>
	Sampler3D = 13,
	/// <summary>The variable is a cube sampler.</summary>
	SamplerCube = 14,
	/// <summary>The variable is a pixel shader.</summary>
	PixelShader = 15,
	/// <summary>The variable is a vertex shader.</summary>
	VertexShader = 16,
	/// <summary>The variable is a pixel fragment.</summary>
	PixelFragment = 17,
	/// <summary>The variable is a vertex fragment.</summary>
	VertexFragment = 18,
	/// <summary>The variable is an unsigned integer.</summary>
	UInt = 19,
	/// <summary>The variable is an 8-bit unsigned integer.</summary>
	UInt8 = 20,
	/// <summary>The variable is a geometry shader.</summary>
	GeometryShader = 21,
	/// <summary>The variable is a rasterizer-state object.</summary>
	Rasterizer = 22,
	/// <summary>The variable is a depth-stencil-state object.</summary>
	DepthStencil = 23,
	/// <summary>The variable is a blend-state object.</summary>
	Blend = 24,
	/// <summary>The variable is a buffer.</summary>
	Buffer = 25,
	/// <summary>The variable is a constant buffer.</summary>
	Cbuffer = 26,
	/// <summary>The variable is a texture buffer.</summary>
	TBuffer = 27,
	/// <summary>The variable is a 1D-texture array.</summary>
	Texture1DArray = 28,
	/// <summary>The variable is a 2D-texture array.</summary>
	Texture2DArray = 29,
	/// <summary>The variable is a render-target view.</summary>
	RenderTargetView = 30,
	/// <summary>The variable is a depth-stencil view.</summary>
	DepthStencilView = 31,
	/// <summary>The variable is a 2D-multisampled texture.</summary>
	Texture2DMS = 32,
	/// <summary>The variable is a 2D-multisampled-texture array.</summary>
	Texture2DMSArray = 33,
	/// <summary>The variable is a texture-cube array.</summary>
	TextureCubeArray = 34,
	/// <summary>The variable holds a compiled hull-shader binary.</summary>
	HullShader = 35,
	/// <summary>The variable holds a compiled domain-shader binary.</summary>
	DomainShader = 36,
	/// <summary>The variable is an interface.</summary>
	InterfacePointer = 37,
	/// <summary>The variable holds a compiled compute-shader binary.</summary>
	ComputeShader = 38,
	/// <summary>The variable is a double precision (64-bit) floating-point number.</summary>
	Double = 39,
	/// <summary>The variable is a 1D read-and-write texture.</summary>
	RWTexture1D = 40,
	/// <summary>The variable is an array of 1D read-and-write textures.</summary>
	RWTexture1DArray = 41,
	/// <summary>The variable is a 2D read-and-write texture.</summary>
	RWTexture2D = 42,
	/// <summary>The variable is an array of 2D read-and-write textures.</summary>
	RWTexture2DArray = 43,
	/// <summary>The variable is a 3D read-and-write texture.</summary>
	RWTexture3D = 44,
	/// <summary>The variable is a read-and-write buffer.</summary>
	RWBuffer = 45,
	/// <summary>The variable is a byte-address buffer.</summary>
	ByteAddressBuffer = 46,
	/// <summary>The variable is a read-and-write byte-address buffer.</summary>
	RWByteAddressBuffer = 47,
	
	/// <summary>
	/// <para>The variable is a structured buffer. For more information about structured buffer, see the <b>Remarks</b> section.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3dcommon/ne-d3dcommon-d3d_shader_variable_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	StructuredBuffer = 48,
	
	/// <summary>The variable is a read-and-write structured buffer.</summary>
	RWStructuredBuffer = 49,
	/// <summary>The variable is an append structured buffer.</summary>
	AppendStructuredBuffer = 50,
	/// <summary>The variable is a consume structured buffer.</summary>
	ConsumeStructuredBuffer = 51,
	/// <summary>The variable is an 8-byte FLOAT.</summary>
	Min8Float = 52,
	/// <summary>The variable is a 10-byte FLOAT.</summary>
	Min10Float = 53,
	/// <summary>The variable is a 16-byte FLOAT.</summary>
	Min16Float = 54,
	/// <summary>The variable is a 12-byte INT.</summary>
	Min12Int = 55,
	/// <summary>The variable is a 16-byte INT.</summary>
	Min16Int = 56,
	/// <summary>The variable is a 16-byte INT.</summary>
	Min16UInt = 57,
	Int16 = 58,
	UInt16 = 59,
	Float16 = 60,
	Int64 = 61,
	UInt64 = 62,
	/// <summary>The variable is a void pointer.</summary>
	D3D10Void = 0,
	/// <summary>The variable is a boolean.</summary>
	D3D10Bool = 1,
	/// <summary>The variable is an integer.</summary>
	D3D10Int = 2,
	/// <summary>The variable is a floating-point number.</summary>
	D3D10Float = 3,
	/// <summary>The variable is a string.</summary>
	D3D10String = 4,
	/// <summary>The variable is a texture.</summary>
	D3D10Texture = 5,
	/// <summary>The variable is a 1D texture.</summary>
	D3D10Texture1D = 6,
	/// <summary>The variable is a 2D texture.</summary>
	D3D10Texture2D = 7,
	/// <summary>The variable is a 3D texture.</summary>
	D3D10Texture3D = 8,
	/// <summary>The variable is a texture cube.</summary>
	D3D10TextureCube = 9,
	/// <summary>The variable is a sampler.</summary>
	D3D10Sampler = 10,
	/// <summary>The variable is a 1D sampler.</summary>
	D3D10Sampler1D = 11,
	/// <summary>The variable is a 2D sampler.</summary>
	D3D10Sampler2D = 12,
	/// <summary>The variable is a 3D sampler.</summary>
	D3D10Sampler3D = 13,
	/// <summary>The variable is a cube sampler.</summary>
	D3D10SamplerCube = 14,
	/// <summary>The variable is a pixel shader.</summary>
	D3D10PixelShader = 15,
	/// <summary>The variable is a vertex shader.</summary>
	D3D10VertexShader = 16,
	/// <summary>The variable is a pixel fragment.</summary>
	D3D10Pixelfragment = 17,
	/// <summary>The variable is a vertex fragment.</summary>
	D3D10Vertexfragment = 18,
	/// <summary>The variable is an unsigned integer.</summary>
	D3D10UInt = 19,
	/// <summary>The variable is an 8-bit unsigned integer.</summary>
	D3D10UInt8 = 20,
	/// <summary>The variable is a geometry shader.</summary>
	D3D10GeometryShader = 21,
	/// <summary>The variable is a rasterizer-state object.</summary>
	D3D10Rasterizer = 22,
	/// <summary>The variable is a depth-stencil-state object.</summary>
	D3D10DepthStencil = 23,
	/// <summary>The variable is a blend-state object.</summary>
	D3D10Blend = 24,
	/// <summary>The variable is a buffer.</summary>
	D3D10Buffer = 25,
	/// <summary>The variable is a constant buffer.</summary>
	D3D10CBuffer = 26,
	/// <summary>The variable is a texture buffer.</summary>
	D3D10TBuffer = 27,
	/// <summary>The variable is a 1D-texture array.</summary>
	D3D10Texture1DArray = 28,
	/// <summary>The variable is a 2D-texture array.</summary>
	D3D10Texture2DArray = 29,
	/// <summary>The variable is a render-target view.</summary>
	D3D10Rendertargetview = 30,
	/// <summary>The variable is a depth-stencil view.</summary>
	D3D10DepthStencilView = 31,
	/// <summary>The variable is a 2D-multisampled texture.</summary>
	D3D10Texture2DMS = 32,
	/// <summary>The variable is a 2D-multisampled-texture array.</summary>
	D3D10Texture2DMSArray = 33,
	/// <summary>The variable is a texture-cube array.</summary>
	D3D10TextureCubeArray = 34,
	/// <summary>The variable holds a compiled hull-shader binary.</summary>
	D3D11HullShader = 35,
	/// <summary>The variable holds a compiled domain-shader binary.</summary>
	D3D11DomainShader = 36,
	/// <summary>The variable is an interface.</summary>
	D3D11InterfacePointer = 37,
	/// <summary>The variable holds a compiled compute-shader binary.</summary>
	D3D11ComputeShader = 38,
	/// <summary>The variable is a double precision (64-bit) floating-point number.</summary>
	D3D11Double = 39,
	/// <summary>The variable is a 1D read-and-write texture.</summary>
	D3D11RWTexture1D = 40,
	/// <summary>The variable is an array of 1D read-and-write textures.</summary>
	D3D11RWTexture1DArray = 41,
	/// <summary>The variable is a 2D read-and-write texture.</summary>
	D3D11RWTexture2D = 42,
	/// <summary>The variable is an array of 2D read-and-write textures.</summary>
	D3D11RWTexture2DArray = 43,
	/// <summary>The variable is a 3D read-and-write texture.</summary>
	D3D11RWTexture3D = 44,
	/// <summary>The variable is a read-and-write buffer.</summary>
	D3D11RWBuffer = 45,
	/// <summary>The variable is a byte-address buffer.</summary>
	D3D11ByteAddressBuffer = 46,
	/// <summary>The variable is a read and write byte-address buffer.</summary>
	D3D11RWByteAddressBuffer = 47,
	/// <summary>The variable is a structured buffer.</summary>
	D3D11StructuredBuffer = 48,
	/// <summary>The variable is a read-and-write structured buffer.</summary>
	D3D11RwstructuredBuffer = 49,
	/// <summary>The variable is an append structured buffer.</summary>
	D3D11AppendStructuredBuffer = 50,
	/// <summary>The variable is a consume structured buffer.</summary>
	D3D11ConsumeStructuredBuffer = 51,
}

