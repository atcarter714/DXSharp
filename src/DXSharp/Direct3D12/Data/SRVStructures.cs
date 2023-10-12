#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_CONSTANT_BUFFER_VIEW_DESC ) )]
public struct ConstBufferViewDescription {
	/// <summary>
	/// <para>The D3D12_GPU_VIRTUAL_ADDRESS of the constant buffer. D3D12_GPU_VIRTUAL_ADDRESS is a typedef'd alias of UINT64.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_constant_buffer_view_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ulong BufferLocation ;
	
	/// <summary>The size in bytes of the constant buffer.</summary>
	public uint SizeInBytes ;
	
	public ConstBufferViewDescription( ulong bufferLocation, uint sizeInBytes ) {
		BufferLocation = bufferLocation ;
		SizeInBytes    = sizeInBytes ;
	}
	public ConstBufferViewDescription(in D3D12_CONSTANT_BUFFER_VIEW_DESC desc) {
		BufferLocation = desc.BufferLocation ;
		SizeInBytes    = desc.SizeInBytes ;
	}
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_BUFFER_SRV ) )]
public struct BufferSRV {
	/// <summary>The index of the first element to be accessed by the view.</summary>
	public ulong FirstElement ;
	/// <summary>The number of elements in the resource.</summary>
	public uint NumElements ;
	/// <summary>The size of each element in the buffer structure (in bytes) when the buffer represents a structured buffer.</summary>
	public uint StructureByteStride ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_buffer_srv_flags">D3D12_BUFFER_SRV_FLAGS</a>-typed
	/// value that identifies view options for the buffer. Currently, the only option is to identify a raw view of the buffer. For more
	/// info about raw viewing of buffers, see
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-resources-intro">
	/// Raw Views of Buffers
	/// </a>.
	/// </summary>
	public BufferSRVFlags Flags ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX1D_SRV ) )]
public struct Tex1DSRV {
	/// <summary>Index of the most detailed mipmap level to use; this number is between 0 and <b>MipLevels</b> (from the original Texture1D for which <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> creates a view) -1.</summary>
	public uint MostDetailedMip;

	/// <summary>
	/// <para>The maximum number of mipmap levels for the view  of the texture. See the remarks. Set to -1 to indicate all the mipmap levels from <b>MostDetailedMip</b> on down to least detailed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex1d_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint MipLevels;

	/// <summary>
	/// <para>Specifies the minimum mipmap level that you can access. Specifying 0.0f means that you can access all of the mipmap levels. Specifying 3.0f means that you can access mipmap levels from 3.0f to *MipCount - 1*. We recommend that you don't set *MostDetailedMip* and *ResourceMinLODClamp* at the same time. Instead, set one of those two members to 0 (to get default behavior). That's because *MipLevels* is interpreted differently in conjunction with different fields: * For *MostDetailedMip*, mips are in the range \[*MostDetailedMip*, *MostDetailedMip* + *MipLevels* - 1]. * For *ResourceMinLODClamp*, mips are in the range \[*ResourceMinLODClamp*, *MipLevels* - 1].</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex1d_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float ResourceMinLODClamp;
	
	public Tex1DSRV( uint mostDetailedMip, uint mipLevels, float resourceMinLODClamp ) {
		MostDetailedMip    = mostDetailedMip ;
		MipLevels          = mipLevels ;
		ResourceMinLODClamp = resourceMinLODClamp ;
	}
	
	public Tex1DSRV( in D3D12_TEX1D_SRV desc ) {
		MostDetailedMip    = desc.MostDetailedMip ;
		MipLevels          = desc.MipLevels ;
		ResourceMinLODClamp = desc.ResourceMinLODClamp ;
	}
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX1D_ARRAY_SRV ) )]
public struct Tex1DArraySRV {
	/// <summary>Index of the most detailed mipmap level to use; this number is between 0 and <b>MipLevels</b> (from the original Texture1D for which <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> creates a view) -1.</summary>
	public uint MostDetailedMip ;

