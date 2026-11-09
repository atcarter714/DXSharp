struct Body {
    float4 positionMass;
    float4 velocity;
};

RWStructuredBuffer<Body> Source : register(u0);
RWStructuredBuffer<Body> Destination : register(u1);
cbuffer Simulation : register(b0) {
    uint bodyCount;
    float stepSize;
    float gravity;
    float softeningSquared;
};

[numthreads(64, 1, 1)]
void Step(uint3 thread : SV_DispatchThreadID) {
    uint index = thread.x;
    if (index >= bodyCount) return;

    Body body = Source[index];
    float3 acceleration = 0;
    for (uint other = 0; other < bodyCount; ++other) {
        float4 neighbor = Source[other].positionMass;
        float3 displacement = neighbor.xyz - body.positionMass.xyz;
        float inverseDistance = rsqrt(dot(displacement, displacement) + softeningSquared);
        acceleration += displacement * (neighbor.w * inverseDistance * inverseDistance * inverseDistance);
    }

    body.velocity.xyz += acceleration * (gravity * stepSize);
    body.positionMass.xyz += body.velocity.xyz * stepSize;
    Destination[index] = body;
}
