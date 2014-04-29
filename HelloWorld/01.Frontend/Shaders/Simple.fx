struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

float4x4 gWorld; 
float4x4 gView; 
float4x4 gProj; 

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	float4x4 worldViewProj = mul(mul(gWorld, gView), gProj);
	output.pos = mul(input.pos, worldViewProj);
	output.col = input.col;
	
	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	return input.col;
}

BlendState SrcAlphaBlendingAdd 
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

technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
		SetBlendState( SrcAlphaBlendingAdd, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}