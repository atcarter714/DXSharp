struct Agent { float2 center; float2 velocity; };
struct Ship { float2 center; float2 heading; float4 tint; float4 motion; };
RWStructuredBuffer<Agent> Agents : register(u0);
RWStructuredBuffer<Ship> Ships : register(u1);
RWStructuredBuffer<float4> Bodies : register(u2);
cbuffer Scene : register(b0) {
    uint ShipCount; uint BodyCount; uint Reset; float Delta;
    float Time; float Radius; uint SectorIndex; uint MajorCount;
    float BeltInner; float BeltOuter; float Reserved0; float Reserved1;
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
    float a = Unit(seed*13) * 6.2831853 + Time*(0.085*pow(BeltInner/lane,1.5));
    float2 center = MajorBodies[0].xy + float2(cos(a),sin(a)) * (Radius*lane);
    Bodies[id] = float4(center, 0.09 + pow(Unit(seed*23),3)*0.52, Unit(seed*31));
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
