#region Using Directives
#pragma warning disable CS8981, CS1591
using System ;
using System.Runtime.InteropServices ;
using System.Runtime.CompilerServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.System.SystemInformation ;

#endregion
namespace DXSharp ;



/// <summary>Enumeration representing the major GPU vendors.</summary>
public enum GPUVendor: uint {
	Unknown = 0x0000U,
	AMD     = 0x1002U,
	Intel   = 0x8086U,
	Nvidia  = 0x10DEU,
} ;

public enum CPUVendor: uint {
	Unknown = 0x0000U,
	AMD     = 0x1002U,
	Intel   = 0x8086U,
	ARM     = 0x13B5U,
} ;



/// <summary>Describes the manufacturer and architecture of the processor.</summary>

[EquivalentOf( typeof( PROCESSOR_ARCHITECTURE ) )]
public enum ProcessorArchitecture: ushort {
	Intel        = 0,
	Mips         = 1,
	Alpha        = 2,
	PPC          = 3,
	SHX          = 4,
	ARM          = 5,
	IA64         = 6,
	Alpha64      = 7,
	MSIL         = 8,
	AMD64        = 9,
	IA32OnWin64  = 10,
	Neutral      = 11,
	ARM64        = 12,
	ARM32OnWin64 = 13,
	IA32OnArm64  = 14,
	Unknown      = 65535,
} ;


[EquivalentOf( typeof(D3D_PRIMITIVE_TOPOLOGY) )]
public enum PrimitiveTopology {
	/// <summary>The IA stage has not been initialized with a primitive topology. The IA stage will not function properly unless a primitive topology is defined.</summary>
	D3D_Undefined = 0,

	/// <summary>Interpret the vertex data as a list of points.</summary>
	D3D_PointList = 1,

	/// <summary>Interpret the vertex data as a list of lines.</summary>
	D3D_LineList = 2,

	/// <summary>Interpret the vertex data as a line strip.</summary>
	D3D_LineStrip = 3,

	/// <summary>Interpret the vertex data as a list of triangles.</summary>
	D3D_TriangleList = 4,

	/// <summary>Interpret the vertex data as a triangle strip.</summary>
	D3D_TriangleStrip = 5,
	D3D_TriangleFan = 6,

	/// <summary>Interpret the vertex data as a list of lines with adjacency data.</summary>
	D3D_LineList_ADJ = 10,

	/// <summary>Interpret the vertex data as a line strip with adjacency data.</summary>
	D3D_LineStrip_ADJ = 11,

	/// <summary>Interpret the vertex data as a list of triangles with adjacency data.</summary>
	D3D_TriangleList_ADJ = 12,

	/// <summary>Interpret the vertex data as a triangle strip with adjacency data.</summary>
	D3D_TriangleStrip_ADJ = 13,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_1xControlPoint_PatchList = 33,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_2xControlPoint_PatchList = 34,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_3xControlPoint_PatchList = 35,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_4xControlPoint_PatchList = 36,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_5xControlPoint_PatchList = 37,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_6xControlPoint_PatchList = 38,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_7xControlPoint_PatchList = 39,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_8xControlPoint_PatchList = 40,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_9xControlPoint_PatchList = 41,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_10xControlPoint_PatchList = 42,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_11xControlPoint_PatchList = 43,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_12xControlPoint_PatchList = 44,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_13xControlPoint_PatchList = 45,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_14xControlPoint_PatchList = 46,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_15xControlPoint_PatchList = 47,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_16xControlPoint_PatchList = 48,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_17xControlPoint_PatchList = 49,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_18xControlPoint_PatchList = 50,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_19xControlPoint_PatchList = 51,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_20xControlPoint_PatchList = 52,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_21xControlPoint_PatchList = 53,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_22xControlPoint_PatchList = 54,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_23xControlPoint_PatchList = 55,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_24xControlPoint_PatchList = 56,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_25xControlPoint_PatchList = 57,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_26xControlPoint_PatchList = 58,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_27xControlPoint_PatchList = 59,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_28xControlPoint_PatchList = 60,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_29xControlPoint_PatchList = 61,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_30xControlPoint_PatchList = 62,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_31xControlPoint_PatchList = 63,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D_32xControlPoint_PatchList = 64,

