#region Using Directives
using System.Buffers ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12 ;


// -----------------------------------------------------------------------------------------------------------------
// Contract Interfaces:
// -----------------------------------------------------------------------------------------------------------------

public interface IRootParam {
	ref RootParameterType ParameterType { get ; }
	ref ShaderVisibility ShaderVisibility { get ; }
} ;
public interface IRootParam1: IRootParam {
	
} ;


// -----------------------------------------------------------------------------------------------------------------
// Enumeration Types:
// -----------------------------------------------------------------------------------------------------------------


/// <summary>
/// Specifies the volatility of the data referenced by descriptors in a Root Signature 1.1 description, which can enable some driver optimizations.
/// </summary>
/// <remarks>
/// <para>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_root_descriptor1">D3D12_ROOT_DESCRIPTOR1</a> structure.
/// To specify the volatility of both descriptors and data, refer to
/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_range_flags">D3D12_DESCRIPTOR_RANGE_FLAGS</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_root_descriptor_flags#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[Flags, EquivalentOf( typeof( D3D12_ROOT_DESCRIPTOR_FLAGS ) )]
public enum RootDescriptorFlags {
	/// <summary>Default assumptions are made for data (for SRV/CBV: DATA_STATIC_WHILE_SET_AT_EXECUTE, and for UAV: DATA_VOLATILE).</summary>
	None = 0x00000000,
	/// <summary>Data is volatile. Equivalent to Root Signature Version 1.0.</summary>
	DataVolatile = 0x00000002,
	/// <summary>Data is static while set at execute.</summary>
	DataStaticWhileSetAtExecute = 0x00000004,
	/// <summary>Data is static. The best potential for driver optimization.</summary>
	DataStatic = 0x00000008,
} ;


/// <summary>Specifies options for root signature layout.</summary>
/// <remarks>
/// <para>This enum is used in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_root_signature_desc">D3D12_ROOT_SIGNATURE_DESC</a> structure.</para>
/// <para>The value in denying access to shader stages is a minor optimization on some hardware. If, for example, the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_shader_visibility">D3D12_SHADER_VISIBILITY_ALL</a> flag has been set to broadcast the root signature to all shader stages, then denying access can overrule this and save the hardware some work. Alternatively if the shader is so simple that no root signature resources are needed, then denying access could be used here too.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_root_signature_flags#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_ROOT_SIGNATURE_FLAGS))]
public enum RootSignatureFlags {
	/// <summary>Indicates default behavior.</summary>
	None = 0x00000000,
	/// <summary>The app is opting in to using the Input Assembler (requiring an input layout that defines a set of vertex buffer bindings). Omitting this flag can result in one root argument space being saved on some hardware. Omit this flag if the Input Assembler is not required, though the optimization is minor.</summary>
	AllowInputAssemblerInputLayout = 0x00000001,
	/// <summary>Denies the vertex shader access to the root signature.</summary>
	DenyVertexShaderRootAccess = 0x00000002,
	/// <summary>Denies the hull shader access to the root signature.</summary>
	DenyHullShaderRootAccess = 0x00000004,
	/// <summary>Denies the domain shader access to the root signature.</summary>
	DenyDomainShaderRootAccess = 0x00000008,
	/// <summary>Denies the geometry shader access to the root signature.</summary>
	DenyGeometryShaderRootAccess = 0x00000010,
	/// <summary>Denies the pixel shader access to the root signature.</summary>
	DenyPixelShaderRootAccess = 0x00000020,
	/// <summary>The app is opting in to using Stream Output. Omitting this flag can result in one root argument space being saved on some hardware. Omit this flag if Stream Output is not required, though the optimization is minor.</summary>
	AllowStreamOutput = 0x00000040,
	/// <summary>The root signature is to be used with raytracing shaders to define resource bindings sourced from shader records in shader tables.  This flag cannot be combined with any other root signature flags, which are all related to the graphics pipeline.  The absence of the flag means the root signature can be used with graphics or compute, where the compute version is also shared with raytracing’s global root signature.</summary>
	LocalRootSignature = 0x00000080,
	/// <summary>Denies the amplification shader access to the root signature.</summary>
	DenyAmplificationShaderRootAccess = 0x00000100,
	/// <summary>Denies the mesh shader access to the root signature.</summary>
	DenyMeshShaderRootAccess = 0x00000200,
	/// <summary>The shaders are allowed to index the CBV/SRV/UAV descriptor heap directly, using the *ResourceDescriptorHeap* built-in variable.</summary>
	CbvSrvUavHeapDirectlyIndexed = 0x00000400,
	/// <summary>The shaders are allowed to index the sampler descriptor heap directly, using the *SamplerDescriptorHeap* built-in variable.</summary>
	SamplerHeapDirectlyIndexed = 0x00000800,
} ;