	/// <summary>
	/// <para>The maximum number of mipmap levels for the view of the texture. See the remarks in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tex1d_srv">D3D12_TEX1D_SRV</a>. Set to -1 to indicate all the mipmap levels from <b>MostDetailedMip</b> on down to least detailed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex1d_array_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint MipLevels ;

	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;

	/// <summary>Number of textures in the array.</summary>
	public uint ArraySize ;

	/// <summary>
	/// <para>Specifies the minimum mipmap level that you can access. Specifying 0.0f means that you can access all of the mipmap levels. Specifying 3.0f means that you can access mipmap levels from 3.0f to *MipCount - 1*. We recommend that you don't set *MostDetailedMip* and *ResourceMinLODClamp* at the same time. Instead, set one of those two members to 0 (to get default behavior). That's because *MipLevels* is interpreted differently in conjunction with different fields: * For *MostDetailedMip*, mips are in the range \[*MostDetailedMip*, *MostDetailedMip* + *MipLevels* - 1]. * For *ResourceMinLODClamp*, mips are in the range \[*ResourceMinLODClamp*, *MipLevels* - 1].</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex1d_array_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float ResourceMinLODClamp ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_SRV ) )]
public struct Tex2DSRV {
	/// <summary>The index of the most detailed mipmap level to use; this number is between 0 and <b>MipLevels</b> (from the original Texture2D for which <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> creates a view) -1.</summary>
	public uint MostDetailedMip ;
	/// <summary>The maximum number of mipmap levels for the view of the texture. See the remarks in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tex1d_srv">D3D12_TEX1D_SRV</a>. Set to -1 to indicate all the mipmap levels from <b>MostDetailedMip</b> on down to least detailed.</summary>
	public uint MipLevels ;
	/// <summary>The index (plane slice number) of the plane to use in the texture.</summary>
	public uint PlaneSlice ;

	/// <summary>
	/// <para>Specifies the minimum mipmap level that you can access. Specifying 0.0f means that you can access all of the mipmap levels. Specifying 3.0f means that you can access mipmap levels from 3.0f to *MipCount - 1*. We recommend that you don't set *MostDetailedMip* and *ResourceMinLODClamp* at the same time. Instead, set one of those two members to 0 (to get default behavior). That's because *MipLevels* is interpreted differently in conjunction with different fields: * For *MostDetailedMip*, mips are in the range \[*MostDetailedMip*, *MostDetailedMip* + *MipLevels* - 1]. * For *ResourceMinLODClamp*, mips are in the range \[*ResourceMinLODClamp*, *MipLevels* - 1].</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex2d_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float ResourceMinLODClamp ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_ARRAY_SRV ) )]
public struct Tex2DArraySRV {
	/// <summary>Index of the most detailed mipmap level to use; this number is between 0 and <b>MipLevels</b> -1 (where <b>MipLevels</b> is from the original Texture2D for which <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> creates a view).</summary>
	public uint MostDetailedMip ;

	/// <summary>
	/// <para>The maximum number of mipmap levels for the view of the texture. See the remarks in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tex1d_srv">D3D12_TEX1D_SRV</a>.</para>
	/// <para>Set to -1 to indicate all the mipmap levels from <b>MostDetailedMip</b> on down to least detailed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex2d_array_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint MipLevels ;

	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;

	/// <summary>Number of textures in the array.</summary>
	public uint ArraySize ;

	/// <summary>The index (plane slice number) of the plane to use in an array of textures.</summary>
	public uint PlaneSlice ;

