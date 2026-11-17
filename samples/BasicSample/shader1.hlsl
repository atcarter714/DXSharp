struct Vertex { float4 position : SV_Position; float3 color : COLOR; };
Vertex VS(uint id : SV_VertexID) {
    const float2 positions[3] = { float2(0,0.7), float2(0.7,-0.7), float2(-0.7,-0.7) };
    const float3 colors[3] = { float3(1,0.2,0.15), float3(0.15,1,0.4), float3(0.2,0.4,1) };
    Vertex vertex;
    vertex.position = float4(positions[id],0,1);
    vertex.color = colors[id];
    return vertex;
}
float4 PS(Vertex vertex) : SV_Target { return float4(vertex.color,1); }