/// <summary>Specifies the shaders that can access the contents of a given root signature slot.</summary>
/// <remarks>
/// <para>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_root_parameter">D3D12_ROOT_PARAMETER</a> structure. The compute queue always uses <b>D3D12_SHADER_VISIBILITY_ALL</b> because it has only one active stage. The 3D queue can choose values, but if it uses <b>D3D12_SHADER_VISIBILITY_ALL</b>, all shader stages can access whatever is bound at the root signature slot.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_shader_visibility#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_SHADER_VISIBILITY))]
public enum ShaderVisibility {
	/// <summary>Specifies that all shader stages can access whatever is bound at the root signature slot.</summary>
	All = 0,
	/// <summary>Specifies that the vertex shader stage can access whatever is bound at the root signature slot.</summary>
	Vertex = 1,
	/// <summary>Specifies that the hull shader stage can access whatever is bound at the root signature slot.</summary>
	Hull = 2,
	/// <summary>Specifies that the domain shader stage can access whatever is bound at the root signature slot.</summary>
	Domain = 3,
	/// <summary>Specifies that the geometry shader stage can access whatever is bound at the root signature slot.</summary>
	Geometry = 4,
	/// <summary>Specifies that the pixel shader stage can access whatever is bound at the root signature slot.</summary>
	Pixel = 5,
	/// <summary>Specifies that the amplification shader stage can access whatever is bound at the root signature slot.</summary>
	Amplification = 6,
	/// <summary>Specifies that the mesh shader stage can access whatever is bound at the root signature slot.</summary>
	Mesh = 7,
} ;


/// <summary>Specifies the border color for a static sampler.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_static_sampler_desc">D3D12_STATIC_SAMPLER_DESC</a> structure.</remarks>
[Flags, EquivalentOf(typeof(D3D12_STATIC_BORDER_COLOR))]
public enum StaticBorderColor {
	/// <summary>Indicates black, with the alpha component as fully transparent.</summary>
	TransparentBlack = 0,
	/// <summary>Indicates black, with the alpha component as fully opaque.</summary>
	OpaqueBlack = 1,
	/// <summary>Indicates white, with the alpha component as fully opaque.</summary>
	OpaqueWhite = 2,
	OpaqueBlackUint = 3,
	OpaqueWhiteUint = 4,
} ;


/// <summary>Specifies the type of root signature slot.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_root_parameter">D3D12_ROOT_PARAMETER</a> structure.</remarks>
[Flags, EquivalentOf(typeof(D3D12_ROOT_PARAMETER_TYPE))]
public enum RootParameterType {
	/// <summary>The slot is for a descriptor table.</summary>
	DescriptorTable = 0,
	/// <summary>The slot is for root constants.</summary>
	Const32Bits = 1,
	/// <summary>The slot is for a constant-buffer view (CBV).</summary>
	CBV = 2,
	/// <summary>The slot is for a shader-resource view (SRV).</summary>
	SRV = 3,
	/// <summary>The slot is for a unordered-access view (UAV).</summary>
	UAV = 4,
} ;


/// <summary>Specifies a range so that, for example, if part of a descriptor table has 100 shader-resource views (SRVs) that range can be declared in one entry rather than 100.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_range">D3D12_DESCRIPTOR_RANGE</a> structure.</remarks>
[EquivalentOf(typeof(D3D12_DESCRIPTOR_RANGE_TYPE))]
public enum DescriptorRangeType {
	/// <summary>Specifies a range of SRVs.</summary>
	SRV = 0,
	/// <summary>Specifies a range of unordered-access views (UAVs).</summary>
	UAV = 1,
	/// <summary>Specifies a range of constant-buffer views (CBVs).</summary>
	CBV = 2,
	/// <summary>Specifies a range of samplers.</summary>
	Sampler = 3,
} ;


