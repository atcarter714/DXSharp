struct Agent { float2 center; float2 velocity; };
struct Ship { float2 center; float2 heading; float4 tint; float4 motion; };
RWStructuredBuffer<Agent> Agents : register(u0);
RWStructuredBuffer<Ship> Ships : register(u1);
RWStructuredBuffer<float4> Bodies : register(u2);
cbuffer Scene : register(b0) {
    uint ShipCount; uint BodyCount; uint Reset; float Delta;
    float Time; float Radius; uint SectorIndex; uint MajorCount;
    float BeltInner; float BeltOuter; float SolarSpacing; float Reserved1;
    float4 MajorBodies[11];
};

uint Mix(uint x) {
    x ^= x >> 16;
    x *= 0x7feb352d;
    x ^= x >> 15;
    x *= 0x846ca68b;
    return x ^ (x >> 16);
}
float Unit(uint x) { return float(Mix(x) >> 8) * (1.0 / 16777216.0); }

[numthreads(64,1,1)]
void OrbitField(uint3 threadId : SV_DispatchThreadID) {
    uint id = threadId.x;
    if (id >= BodyCount) return;
    if (id < 11) { Bodies[id] = MajorBodies[min(id,MajorCount-1)]; return; }
    uint seed = id + SectorIndex*16381;
    float lane = lerp(BeltInner,BeltOuter,Unit(seed*7));
    float inner = BeltInner, outer = BeltOuter;
    float rate = 0.085*pow(BeltInner/lane,1.5);
    if (SolarSpacing > 0) {
        // Avoid the strongest Jupiter resonances near 2.50 and 2.82 AU.
        float sample = Unit(seed*7)*1.06;
        float au = 2.1 + sample + 0.07*step(0.37,sample) + 0.07*step(0.62,sample);
        lane = au;
        inner = 2.1;
        outer = 3.3;
        rate = 0.00071677/pow(au,1.5);
    }
    float mean = fmod(Unit(seed*13)*6.2831853 + Time*rate,6.2831853);
    float e = min(lerp(0.025,0.14,Unit(seed*47)),min(lane-inner,outer-lane)/lane);
    float eccentric = mean;
    [unroll] for (int i=0;i<3;++i) eccentric -= (eccentric-e*sin(eccentric)-mean)/(1-e*cos(eccentric));
    float2 orbit = lane*float2(cos(eccentric)-e,sqrt(1-e*e)*sin(eccentric));
    if (SolarSpacing > 0) {
        float radial = length(orbit);
        orbit *= SolarSpacing*(0.12+0.34*log2(1+3*radial))/radial;
    }
    float angle = Unit(seed*41)*6.2831853;
    float cs = cos(angle), sn = sin(angle);
    float2 center = MajorBodies[0].xy + float2(orbit.x*cs-orbit.y*sn,orbit.x*sn+orbit.y*cs)*Radius;
    Bodies[id] = float4(center, 0.05 + pow(Unit(seed*23),3)*0.26, Unit(seed*31));
}

[numthreads(64,1,1)]
void Steering(uint3 threadId : SV_DispatchThreadID) {
    uint id = threadId.x;
    if (id >= ShipCount) return;
    uint seed = id + SectorIndex*21611;
    uint destinations = MajorCount - 1 + (BodyCount - 11);
    uint bodyIndex = 1 + (Mix(seed + 41) % destinations);
    if (bodyIndex >= MajorCount) bodyIndex += 11 - MajorCount;
    float4 body = Bodies[bodyIndex];
    float phase = Unit(seed*5+7)*6.2831853 + Time*(0.28+Unit(seed*13+1)*0.21);
    float orbitRadius = body.z*1.68 + 2.8 + Unit(seed*17+9)*6.8;
    float2 target = body.xy + float2(cos(phase),sin(phase))*orbitRadius;
    Agent agent;
    if (Reset != 0) {
        agent.center = target + float2(Unit(seed*19+3)-0.5,Unit(seed*23+3)-0.5)*2.4;
        agent.velocity = float2(-sin(phase),cos(phase))*orbitRadius*0.32;
    } else agent = Agents[id];
    float2 desired = (target-agent.center)*2.3;
    desired *= min(1,9 / max(length(desired),0.01));
    agent.velocity = lerp(agent.velocity,desired,saturate(Delta*3.2));
    agent.center += agent.velocity*Delta;
    Agents[id] = agent;
    Ship ship;
    ship.center = agent.center;
    ship.heading = length(agent.velocity) > 0.001 ? normalize(agent.velocity) : float2(-sin(phase),cos(phase));
    ship.tint = float4(0.58,0.66,0.76,0.68);
    ship.motion = float4(0,Unit(seed),0.28,0);
    Ships[id] = ship;
}
RWStructuredBuffer<float4> TrafficTotals : register(u3);
groupshared float4 Partial[64];
[numthreads(64,1,1)]
void Summarize(uint id : SV_DispatchThreadID, uint lane : SV_GroupIndex, uint3 group : SV_GroupID) {
    float4 value = 0;
    if (id < ShipCount) {
        Agent agent = Agents[id];
        value = float4(agent.center, length(agent.velocity), 1);
    }
    Partial[lane] = value;
    GroupMemoryBarrierWithGroupSync();
    for (uint stride = 32; stride != 0; stride >>= 1) {
        if (lane < stride) Partial[lane] += Partial[lane + stride];
        GroupMemoryBarrierWithGroupSync();
    }
    if (lane == 0) TrafficTotals[group.x] = Partial[0];
}