	/// <summary>The IA stage has not been initialized with a primitive topology. The IA stage will not function properly unless a primitive topology is defined.</summary>
	D3D10_Undefined = 0,

	/// <summary>Interpret the vertex data as a list of points.</summary>
	D3D10_PointList = 1,

	/// <summary>Interpret the vertex data as a list of lines.</summary>
	D3D10_LineList = 2,

	/// <summary>Interpret the vertex data as a line strip.</summary>
	D3D10_LineStrip = 3,

	/// <summary>Interpret the vertex data as a list of triangles.</summary>
	D3D10_TriangleList = 4,

	/// <summary>Interpret the vertex data as a triangle strip.</summary>
	D3D10_TriangleStrip = 5,

	/// <summary>Interpret the vertex data as a list of lines with adjacency data.</summary>
	D3D10_LineList_ADJ = 10,

	/// <summary>Interpret the vertex data as a line strip with adjacency data.</summary>
	D3D10_LineStrip_ADJ = 11,

	/// <summary>Interpret the vertex data as a list of triangles with adjacency data.</summary>
	D3D10_TriangleList_ADJ = 12,

	/// <summary>Interpret the vertex data as a triangle strip with adjacency data.</summary>
	D3D10_TriangleStrip_ADJ = 13,

	/// <summary>The IA stage has not been initialized with a primitive topology. The IA stage will not function properly unless a primitive topology is defined.</summary>
	D3D11_Undefined = 0,

	/// <summary>Interpret the vertex data as a list of points.</summary>
	D3D11_PointList = 1,

	/// <summary>Interpret the vertex data as a list of lines.</summary>
	D3D11_LineList = 2,

	/// <summary>Interpret the vertex data as a line strip.</summary>
	D3D11_LineStrip = 3,

	/// <summary>Interpret the vertex data as a list of triangles.</summary>
	D3D11_TriangleList = 4,

	/// <summary>Interpret the vertex data as a triangle strip.</summary>
	D3D11_TriangleStrip = 5,

	/// <summary>Interpret the vertex data as a list of lines with adjacency data.</summary>
	D3D11_LineList_ADJ = 10,

	/// <summary>Interpret the vertex data as a line strip with adjacency data.</summary>
	D3D11_LineStrip_ADJ = 11,

	/// <summary>Interpret the vertex data as a list of triangles with adjacency data.</summary>
	D3D11_TriangleList_ADJ = 12,

	/// <summary>Interpret the vertex data as a triangle strip with adjacency data.</summary>
	D3D11_TriangleStrip_ADJ = 13,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_1xControlPoint_PatchList = 33,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_2xControlPoint_PatchList = 34,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_3xControlPoint_PatchList = 35,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_4xControlPoint_PatchList = 36,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_5xControlPoint_PatchList = 37,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_6xControlPoint_PatchList = 38,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_7xControlPoint_PatchList = 39,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_8xControlPoint_PatchList = 40,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_9xControlPoint_PatchList = 41,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_10xControlPoint_PatchList = 42,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_11xControlPoint_PatchList = 43,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_12xControlPoint_PatchList = 44,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_13xControlPoint_PatchList = 45,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_14xControlPoint_PatchList = 46,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_15xControlPoint_PatchList = 47,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_16xControlPoint_PatchList = 48,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_17xControlPoint_PatchList = 49,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_18xControlPoint_PatchList = 50,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_19xControlPoint_PatchList = 51,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_20xControlPoint_PatchList = 52,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_21xControlPoint_PatchList = 53,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_22xControlPoint_PatchList = 54,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_23xControlPoint_PatchList = 55,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_24xControlPoint_PatchList = 56,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_25xControlPoint_PatchList = 57,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_26xControlPoint_PatchList = 58,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_27xControlPoint_PatchList = 59,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_28xControlPoint_PatchList = 60,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_29xControlPoint_PatchList = 61,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_30xControlPoint_PatchList = 62,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_31xControlPoint_PatchList = 63,

	/// <summary>Interpret the vertex data as a patch list.</summary>
	D3D11_32xControlPoint_PatchList = 64,
} ;