/// <summary>Specifies the version of root signature layout.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d_root_signature_version#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D_ROOT_SIGNATURE_VERSION))]
public enum RootSignatureVersion {
	/// <summary>Version one of root signature layout.</summary>
	Version1 = 1,
	/// <summary>Version one of root signature layout.</summary>
	Version1_0 = 1,
	/// <summary>Version 1.1  of root signature layout. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/root-signature-version-1-1">Root Signature Version 1.1</a>.</summary>
	Version1_1 = 2,
	/// <summary>
	/// Version 1.2 of root signature layout. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/root-signature-version-1-2">Root Signature Version 1.2</a>.
	/// </summary>
	Version1_2 = 3,
} ;



// =================================================================================================================


[StructLayout( LayoutKind.Sequential ),
 EquivalentOf( typeof( D3D12_ROOT_PARAMETER ) )]
public struct RootParameter: IRootParam {
	
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_root_parameter_type">D3D12_ROOT_PARAMETER_TYPE</a>-typed value
	/// that specifies the type of root signature slot. This member determines which type to use in the union below.
	/// </summary>
	public RootParameterType parameterType ;

	/// <summary>
	/// A data union that specifies the root signature slot.
	/// The type of data in this union is determined by the value of the <see cref="RootParameterType"/> member.
	/// </summary>
	public _paramDataUnion parameterData ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_shader_visibility">D3D12_SHADER_VISIBILITY</a>-typed value
	/// that specifies the shaders that can access the contents of the root signature slot.
	/// </summary>
	public ShaderVisibility shaderVisibility ;

	[StructLayout( LayoutKind.Explicit )]
	public partial struct _paramDataUnion {
		[FieldOffset(0)] public RootDescriptorTable DescriptorTable ;
		[FieldOffset(0)] public RootConstants Constants ;
		[FieldOffset(0)] public RootDescriptor Descriptor ;
		
		public _paramDataUnion( in RootDescriptorTable descriptorTable ) {
			Unsafe.SkipInit( out this ) ;
			this.DescriptorTable = descriptorTable ;
		}
		
		public _paramDataUnion( in RootConstants constants ) {
			Unsafe.SkipInit( out this ) ;
			this.Constants = constants ;
		}
		public _paramDataUnion( in RootDescriptor descriptor ) {
			Unsafe.SkipInit( out this ) ;
			this.Descriptor = descriptor ;
		}
		
		public static implicit operator _paramDataUnion( in RootDescriptorTable descriptorTable ) => new( descriptorTable ) ;
		public static implicit operator _paramDataUnion( in RootConstants constants ) => new( constants ) ;
	}

	public ref RootParameterType ParameterType {
		get {
			return ref Unsafe.AsRef< RootParameterType >( parameterType ) ;
		}
	}

	public ref ShaderVisibility ShaderVisibility {
		get {
			return ref Unsafe.AsRef< ShaderVisibility >( shaderVisibility ) ;
		}
	}
} ;


/// <summary>
/// Describes the root signature 1.0 layout of a descriptor table as a collection of descriptor
/// ranges that are all relative to a single base descriptor handle.
/// </summary>
/// <remarks>
/// <para>Samplers are not allowed in the same descriptor table as constant-buffer views (CBVs), unordered-access views (UAVs), and shader-resource views (SRVs).</para>
/// <para><b>D3D12_ROOT_DESCRIPTOR_TABLE</b> is the data type of the <b>DescriptorTable</b> member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_root_parameter">D3D12_ROOT_PARAMETER</a>. Use a <b>D3D12_ROOT_DESCRIPTOR_TABLE</b> when you set <b>D3D12_ROOT_PARAMETER</b>'s <b>ParameterType</b> member to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_root_parameter_type">D3D12_ROOT_PARAMETER_TYPE_DESCRIPTOR_TABLE</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_root_descriptor_table#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[StructLayout( LayoutKind.Sequential ),
 EquivalentOf( typeof( D3D12_ROOT_DESCRIPTOR_TABLE ) )]
public struct RootDescriptorTable {
	/// <summary>The number of ranges in the descriptor table.</summary>
	public uint NumDescriptorRanges ;

	/// <summary>
	/// An array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_range">D3D12_DESCRIPTOR_RANGE</a>
	/// structures that describe the descriptor ranges.
	/// </summary>
	public unsafe DescriptorRange* pDescriptorRanges ;
	[UnscopedRef] public unsafe Span< DescriptorRange > DescriptorRanges =>
									new( pDescriptorRanges, (int) NumDescriptorRanges ) ;
	
	
	public unsafe RootDescriptorTable( uint numRanges, DescriptorRange* pRanges ) {
		NumDescriptorRanges = numRanges ;
		pDescriptorRanges = pRanges ;
	}
	
