RWStructuredBuffer<uint> Image : register(u0);
RaytracingAccelerationStructure Scene : register(t0);

struct Payload {
    float3 color;
};

[shader("raygeneration")]
void RayGen() {
    uint2 pixel = DispatchRaysIndex().xy;
    uint2 dimensions = DispatchRaysDimensions().xy;
    float2 screen = ((float2(pixel) + 0.5) / dimensions) * 2 - 1;

    RayDesc ray;
    ray.Origin = float3(screen.x, -screen.y, -1);
    ray.Direction = float3(0, 0, 1);
    ray.TMin = 0;
    ray.TMax = 4;

    Payload payload = (Payload)0;
    TraceRay(Scene, RAY_FLAG_NONE, 0xff, 0, 1, 0, ray, payload);
    uint3 rgb = (uint3)(saturate(payload.color) * 255 + 0.5);
    Image[pixel.y * dimensions.x + pixel.x] = rgb.r | (rgb.g << 8) | (rgb.b << 16) | 0xff000000;
}

[shader("miss")]
void Miss(inout Payload payload) {
    payload.color = float3(0.04, 0.06, 0.1);
}

[shader("closesthit")]
void ClosestHit(inout Payload payload, BuiltInTriangleIntersectionAttributes attributes) {
    float2 barycentrics = attributes.barycentrics;
    payload.color = float3(1 - barycentrics.x - barycentrics.y, barycentrics);
}
