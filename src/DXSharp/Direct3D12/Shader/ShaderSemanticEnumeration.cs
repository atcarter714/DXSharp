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
	[Semantic(nameof(Binormal), hasIndex: true)]     Binormal     = 1ul << 0,
	[Semantic(nameof(BlendIndices), hasIndex: true)] BlendIndices = 1ul << 1,
	[Semantic(nameof(BlendWeight), hasIndex: true)]  BlendWeight  = 1ul << 2,
	[Semantic(nameof(Color), hasIndex: true)]        Color        = 1ul << 3,
	[Semantic(nameof(Normal), hasIndex: true)]       Normal       = 1ul << 4,
	[Semantic(nameof(PositionN), hasIndex: true)]    PositionN    = 1ul << 5,
	[Semantic(nameof(PositionT))]                    PositionT    = 1ul << 6,
	[Semantic(nameof(PSize), hasIndex: true)]        PSize        = 1ul << 7,
	[Semantic(nameof(Tangent), hasIndex: true)]      Tangent      = 1ul << 8,
	[Semantic(nameof(TexCoord), hasIndex: true)]     TexCoord     = 1ul << 9,
	
	// ------------------------------------------------------------------------------
	// System Value (SV) Semantics:
	// ------------------------------------------------------------------------------
	[SystemValue(nameof(ClipDistance), true)] ClipDistance       = 1ul << 10,
	[SystemValue(nameof(CullDistance), true)] CullDistance       = 1ul << 11,
	[SystemValue(nameof(Coverage))]               Coverage               = 1ul << 12,
	[SystemValue(nameof(Depth))]                  Depth                  = 1ul << 13,
	[SystemValue(nameof(DepthGreaterEqual))]      DepthGreaterEqual      = 1ul << 14,
	[SystemValue(nameof(DepthLessEqual))]         DepthLessEqual         = 1ul << 15,
	[SystemValue(nameof(DispatchThreadID))]       DispatchThreadID       = 1ul << 16,
	[SystemValue(nameof(DomainLocation))]         DomainLocation         = 1ul << 17,
	[SystemValue(nameof(GroupID))]                GroupID                = 1ul << 18,
	[SystemValue(nameof(GroupIndex))]             GroupIndex             = 1ul << 19,
	[SystemValue(nameof(GroupThreadID))]          GroupThreadID          = 1ul << 20,
	[SystemValue(nameof(GSInstanceID))]           GSInstanceID           = 1ul << 21,
	[SystemValue(nameof(InnerCoverage))]          InnerCoverage          = 1ul << 22,
	[SystemValue(nameof(InsideTessFactor))]       InsideTessFactor       = 1ul << 23,
	[SystemValue(nameof(InstanceID))]             InstanceID             = 1ul << 24,
	[SystemValue(nameof(IsFrontFace))]            IsFrontFace            = 1ul << 25,
	[SystemValue(nameof(OutputControlPointID))]   OutputControlPointID   = 1ul << 26,
	[SystemValue(nameof(Position))]               Position               = 1ul << 27,
	[SystemValue(nameof(PrimitiveID))]            PrimitiveID            = 1ul << 28,
	[SystemValue(nameof(RenderTargetArrayIndex))] RenderTargetArrayIndex = 1ul << 29,
	[SystemValue(nameof(SampleIndex))]            SampleIndex            = 1ul << 30,
	[SystemValue(nameof(StencilRef))]             StencilRef             = 1ul << 31,
	[SystemValue(nameof(Target), true)]   Target                 = 1ul << 32,
	[SystemValue(nameof(VertexID))]               VertexID               = 1ul << 33,
	[SystemValue(nameof(ViewportArrayIndex))]     ViewportArrayIndex     = 1ul << 34,
	[SystemValue(nameof(ShadingRate))]            ShadingRate            = 1ul << 35,
	// ------------------------------------------------------------------------------
} ;
