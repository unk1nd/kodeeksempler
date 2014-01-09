//VS utdata:
struct Vertex2Pixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
    float LightingFactor: TEXCOORD0;
};

//PS utdata:
struct Pixel2Frame
{
    float4 Color : COLOR0;
};

//------- Shader-parametre --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float4x4 xInvTransWorld;

float3 xLightDirection;
float xAmbient;
bool xEnableLighting;

//------- Technique: Belysning  --------
Vertex2Pixel LysVS( float4 inPos : POSITION, float4 inColor: COLOR, float3 inNormal: NORMAL)
{	
	Vertex2Pixel Output = (Vertex2Pixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;
	
	//Normaliserer normalvektoren:
	float3 normal = normalize(inNormal);
		
	//Caster til 3x3 matrise for å fjerne evt. translasjon i matrisa. 
	//Hvis vi ikke fjerner translasjonsdelen av matrisa vil normalvektorene bli feil etter transformasjonen.
	//float3x3 rotMatrix = (float3x3)xWorld;
	
	//Transformerer normalvektorene:
	//float3 rotNormal = mul(normal, rotMatrix);
	//float3 rotNormal = mul(normal, xWorld);
	
	//Bruker invers-transpose:
	float3 rotNormal = mul(normal, xInvTransWorld);
	
	//float3 rotNormal = normal; 
	
	rotNormal = normalize(rotNormal);
	
	Output.LightingFactor = 1;
	
	if (xEnableLighting) {
		Output.LightingFactor = max(dot(rotNormal, -xLightDirection), 0);
    } 
	return Output;    
}

Pixel2Frame LysPS(Vertex2Pixel PSIn) 
{
	Pixel2Frame Output = (Pixel2Frame)0;		
    
	Output.Color = PSIn.Color*clamp(PSIn.LightingFactor + xAmbient,0,1);
	
	return Output;
}

technique Belysning
{
	pass P0
    {   
    	VertexShader = compile vs_2_0 LysVS();
        PixelShader  = compile ps_2_0 LysPS();
    }
}