	public RootDescriptorTable( uint numRanges, nint pRanges ) {
		NumDescriptorRanges = numRanges ;
		unsafe { pDescriptorRanges = (DescriptorRange *)pRanges ; }
	}
	
	public RootDescriptorTable( out MemoryHandle handle,
								params DescriptorRange[ ] ranges ) {
		Memory< DescriptorRange > memory = ranges ;
		handle = memory.Pin( ) ;
		NumDescriptorRanges = (uint)memory.Length ;
		unsafe { pDescriptorRanges = (DescriptorRange *)handle.Pointer ; }
	}
	
	public RootDescriptorTable( Memory< DescriptorRange > ranges ) {
		NumDescriptorRanges = (uint)ranges.Length ;
		unsafe {
			fixed ( DescriptorRange* pRanges = &ranges.Span[ 0 ] )
				pDescriptorRanges = pRanges ;
		}
	}
} ;


[StructLayout( LayoutKind.Sequential ),
 EquivalentOf( typeof( D3D12_ROOT_CONSTANTS ) )]
public struct RootConstants {
	/// <summary>The shader register that contains the constants.</summary>
	public uint ShaderRegister ;

	/// <summary>The register space. This is typically 0.</summary>
	public uint RegisterSpace ;

	/// <summary>
	/// <para>The number of constants that occupy a single shader slot (these constants appear like a single constant buffer). All constants occupy a single root signature bind slot.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_root_constants#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Num32BitValues ;
	
	
	public RootConstants( uint shaderRegister = 0,
						  uint registerSpace = 0,
						  uint num32BitValues = 0 ) {
		ShaderRegister = shaderRegister ;
		RegisterSpace = registerSpace ;
		Num32BitValues = num32BitValues ;
	}
} ;
	

[StructLayout( LayoutKind.Sequential ),
 EquivalentOf( typeof( D3D12_ROOT_DESCRIPTOR ) )]
public struct RootDescriptor {
	/// <summary>The shader register.</summary>
	public uint ShaderRegister;

	/// <summary>The register space.</summary>
	public uint RegisterSpace;
} ;


[StructLayout(LayoutKind.Sequential),
 EquivalentOf(typeof(D3D12_STATIC_SAMPLER_DESC))]
public struct StaticSamplerDescription {
	/// <summary>The filtering method to use when sampling a texture, as a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">D3D12_FILTER</a> enumeration constant.</summary>
	public Filter Filter ;

	/// <summary>Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">D3D12_TEXTURE_ADDRESS_MODE</a> mode to use for resolving a <i>u</i> texture coordinate that is outside the 0 to 1 range.</summary>
	public TextureAddressMode AddressU ;

	/// <summary>Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">D3D12_TEXTURE_ADDRESS_MODE</a> mode to use for resolving a <i>v</i> texture coordinate that is outside the 0 to 1 range.</summary>
	public TextureAddressMode AddressV ;

	/// <summary>Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">D3D12_TEXTURE_ADDRESS_MODE</a> mode to use for resolving a <i>w</i> texture coordinate that is outside the 0 to 1 range.</summary>
	public TextureAddressMode AddressW ;

	/// <summary>Offset from the calculated mipmap level. For example, if Direct3D calculates that a texture should be sampled at mipmap level 3 and MipLODBias is 2, then the texture will be sampled at mipmap level 5.</summary>
	public float MipLODBias ;

	/// <summary>Clamping value used if D3D12_FILTER_ANISOTROPIC or D3D12_FILTER_COMPARISON_ANISOTROPIC is specified as the filter. Valid values are between 1 and 16.</summary>
	public uint MaxAnisotropy ;

	/// <summary>
	/// <para>A function that compares sampled data against existing sampled data. The function options are listed in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_comparison_func">D3D12_COMPARISON_FUNC</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_static_sampler_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ComparisonFunction ComparisonFunc ;

	/// <summary>
	/// <para>One member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_static_border_color">D3D12_STATIC_BORDER_COLOR</a>, the border color to use if D3D12_TEXTURE_ADDRESS_MODE_BORDER is specified for AddressU, AddressV, or AddressW. Range must be between 0.0 and 1.0 inclusive.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_static_sampler_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public StaticBorderColor BorderColor ;

	/// <summary>Lower end of the mipmap range to clamp access to, where 0 is the largest and most detailed mipmap level and any level higher than that is less detailed.</summary>
	public float MinLOD ;