	/// <summary>
	/// <para>Specifies the minimum mipmap level that you can access. Specifying 0.0f means that you can access all of the mipmap levels. Specifying 3.0f means that you can access mipmap levels from 3.0f to *MipCount - 1*. We recommend that you don't set *MostDetailedMip* and *ResourceMinLODClamp* at the same time. Instead, set one of those two members to 0 (to get default behavior). That's because *MipLevels* is interpreted differently in conjunction with different fields: * For *MostDetailedMip*, mips are in the range \[*MostDetailedMip*, *MostDetailedMip* + *MipLevels* - 1]. * For *ResourceMinLODClamp*, mips are in the range \[*ResourceMinLODClamp*, *MipLevels* - 1].</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex2d_array_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float ResourceMinLODClamp ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_SRV ) )]
public struct Tex2DMS {
	/// <summary>Integer of any value. See remarks.</summary>
	public uint UnusedField_NothingToDefine ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_ARRAY_SRV ) )]
public struct Tex2DMSArraySRV {
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice;
	/// <summary>Number of textures to use.</summary>
	public uint ArraySize;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX3D_SRV ) )]
public struct Tex3DSRV {
	/// <summary>Index of the most detailed mipmap level to use; this number is between 0 and <b>MipLevels</b> (from the original Texture3D for which <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> creates a view) -1.</summary>
	public uint MostDetailedMip ;

	/// <summary>
	/// <para>The maximum number of mipmap levels for the view of the texture. See the remarks in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tex1d_srv">D3D12_TEX1D_SRV</a>. Set to -1 to indicate all the mipmap levels from <b>MostDetailedMip</b> on down to least detailed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex3d_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint MipLevels ;

	/// <summary>
	/// <para>Specifies the minimum mipmap level that you can access. Specifying 0.0f means that you can access all of the mipmap levels. Specifying 3.0f means that you can access mipmap levels from 3.0f to *MipCount - 1*. We recommend that you don't set *MostDetailedMip* and *ResourceMinLODClamp* at the same time. Instead, set one of those two members to 0 (to get default behavior). That's because *MipLevels* is interpreted differently in conjunction with different fields: * For *MostDetailedMip*, mips are in the range \[*MostDetailedMip*, *MostDetailedMip* + *MipLevels* - 1]. * For *ResourceMinLODClamp*, mips are in the range \[*ResourceMinLODClamp*, *MipLevels* - 1].</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex3d_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float ResourceMinLODClamp ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEXCUBE_SRV ) )]
public struct TexCubeSRV {
	/// <summary>Index of the most detailed mipmap level to use; this number is between 0 and <b>MipLevels</b> (from the original TextureCube for which <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> creates a view) -1.</summary>
	public uint MostDetailedMip ;

	/// <summary>
	/// <para>The maximum number of mipmap levels for the view of the texture. See the remarks in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tex1d_srv">D3D12_TEX1D_SRV</a>. Set to -1 to indicate all the mipmap levels from <b>MostDetailedMip</b> on down to least detailed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_texcube_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint MipLevels ;

	/// <summary>
	/// <para>Specifies the minimum mipmap level that you can access. Specifying 0.0f means that you can access all of the mipmap levels. Specifying 3.0f means that you can access mipmap levels from 3.0f to *MipCount - 1*. We recommend that you don't set *MostDetailedMip* and *ResourceMinLODClamp* at the same time. Instead, set one of those two members to 0 (to get default behavior). That's because *MipLevels* is interpreted differently in conjunction with different fields: * For *MostDetailedMip*, mips are in the range \[*MostDetailedMip*, *MostDetailedMip* + *MipLevels* - 1]. * For *ResourceMinLODClamp*, mips are in the range \[*ResourceMinLODClamp*, *MipLevels* - 1].</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_texcube_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float ResourceMinLODClamp ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEXCUBE_ARRAY_SRV ) )]
public struct TexCubeArraySRV {
	/// <summary>Index of the most detailed mipmap level to use; this number is between 0 and <b>MipLevels</b> (from the original TextureCube for which <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> creates a view) -1.</summary>
	public uint MostDetailedMip ;

	/// <summary>
	/// <para>The maximum number of mipmap levels for the view of the texture. See the remarks in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tex1d_srv">D3D12_TEX1D_SRV</a>. Set to -1 to indicate all the mipmap levels from <b>MostDetailedMip</b> on down to least detailed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_texcube_array_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint MipLevels ;

	/// <summary>Index of the first 2D texture to use.</summary>
	public uint First2DArrayFace ;

