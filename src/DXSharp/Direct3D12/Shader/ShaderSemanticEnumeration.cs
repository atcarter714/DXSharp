#region Using Directives
using System.Collections.ObjectModel ;
using System.Globalization ;
using DXSharp.XTensions ;
#endregion
namespace DXSharp.Direct3D12.Shader ;


/// <summary>
/// Defines bitmask flags/enumeration constant values for
/// the semantics of a Direct3D shader input or output.
/// </summary>
[Flags] public enum ShaderSemantic: ulong {
	// ------------------------------------------------------------------------------
	// Common Semantics:
	// ------------------------------------------------------------------------------
	Binormal     = 1ul << 0,
	BlendIndices = 1ul << 1,
	BlendWeight  = 1ul << 2,
	Color        = 1ul << 3,
	Normal       = 1ul << 4,
	PositionN    = 1ul << 5,
	PositionT    = 1ul << 6,
	PSize        = 1ul << 7,
	Tangent      = 1ul << 8,
	TexCoord     = 1ul << 9,
	
	// ------------------------------------------------------------------------------
	// SV Semantics:
	// ------------------------------------------------------------------------------
	ClipDistance           = 1ul << 10,
	CullDistance           = 1ul << 11,
	Coverage               = 1ul << 12,
	Depth                  = 1ul << 13,
	DepthGreaterEqual      = 1ul << 14,
	DepthLessEqual         = 1ul << 15,
	DispatchThreadID       = 1ul << 16,
	DomainLocation         = 1ul << 17,
	GroupID                = 1ul << 18,
	GroupIndex             = 1ul << 19,
	GroupThreadID          = 1ul << 20,
	GSInstanceID           = 1ul << 21,
	InnerCoverage          = 1ul << 22,
	InsideTessFactor       = 1ul << 23,
	InstanceID             = 1ul << 24,
	IsFrontFace            = 1ul << 25,
	OutputControlPointID   = 1ul << 26,
	Position               = 1ul << 27,
	PrimitiveID            = 1ul << 28,
	RenderTargetArrayIndex = 1ul << 29,
	SampleIndex            = 1ul << 30,
	StencilRef             = 1ul << 31,
	Target                 = 1ul << 32,
	VertexID               = 1ul << 33,
	ViewportArrayIndex     = 1ul << 34,
	ShadingRate            = 1ul << 35,
	// ------------------------------------------------------------------------------
} ;