	/// <summary>Upper end of the mipmap range to clamp access to, where 0 is the largest and most detailed mipmap level and any level higher than that is less detailed. This value must be greater than or equal to MinLOD. To have no upper limit on LOD set this to a large value such as D3D12_FLOAT32_MAX.</summary>
	public float MaxLOD ;

	/// <summary>
	/// <para>The <i>ShaderRegister</i> and <i>RegisterSpace</i> parameters correspond to the binding syntax of HLSL.  For example, in HLSL:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_static_sampler_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint ShaderRegister ;

	/// <summary>
	/// <para>See the description for <i>ShaderRegister</i>. Register space is optional; the default register space is 0.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_static_sampler_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint RegisterSpace ;

	/// <summary>Specifies the visibility of the sampler to the pipeline shaders, one member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_shader_visibility">D3D12_SHADER_VISIBILITY</a>.</summary>
	public ShaderVisibility ShaderVisibility ;
	
	
	public StaticSamplerDescription( Filter filter = Filter.MinMagMipPoint,
									 TextureAddressMode addressU = TextureAddressMode.Clamp,
									 TextureAddressMode addressV = TextureAddressMode.Clamp,
									 TextureAddressMode addressW = TextureAddressMode.Clamp,
									 float mipLODBias = 0.0f,
									 uint maxAnisotropy = 0,
									 ComparisonFunction comparisonFunc = ComparisonFunction.Never,
									 StaticBorderColor borderColor = StaticBorderColor.TransparentBlack,
									 float minLOD = 0.0f,
									 float maxLOD = 0.0f,
									 uint shaderRegister = 0,
									 uint registerSpace = 0,
									 ShaderVisibility shaderVisibility = ShaderVisibility.All ) {
		Filter = filter ;
		AddressU = addressU ;
		AddressV = addressV ;
		AddressW = addressW ;
		MipLODBias = mipLODBias ;
		MaxAnisotropy = maxAnisotropy ;
		ComparisonFunc = comparisonFunc ;
		BorderColor = borderColor ;
		MinLOD = minLOD ;
		MaxLOD = maxLOD ;
		ShaderRegister = shaderRegister ;
		RegisterSpace = registerSpace ;
		ShaderVisibility = shaderVisibility ;
	}
}


[StructLayout( LayoutKind.Sequential ),
 EquivalentOf( typeof( D3D12_DESCRIPTOR_RANGE ) )]
public struct DescriptorRange {
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_range_type">D3D12_DESCRIPTOR_RANGE_TYPE</a>-typed
	/// value that specifies the type of descriptor range.
	/// </summary>
	public DescriptorRangeType RangeType ;

	/// <summary>
	/// The number of descriptors in the range. Use -1 or UINT_MAX to specify an unbounded size.
	/// If a given descriptor range is unbounded, then it must either be the last range in the table definition,
	/// or else the following range in the table definition must have a value for *OffsetInDescriptorsFromTableStart*
	/// that is not [D3D12_DESCRIPTOR_RANGE_OFFSET_APPEND]().
	/// </summary>
	public uint NumDescriptors ;

	/// <summary>
	/// The base shader register in the range.
	/// For example, for shader-resource views (SRVs), 3 maps to ": register(t3);" in HLSL.
	/// </summary>
	public uint BaseShaderRegister ;

	/// <summary>
	/// <para>The register space. Can typically be 0, but allows multiple descriptor  arrays of unknown size to not appear to overlap.
	/// For example, for SRVs, by extending the example in the <b>BaseShaderRegister</b> member description, 5 maps to ": register(t3,space5);" in HLSL.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_descriptor_range#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint RegisterSpace ;

	/// <summary>
	/// The offset in descriptors, from the start of the descriptor table which was set as the root argument value for this parameter slot.
	/// This value can be <b>D3D12_DESCRIPTOR_RANGE_OFFSET_APPEND</b>, which indicates this range should immediately follow the preceding range.
	/// </summary>
	public uint OffsetInDescriptorsFromTableStart ;
	
	
	
	public DescriptorRange( DescriptorRangeType rangeType =
								DescriptorRangeType.Sampler,
							uint numDescriptors = 0,
							uint baseShaderRegister = 0,
							uint registerSpace = 0,
							uint offsetInDescriptorsFromTableStart = 0 ) {
		RangeType = rangeType ;
		NumDescriptors = numDescriptors ;
		BaseShaderRegister = baseShaderRegister ;
		RegisterSpace = registerSpace ;
		OffsetInDescriptorsFromTableStart = offsetInDescriptorsFromTableStart ;
	}
} ;



