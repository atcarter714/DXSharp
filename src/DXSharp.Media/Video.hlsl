cbuffer Fit : register(b0) { float2 Scale; };
Texture2D<float4> Video : register(t0);
SamplerState LinearClamp : register(s0);
struct Vertex { float4 Position : SV_Position; float2 UV : TEXCOORD; };
Vertex VS(uint id : SV_VertexID) {
    const float2 points[6] = { float2(-1,1),float2(1,1),float2(-1,-1),float2(-1,-1),float2(1,1),float2(1,-1) };
    Vertex result; float2 p = points[id]; result.Position = float4(p * Scale,0,1); result.UV = float2(p.x*.5+.5,.5-p.y*.5); return result;
}
float4 PS(Vertex input) : SV_Target { return float4(Video.Sample(LinearClamp,input.UV).rgb,1); }
