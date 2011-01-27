float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

float4 xWaveSpeeds;
float4 xWaveHeight;
float4 xWaveLenght;
float4 xWaveDir0;
float4 xWaveDir1;
float4 xWaveDir2;
float4 xWaveDir3;

float3 xCameraPos;
float xBumpStrenght;
float xTexStretch;
float xTime;

Texture xCubeMap;
samplerCUBE CubeMapSampler = sampler_state
{ 
	texture = <xCubeMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

struct OWVertexToPixel
{
	float4 Poisition : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Pos3D: TEXCOORD;
	float3x3 TTW : TEXCOORD2;
};


struct oWPixelToFrame
{
	float4 Color : COLOR0;
};

OWVertexToPixel OWVertexShader(float4 inPos: POISTION0, float2 inTexCoord: TEXCOORD0)
{
	OWVertexToPixel Output = (OWVertexToPixel)0;

	float4 dotProducts;
	dotProducts.x = dot(xWaveDir0, inPos.xz);
	dotProducts.y = dot(xWaveDir1, inPos.xz);
	dotProducts.z = dot(xWaveDir2, inPos.xz);
	dotProducts.w = dot(xWaveDir3, inPos.xz);

	float4 arguments = dotProducts / xWaveLenght+xTime*xWaveSpeeds;
	float4 height = xWaveHeight* sin(arguments);

	float4 final3DPos = inPos;

	final3DPos.y +=height.x;
	final3DPos.y +=height.y;
	final3DPos.y +=height.z;
	final3DPos.y +=height.w;

	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);
	//Output.Position = mul(final3Dpos, preWorldViewProjection);

	float4 final3dPosW = mul(final3DPos, xWorld);
	Output.Pos3D = final3dPosW;


};
/*
OWPixelToFrame OWPixelShader(OWVetexToPixel PSIn) : COLOR0
{
	OWPixelToFrame Output = (OWPixelToFrame)0;

	float4 waterColor = float4(0, 0.1, 0.3, 1);
	Output.Color = waterColor;

	return Output;
}*/

technique OceanWater
{
	pass Pass0
	{
		//VertexShader = compile vs_2_0 OWVertexShader();
		//PixelShader = compile  ps_2_0 OWPixelShader();
	}
}