//! ------- Description 1.1: ----------------------------------



/// <summary>Describes the layout of a root signature version 1.1.</summary>
/// <remarks>
/// Use this structure with the
/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_versioned_root_signature_desc">D3D12_VERSIONED_ROOT_SIGNATURE_DESC</a>
/// structure.
/// </remarks>
[EquivalentOf( typeof( D3D12_ROOT_SIGNATURE_DESC1 ) )]
public partial struct RootSignatureDescription1 {
	/// <summary>The number of slots in the root signature. This number is also the number of elements in the <i>pParameters</i> array.</summary>
	public uint NumParameters ;

	/// <summary>
	/// An array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_root_parameter1">D3D12_ROOT_PARAMETER1</a> structures
	/// for the slots in the root signature.
	/// </summary>
	public unsafe RootParameter1* pParameters ;
	[UnscopedRef] public unsafe Span< RootParameter1 > Parameters =>
									new( pParameters, (int) NumParameters ) ;
	
	/// <summary>Specifies the number of static samplers.</summary>
	public uint NumStaticSamplers ;

	/// <summary>Pointer to one or more <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_static_sampler_desc">D3D12_STATIC_SAMPLER_DESC</a> structures.</summary>
	public unsafe StaticSamplerDescription* pStaticSamplers ;
	[UnscopedRef] public unsafe Span< StaticSamplerDescription > StaticSamplers =>
									new( pStaticSamplers, (int) NumStaticSamplers ) ;

	/// <summary>
	/// Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_root_signature_flags">D3D12_ROOT_SIGNATURE_FLAGS</a>
	/// that determine the data volatility.
	/// </summary>
	public RootSignatureFlags Flags ;
	
	
	public unsafe RootSignatureDescription1( uint numParameters = 0U,
											 RootParameter1* pParameters = null,
											 uint numStaticSamplers = 0U,
											 StaticSamplerDescription* pStaticSamplers = null,
											 RootSignatureFlags flags = RootSignatureFlags.None ) {
		NumParameters = numParameters ;
		this.pParameters = pParameters ;
		NumStaticSamplers = numStaticSamplers ;
		this.pStaticSamplers = pStaticSamplers ;
		Flags = flags ;
	}

	
	public RootSignatureDescription1( out MemoryHandle[ ] handles,
									  RootParameter1[ ]? rootParams = null,
									  StaticSamplerDescription[ ]? staticSamplers = null,
									  RootSignatureFlags flags = RootSignatureFlags.None ) {
		Memory< StaticSamplerDescription > staticSamplerMem = staticSamplers ;
		Memory< RootParameter1 > rootParamMem = rootParams ;
		NumStaticSamplers = (uint) staticSamplerMem.Length ;
		NumParameters = (uint) rootParamMem.Length ;

		//! Pin the memory and create the handles:
		handles = new MemoryHandle[ 2 ] ;
		unsafe {
			var hSamplers = handles[ 1 ] = staticSamplerMem.Pin( ) ;
			var hParams   = handles[ 0 ] = rootParamMem.Pin( ) ;
			pStaticSamplers = (StaticSamplerDescription *)hSamplers.Pointer ;
			pParameters     = (RootParameter1 *)hParams.Pointer ;
		}
	}
} ;


/// <summary>Describes the slot of a root signature version 1.1.</summary>
/// <remarks>
/// <para>Use this structure with the
/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_root_signature_desc1">D3D12_ROOT_SIGNATURE_DESC1</a> structure.
/// Refer to the helper structure <a href="https://docs.microsoft.com/windows/desktop/direct3d12/cd3dx12-root-parameter1">CD3DX12_ROOT_PARAMETER1</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_root_parameter1#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_ROOT_PARAMETER1 ) )]
public partial struct RootParameter1: IRootParam1 {
	
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_root_parameter_type">D3D12_ROOT_PARAMETER_TYPE</a>-typed value
	/// that  specifies the type of root signature slot. This member determines which type to use in the union below.
	/// </summary>
	public RootParameterType parameterType ;
	
