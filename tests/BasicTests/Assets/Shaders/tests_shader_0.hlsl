// Globals for texture sampling
Texture2D ShaderTexture : register(t0);
SamplerState Sampler : register(s0);

// Constant buffer to be updated by application per object
cbuffer PerObject : register(b0)
{
    // WorldViewProjection matrix
    float4x4 WorldViewProj;
};

// Vertex Shader input structure with position 
// and texture coordinate
struct VertexShaderInput
{
    float4 Position : SV_Position;
    float2 TextureUV : TEXCOORD0;
};

// Vertex Shader output structure consisting of the
// transformed position and texture coord
// This is also the pixel shader input
struct VertexShaderOutput
{
    float4 Position : SV_Position;
    float2 TextureUV : TEXCOORD0;
};

// Vertex shader main function
VertexShaderOutput VSMain(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    // Transform the position from object space to homogeneous 
    // projection space
    output.Position = mul(input.Position, WorldViewProj);
    // Pass through the texture coord of the vertex
    output.TextureUV = input.TextureUV;

    return output;
}

// A simple Pixel Shader that simply passes through the interpolated color
float4 PSMain(VertexShaderOutput input) : SV_Target
{
    // Sample the pixel color using the sampler and texture 
    // using the input texture coordinate
    return ShaderTexture.Sample(Sampler, input.TextureUV);
}