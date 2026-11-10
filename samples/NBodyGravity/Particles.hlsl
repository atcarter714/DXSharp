struct Body {
    float4 positionMass;
    float4 velocity;
};

StructuredBuffer<Body> Bodies : register(t0);
cbuffer View : register(b0) {
    float aspect;
};

struct Vertex {
    float4 position : SV_Position;
    float2 local : TEXCOORD;
    float3 color : COLOR;
};

Vertex VS(uint vertex : SV_VertexID, uint instance : SV_InstanceID) {
    const float2 corners[6] = {
        float2(-1, -1), float2(-1, 1), float2(1, -1),
        float2(1, -1), float2(-1, 1), float2(1, 1)
    };
    Body body = Bodies[instance];
    float2 center = body.positionMass.xy;
    center.y += body.positionMass.z * 0.35;
    float2 scale = float2(min(1, 1 / aspect), min(1, aspect));

    Vertex result;
    result.local = corners[vertex];
    float radius = 0.006 + 0.003 * body.positionMass.w;
    result.position = float4((center * 0.22 + result.local * radius) * scale, 0, 1);
    result.color = lerp(float3(0.19, 0.65, 1), float3(1, 0.35, 0.17), saturate(body.positionMass.w - 0.5));
    return result;
}

float4 PS(Vertex vertex) : SV_Target {
    float radius = length(vertex.local);
    clip(1 - radius);
    return float4(vertex.color * (0.25 + 0.75 * (1 - radius)), 1);
}