	/// <summary>Union containing the root parameter data.</summary>
	public _rootDataUnion parameterData ;
	
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_shader_visibility">D3D12_SHADER_VISIBILITY</a>-typed value
	/// that  specifies the shaders that can access the contents of the root signature slot.
	/// </summary>
	public ShaderVisibility shaderVisibility ;
	
	
	public ref RootParameterType ParameterType {
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		get => ref Unsafe.AsRef< RootParameterType >( parameterType ) ;
	}
	public ref ShaderVisibility ShaderVisibility {
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		get => ref Unsafe.AsRef< ShaderVisibility >( shaderVisibility ) ;
	}
	
	
	/// <summary>
	/// Data union which stores different types of root parameter data.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public partial struct _rootDataUnion {
		[FieldOffset(0)] public RootDescriptorTable1 DescriptorTable ;
		[FieldOffset(0)] public RootConstants Constants ;
		[FieldOffset(0)] public RootDescriptor1 Descriptor ;
		
		public _rootDataUnion( in RootDescriptorTable1 descTable ) {
			Unsafe.SkipInit( out this ) ;
			DescriptorTable = descTable ;
		}
		public _rootDataUnion( in RootConstants constants ) {
			Unsafe.SkipInit( out this ) ;
			Constants = constants ;
		}
		public _rootDataUnion( in RootDescriptor1 desc ) {
			Unsafe.SkipInit( out this ) ;
			Descriptor = desc ;
		}
		
		public static implicit operator _rootDataUnion( in RootDescriptorTable1 descTable ) => new( descTable ) ;
		public static implicit operator _rootDataUnion( in RootConstants constants ) => new( constants ) ;
		public static implicit operator _rootDataUnion( in RootDescriptor1 desc ) => new( desc ) ;
	}
	
	
	public RootParameter1( in RootDescriptorTable1 descriptorTable,
						   ShaderVisibility shaderVisibility = ShaderVisibility.All ) {
		parameterType    = RootParameterType.DescriptorTable ;
		parameterData  = descriptorTable ;
		shaderVisibility = shaderVisibility ;
	}
	
	public RootParameter1( in RootConstants constants,
						   ShaderVisibility shaderVisibility = ShaderVisibility.All ) {
		parameterType    = RootParameterType.Const32Bits ;
		parameterData  = constants ;
		shaderVisibility = shaderVisibility ;
	}
	
	public RootParameter1( in RootDescriptor1 descriptor,
						   RootParameterType type = RootParameterType.DescriptorTable,
						   ShaderVisibility   shaderVisibility = ShaderVisibility.All ) {
		parameterType    = type ;
		parameterData  = descriptor ;
		shaderVisibility = shaderVisibility ;
	}
} ;


/// <summary>Describes descriptors inline in the root signature version 1.1 that appear in shaders.</summary>
/// <remarks>
/// <para><b>D3D12_ROOT_DESCRIPTOR1</b> is the data type of the <b>Descriptor</b> member of
/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_root_parameter1">D3D12_ROOT_PARAMETER1</a>.
/// Use a <b>D3D12_ROOT_DESCRIPTOR1</b> when you set <b>D3D12_ROOT_PARAMETER1</b>'s <b>ParameterType</b> field to the
/// D3D12_ROOT_PARAMETER_TYPE_CBV, D3D12_ROOT_PARAMETER_TYPE_SRV, or D3D12_ROOT_PARAMETER_TYPE_UAV members of
/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_root_parameter_type">D3D12_ROOT_PARAMETER_TYPE</a>.</para>
/// <para>Refer to the helper structure <a href="https://docs.microsoft.com/windows/desktop/direct3d12/cd3dx12-root-descriptor1">CD3DX12_ROOT_DESCRIPTOR1</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_root_descriptor1#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_ROOT_DESCRIPTOR1))]
public partial struct RootDescriptor1 {
	/// <summary>The shader register.</summary>
	public uint ShaderRegister ;

	/// <summary>The register space.</summary>
	public uint RegisterSpace ;

	/// <summary>
	/// Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_root_descriptor_flags">D3D12_ROOT_DESCRIPTOR_FLAGS</a>
	/// that determine the volatility of descriptors and the data they reference.
	/// </summary>
	public RootDescriptorFlags Flags ;
	
	
	public RootDescriptor1( uint shaderRegister = 0U, 
							uint registerSpace = 0U, 
							RootDescriptorFlags flags =
								RootDescriptorFlags.None ) {
		ShaderRegister = shaderRegister ;
		RegisterSpace = registerSpace ;
		Flags = flags ;
	}
	
	
	public static implicit operator RootDescriptor1( in ( uint shaderRegister, uint registerSpace, RootDescriptorFlags flags ) tuple ) =>
								new( tuple.shaderRegister, tuple.registerSpace, tuple.flags ) ;
} ;



