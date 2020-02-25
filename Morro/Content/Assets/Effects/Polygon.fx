#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VSInputTransform
{
	float4 Position : POSITION0;
	float4x4 Transform: POSITION1;
};

struct VSInputTransformColor
{
	float4 Position : POSITION0;
	float4x4 Transform: POSITION1;
	float4 Color : COLOR5;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

VSOutput VSTransform(in VSInputTransform input)
{
	VSOutput output = (VSOutput)0;

	output.Position = mul(mul(input.Position, transpose(input.Transform)), WorldViewProjection);
	output.Color = float4(1, 1, 1, 1);

	return output;
}

VSOutput VSTransformColor(in VSInputTransformColor input)
{
	VSOutput output = (VSOutput)0;

	output.Position = mul(mul(input.Position, transpose(input.Transform)), WorldViewProjection);
	output.Color = input.Color;

	return output;
}

float4 MainPS(VSOutput input) : COLOR
{
	return input.Color;
}

technique PolygonVertexTransform
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSTransform();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

technique PolygonVertexTransformColor
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSTransformColor();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};