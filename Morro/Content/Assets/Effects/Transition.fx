#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float3 Color;
float Cutoff;
float Smoothing;

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float value = tex2D(SpriteTextureSampler, input.TextureCoordinates).r;
	float alpha = smoothstep(Cutoff, Cutoff + Smoothing, lerp(value, 1, Smoothing));

	return float4(Color.rgb, alpha);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};