	/// <summary>Number of cube textures in the array.</summary>
	public uint NumCubes ;

	/// <summary>
	/// <para>Specifies the minimum mipmap level that you can access. Specifying 0.0f means that you can access all of the mipmap levels. Specifying 3.0f means that you can access mipmap levels from 3.0f to *MipCount - 1*. We recommend that you don't set *MostDetailedMip* and *ResourceMinLODClamp* at the same time. Instead, set one of those two members to 0 (to get default behavior). That's because *MipLevels* is interpreted differently in conjunction with different fields: * For *MostDetailedMip*, mips are in the range \[*MostDetailedMip*, *MostDetailedMip* + *MipLevels* - 1]. * For *ResourceMinLODClamp*, mips are in the range \[*ResourceMinLODClamp*, *MipLevels* - 1].</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_texcube_array_srv#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float ResourceMinLODClamp ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_RAYTRACING_ACCELERATION_STRUCTURE_SRV ) )]
public struct RayTracingAccelerationStructureSRV {
	/// <summary>The GPU virtual address of the SRV.</summary>
	public ulong Location ;
} ;



/// <summary>Describes a shader-resource view. (D3D12_SHADER_RESOURCE_VIEW_DESC)</summary>
/// <remarks>
/// <para>A view is a format-specific way to look at the data in a resource. The view determines what data to look at, and how it is cast when read. When viewing a resource, the resource-view description must specify a typed format, that is compatible with the resource format. So that means that you can't create a resource-view description using any format with _TYPELESS in the name. You can however view a typeless resource by specifying a typed format for the view. For example, a DXGI_FORMAT_R32G32B32_TYPELESS resource can be viewed with one of these typed formats: DXGI_FORMAT_R32G32B32_FLOAT, DXGI_FORMAT_R32G32B32_UINT, and DXGI_FORMAT_R32G32B32_SINT, since these typed formats are compatible with the typeless resource. Create a shader-resource-view description by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a>.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_resource_view_desc#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_SHADER_RESOURCE_VIEW_DESC ) )]
public struct ShaderResourceViewDescription {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value that specifies the viewing format. See remarks.</summary>
	public Format Format ;
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_srv_dimension">D3D12_SRV_DIMENSION</a>-typed value that specifies the resource type of the view. This type is the same as the resource type of the underlying resource. This member also determines which _SRV to use in the union below.</summary>
	public SRVDimension ViewDimension ;
	/// <summary>A value, constructed using the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_shader_component_mapping">D3D12_ENCODE_SHADER_4_COMPONENT_MAPPING</a> macro. The **D3D12_SHADER_COMPONENT_MAPPING** enumeration specifies what values from memory should be returned when the texture is accessed in a shader via this shader resource view (SRV). For example, it can route component 1 (green) from memory, or the constant `0`, into component 2 (`.b`) of the value given to the shader.</summary>
	public uint Shader4ComponentMapping ;
	
	public _union_SRV ReadAs ;
	[StructLayout(LayoutKind.Explicit)]
	public struct _union_SRV {
		[FieldOffset(0)] public  BufferSRV Buffer ;
		[FieldOffset(0)] public  Tex1DSRV Texture1D ;
		[FieldOffset(0)] public  Tex1DArraySRV Texture1DArray ;
		[FieldOffset(0)] public  Tex2DSRV Texture2D ;
		[FieldOffset(0)] public  Tex2DArraySRV Texture2DArray ;
		[FieldOffset(0)] public  Tex2DMS Texture2DMS ;
		[FieldOffset(0)] public  Tex2DMSArraySRV Texture2DMSArray ;
		[FieldOffset(0)] public  Tex3DSRV Texture3D ;
		[FieldOffset(0)] public  TexCubeSRV TextureCube ;
		[FieldOffset(0)] public  TexCubeArraySRV TextureCubeArray ;
		[FieldOffset(0)] public  RayTracingAccelerationStructureSRV RaytracingAccelerationStructure ;
	} ;
} ;


