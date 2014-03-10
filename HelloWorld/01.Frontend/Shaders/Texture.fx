float3 eye;

float fogNear;
float fogFar;
float4 fogColor;

float4x4 gWorld; 
float4x4 gView; 
float4x4 gProj; 

Texture2D intexture;

SamplerState triLinearSample
{
	Filter = MIN_MAG_MIP_LINEAR;

	AddressU = Wrap;
	AddressV = Wrap;
};


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


BlendState NoBlend
{
  AlphaToCoverageEnable = FALSE;
  BlendEnable[0] = FALSE;
};

struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float2 uv : TEXCOORD;
};

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 uv : TEXCOORD0;
	float distance : TEXCOORD1;
};
 
VS_OUT VS(VS_IN vIn)
{
	VS_OUT vOut;
	
	float4x4 worldViewProj = mul(mul(gWorld, gView), gProj);
	vOut.pos = mul(vIn.pos, worldViewProj);
	vOut.col = vIn.col;
	vOut.uv  = vIn.uv;
	vOut.distance = length(eye - vIn.pos);
	return vOut;
}


float threshold = 0.1;

float4 PS_Texture_Fog(VS_OUT pIn) : SV_Target
{
	float4 textureColor;
	textureColor = pIn.col * intexture.Sample(triLinearSample, pIn.uv);
	float fog = saturate((pIn.distance - fogNear) / (fogNear-fogFar));    
	float4 color = lerp(fogColor, textureColor, fog);
	return color;

}

float4 PS_Texture(VS_OUT pIn) : SV_Target
{
	float4 textureColor;
	textureColor = pIn.col * intexture.Sample(triLinearSample, pIn.uv);
	return textureColor;
}



technique10 TechTexture
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS_Texture_Fog() ) );
		SetBlendState( NoBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
    }
}

technique10 TechTextureNoFog
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS_Texture() ) );
		SetBlendState( NoBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
    }
}

technique10 TechTextureNoFogAlphaBlend 
{     
	pass P0     
	{         
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_0, PS_Texture() ) );
		SetBlendState( SrcAlphaBlendingAdd, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	} 
} 

