// Compact cargo silhouettes for autonomous traffic. Tactical patrol geometry uses its own draw.
cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; };
struct Input { float2 center : TEXCOORD0; float2 heading : TEXCOORD1; float4 motion : TEXCOORD3; };
struct Output { float4 position : SV_Position; float3 tint : COLOR; };

Output VS(Input input, uint id : SV_VertexID) {
    const float2 corners[6] = {
        float2(-1,-1),float2(1,-1),float2(-1,1),float2(-1,1),float2(1,-1),float2(1,1)
    };
    const float4 modules[4] = {
        float4(0,0.42,0.32,0.28), float4(0,-0.08,0.13,0.61),
        float4(-0.30,-0.32,0.19,0.48),float4(0.30,-0.32,0.19,0.48)
    };
    float4 cargo = modules[id/6];
    float2 local = cargo.xy + corners[id%6]*cargo.zw;
    float2 right = float2(-input.heading.y,input.heading.x);
    float2 world = input.center + (right*local.x + input.heading*local.y)*1.85;
    Output output;
    output.position = mul(float4(world.x,0.08+float(id/6)*0.004,world.y,1),ViewProjection);
    output.tint = (id < 6 ? float3(0.37,0.79,0.69) : float3(0.36,0.52,0.48)) *
        (0.76 + 0.24 * input.motion.y);
    return output;
}

float4 PS(Output input) : SV_Target { return float4(input.tint,1); }