//! ------- Description 1.2: ----------------------------------

[EquivalentOf( typeof( D3D12_ROOT_SIGNATURE_DESC2 ) )]
public partial struct RootSignatureDescription2 {
	public uint NumParameters ;
	public unsafe RootParameter1* pParameters ;
	[UnscopedRef] public unsafe Span< RootParameter1 > Parameters =>
									new( pParameters, (int) NumParameters ) ;
	
	public uint NumStaticSamplers ;
	public unsafe StaticSamplerDescription1* pStaticSamplers ;
	[UnscopedRef] public unsafe Span< StaticSamplerDescription1 > StaticSamplers =>
									new( pStaticSamplers, (int) NumStaticSamplers ) ;
	
	public RootSignatureFlags Flags ;
	
	
	
	public unsafe RootSignatureDescription2( uint numParameters = 0U,
											 RootParameter1* pParameters = null,
											 uint numStaticSamplers = 0U,
											 StaticSamplerDescription1* pStaticSamplers = null,
											 RootSignatureFlags flags = RootSignatureFlags.None ) {
		NumParameters = numParameters ;
		this.pParameters = pParameters ;
		NumStaticSamplers = numStaticSamplers ;
		this.pStaticSamplers = pStaticSamplers ;
		Flags = flags ;
	}
	
	public RootSignatureDescription2( out MemoryHandle[ ] handles,
									  RootParameter1[ ]? rootParams = null,
									  StaticSamplerDescription1[ ]? staticSamplers = null,
									  RootSignatureFlags flags = RootSignatureFlags.None ) {
		Memory< StaticSamplerDescription1 > staticSamplerMem = staticSamplers ;
		Memory< RootParameter1 > rootParamMem = rootParams ;
		NumStaticSamplers = (uint) staticSamplerMem.Length ;
		NumParameters = (uint) rootParamMem.Length ;

		//! Pin the memory and create the handles:
		handles = new MemoryHandle[ 2 ] ;
		unsafe {
			var hSamplers = handles[ 1 ] = staticSamplerMem.Pin( ) ;
			var hParams   = handles[ 0 ] = rootParamMem.Pin( ) ;
			pStaticSamplers = (StaticSamplerDescription1 *)hSamplers.Pointer ;
			pParameters     = (RootParameter1 *)hParams.Pointer ;
		}
	}
	
} ;


// -----------------------------------------------------------------------------------------------------------------
// VersionedRootSignatureDescription:
// -----------------------------------------------------------------------------------------------------------------


/// <summary>Holds any version of a root signature description, and is designed to be used with serialization/deserialization functions.</summary>
/// <remarks>
/// <para>Use this structure with the following methods.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_versioned_root_signature_desc#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_VERSIONED_ROOT_SIGNATURE_DESC))]
public partial struct VersionedRootSignatureDescription {
	/// <summary>Specifies one member of D3D_ROOT_SIGNATURE_VERSION that determines the contents of the union.</summary>
	public RootSignatureVersion Version ;

	/// <summary>Access to the data union containing a description.</summary>
	public _rootSignatureUnion DescriptionData ;
	
	
	/// <summary>Data union that can contain a root signature description.</summary>
	[StructLayout( LayoutKind.Explicit )]
	public partial struct _rootSignatureUnion {
		[FieldOffset(0)] public RootSignatureDescription  Desc_1_0 ;
		[FieldOffset(0)] public RootSignatureDescription1 Desc_1_1 ;
		[FieldOffset(0)] public RootSignatureDescription2 Desc_1_2 ;
		
		public _rootSignatureUnion( in RootSignatureDescription desc ) {
			Unsafe.SkipInit( out this ) ;
			Desc_1_0 = desc ;
		}
		public _rootSignatureUnion( in RootSignatureDescription1 desc ) {
			Unsafe.SkipInit( out this ) ;
			Desc_1_1 = desc ;
		}
		public _rootSignatureUnion( in RootSignatureDescription2 desc ) {
			Unsafe.SkipInit( out this ) ;
			Desc_1_2 = desc ;
		}
		
		public static implicit operator _rootSignatureUnion( in RootSignatureDescription desc ) => new( desc ) ;
		public static implicit operator _rootSignatureUnion( in RootSignatureDescription1 desc ) => new( desc ) ;
		public static implicit operator _rootSignatureUnion( in RootSignatureDescription2 desc ) => new( desc ) ;
	}
} ;

