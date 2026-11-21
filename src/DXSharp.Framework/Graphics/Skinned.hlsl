struct Joint { row_major float4x4 Pose; row_major float4x4 Normal; };
struct SkinVertex {
    float3 position : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv : TEXCOORD0;
    float4 weights : BLENDWEIGHT;
    uint4 joints : BLENDINDICES;
};
struct Deformation { float3 position; float3 normal; float3 tangent; };
Deformation Skin(SkinVertex v, Joint a, Joint b, Joint c, Joint d) {
    Deformation o;
    o.position = mul(float4(v.position,1),a.Pose).xyz*v.weights.x + mul(float4(v.position,1),b.Pose).xyz*v.weights.y
               + mul(float4(v.position,1),c.Pose).xyz*v.weights.z + mul(float4(v.position,1),d.Pose).xyz*v.weights.w;
    o.normal = normalize(mul(v.normal,(float3x3)a.Normal)*v.weights.x + mul(v.normal,(float3x3)b.Normal)*v.weights.y
               + mul(v.normal,(float3x3)c.Normal)*v.weights.z + mul(v.normal,(float3x3)d.Normal)*v.weights.w);
    o.tangent = mul(v.tangent.xyz,(float3x3)a.Pose)*v.weights.x + mul(v.tangent.xyz,(float3x3)b.Pose)*v.weights.y
               + mul(v.tangent.xyz,(float3x3)c.Pose)*v.weights.z + mul(v.tangent.xyz,(float3x3)d.Pose)*v.weights.w;
    return o;
}
#ifdef VERIFY_SKIN
RWStructuredBuffer<SkinVertex> Input : register(u0);
RWStructuredBuffer<Joint> Palette : register(u1);
RWStructuredBuffer<float4> Output : register(u2);
cbuffer Count : register(b0) { uint VertexCount; };
[numthreads(64,1,1)]
void CS(uint3 id : SV_DispatchThreadID) {
    if(id.x >= VertexCount) return;
    SkinVertex v=Input[id.x];
    Deformation d=Skin(v,Palette[v.joints.x],Palette[v.joints.y],Palette[v.joints.z],Palette[v.joints.w]);
    Output[id.x*2]=float4(d.position,1); Output[id.x*2+1]=float4(d.normal,0);
}
#else
StructuredBuffer<Joint> Palette : register(t0,space1);
cbuffer Scene : register(b0) {
    row_major float4x4 WorldViewProjection;
    row_major float4x4 World;
    row_major float4x4 NormalToWorld;
    float4 Surface;
    float4 Camera;
};
Texture2D<float4> BaseColor : register(t0);
Texture2D<float4> NormalMap : register(t1);
Texture2D<float4> MetallicRoughness : register(t2);
Texture2D<float4> Occlusion : register(t3);
SamplerState MaterialSampler : register(s0);
struct Fragment {
    float4 position : SV_Position;
    float3 world : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float4 tangent : TEXCOORD2;
    float2 uv : TEXCOORD3;
};
Fragment VS(SkinVertex v) {
    Deformation d=Skin(v,Palette[v.joints.x],Palette[v.joints.y],Palette[v.joints.z],Palette[v.joints.w]);
    Fragment o;
    o.position=mul(float4(d.position,1),WorldViewProjection); o.world=mul(float4(d.position,1),World).xyz;
    o.normal=normalize(mul(d.normal,(float3x3)NormalToWorld));
    o.tangent=float4(mul(d.tangent,(float3x3)World),v.tangent.w); o.uv=v.uv;return o;
}
float4 PS(Fragment i) : SV_Target {
    float3 N=normalize(i.normal);
    float3 T=normalize(i.tangent.xyz-N*dot(i.tangent.xyz,N));
    float3 B=cross(N,T)*i.tangent.w;
    float3 mapped=NormalMap.Sample(MaterialSampler,i.uv).xyz*2-1;
    float3 albedo=BaseColor.Sample(MaterialSampler,i.uv).rgb;
    float3 orm=MetallicRoughness.Sample(MaterialSampler,i.uv).rgb;
    float ao=Occlusion.Sample(MaterialSampler,i.uv).r;
    if(Surface.w<0.5) N=normalize(T*mapped.x+B*mapped.y+N*mapped.z);
    else {
        float2 g=abs(frac(i.world.xz)-0.5)/max(fwidth(i.world.xz),0.0001);
        float line=1-saturate(min(g.x,g.y));
        albedo=lerp(float3(0.045,0.065,0.085),float3(0.09,0.17,0.20),line*0.7);
        orm=float3(1,1,0);ao=1;
    }
    float metallic=saturate(orm.b*Surface.x), roughness=clamp(orm.g*Surface.y,0.06,1);
    float3 V=normalize(Camera.xyz-i.world), L=normalize(float3(-0.48,0.82,0.31)),H=normalize(V+L);
    float nl=saturate(dot(N,L)), nv=max(saturate(dot(N,V)),0.001), nh=saturate(dot(N,H)),vh=saturate(dot(V,H));
    float a=roughness*roughness, a2=a*a, den=nh*nh*(a2-1)+1;
    float D=a2/max(3.14159265*den*den,0.0001), k=(roughness+1)*(roughness+1)/8;
    float G=(nv/(nv*(1-k)+k))*(nl/max(nl*(1-k)+k,0.001));
    float3 F=lerp(0.04.xxx,albedo,metallic)+(1-lerp(0.04.xxx,albedo,metallic))*pow(1-vh,5);
    float3 color=((1-F)*(1-metallic)*albedo/3.14159265 + D*G*F/max(4*nv*nl,0.001))*nl*float3(3.2,3.05,2.8);
    color+=albedo*ao*float3(0.16,0.18,0.22);
    color*=Camera.w;
    color=saturate(color*(2.51*color+0.03)/(color*(2.43*color+0.59)+0.14));
    return float4(pow(color,1/2.2),1);
}
#endif
