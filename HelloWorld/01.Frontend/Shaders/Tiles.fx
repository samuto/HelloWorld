// ---------------------------------
//        global variables
// ---------------------------------
float4x4 gWorld; 
float4x4 gView; 
float4x4 gProj; 
Texture2DArray<float4> textureArray;

float3 lightDirection = float3(0,-1,0);
// ---------------------------------
//   input strcutures for shaders
// ---------------------------------
struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float3 normal : NORMAL;
	float2 tex : TEXCOORD;
	float index : PSIZE;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD0;
	float index : PSIZE;
};

// ---------------------
//   Define states...
// ---------------------
SamplerState samplerState
{
	Filter = MIN_MAG_MIP_POINT;

	AddressU = Clamp;
	AddressV = Clamp;
};

BlendState NoBlend
{
  AlphaToCoverageEnable = FALSE;
  BlendEnable[0] = FALSE;
};

BlendState Blending1 
{     
	BlendEnable[0] = TRUE;
	SrcBlend = SRC_ALPHA;
	DestBlend = INV_SRC_ALPHA;
	BlendOp = ADD;
	SrcBlendAlpha = ZERO;
	DestBlendAlpha = ZERO;
	BlendOpAlpha = ADD;
	RenderTargetWriteMask[0] = 0x0F;
}; 

BlendState Blending2
{     
	BlendEnable[0] = TRUE;
	SrcBlend = DEST_COLOR;
	DestBlend = ZERO;
	BlendOp = ADD;
	SrcBlendAlpha = ZERO;
	DestBlendAlpha = ZERO;
	BlendOpAlpha = ADD;
	RenderTargetWriteMask[0] = 0x0F;
}; 

// ------------------------------------
//         shader functions...
// ------------------------------------
PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;

	float4x4 worldViewProj = mul(mul(gWorld, gView), gProj);
	output.pos = mul(input.pos, worldViewProj);
	output.tex = input.tex;
	output.index = input.index;
	output.col = input.col;

	
	float3 L = normalize(-lightDirection);
    float diffuseLight = max(dot(input.normal, L), 0.5);
	float alpha = input.col.w;
    output.col = input.col * diffuseLight;
	output.col.w = alpha;
	
	return output;
}

PS_IN VS_SIMPLE( VS_IN input )
{
	PS_IN output = (PS_IN)0;

	float4x4 worldViewProj = mul(mul(gWorld, gView), gProj);
	output.pos = mul(input.pos, worldViewProj);
	output.tex = input.tex;
	output.index = input.index;
	output.col = input.col;

	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	return input.col * textureArray.Sample(samplerState, float3(input.tex, input.index));
}

float4 PS_2( PS_IN input ) : SV_Target
{
	float4 color = input.col * textureArray.Sample(samplerState, float3(input.tex, input.index));
	clip( color.a - 0.5 );     
	return color;
}

float4 PS_SIMPLE( PS_IN input ) : SV_Target
{
	float4 white = float4(1,1,1,1);
	float4 color = textureArray.Sample(samplerState, float3(input.tex, input.index));
	return lerp(white, color, input.col.w);

}

technique10 tech0
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS() ) );
		SetBlendState( NoBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
    }
}

technique10 tech1
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS_SIMPLE() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS_SIMPLE() ) );
		SetBlendState( Blending2, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
    }
}


technique10 tech2
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS_2() ) );
		SetBlendState( Blending1, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
    }
}


technique10 tech3
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS_SIMPLE() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS() ) );
		SetBlendState( Blending1, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
    }
}

technique10 tech4
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS_SIMPLE() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS() ) );
		SetBlendState( NoBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
    }
}