#ifndef CUSTOM_TILE_LIGHTING_INCLUDED
#define CUSTOM_TILE_LIGHTING_INCLUDED

uniform float _CustomLightCount;
uniform float4 _CustomLightData[64];  
uniform float4 _CustomLightColor[64]; 

void CalculateCustomLighting_float(
    float3 WorldPos,
    float3 WorldNormal,
    float YScale,
    float RadiusOffset,
    out float3 OutColor,
    out float RawAttenuation)
{
    float3 finalColor = float3(0, 0, 0);
    float totalRawAtten = 0;
    
    float FakeLightHeight = 2; 

    int count = (int)_CustomLightCount;
    if (count > 64) count = 64;

    for (int i = 0; i < count; i++)
    {
        float3 lightPos = _CustomLightData[i].xyz;
        lightPos.z = floor(lightPos.y * 4) * 0.01f + lightPos.z * 4;
        
        float radius = _CustomLightData[i].w + RadiusOffset;
        float3 color = _CustomLightColor[i].rgb;
        float intensity = _CustomLightColor[i].w;
        
        float3 lightPos3D = lightPos;
        lightPos3D.z += FakeLightHeight;

        float3 dir = lightPos3D - WorldPos;
        dir.y /= YScale;

        float dist = length(dir);

        if (dist < radius)
        {
            float3 lightDir = normalize(dir);
            float3 normal = WorldNormal;

            float diffuse = saturate(dot(normal, lightDir));
            float attenuation = 1.0 - smoothstep(0, radius, dist);

            float currentLight = attenuation * intensity * diffuse;

            finalColor += color * currentLight;
            totalRawAtten = max(totalRawAtten, currentLight);
        }
    }

    OutColor = finalColor;
    RawAttenuation = totalRawAtten;
}

#endif