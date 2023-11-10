// A simple vertex and pixel shader:

struct VS_INPUT {
    float4 Pos   : POSITION ;
    float4 Color : COLOR ;
} ;

struct VS_OUTPUT {
    float4 Pos   : SV_POSITION ;
    float4 Color : COLOR ;
} ;


VS_OUTPUT VSMain( VS_INPUT input ) {
    VS_OUTPUT output = (VS_OUTPUT)0 ;
    output.Color = input.Color ;
    output.Pos = input.Pos ;
    return output ;
}

float4 PSMain( VS_OUTPUT input ) : SV_TARGET {
    return input.Color ;
}