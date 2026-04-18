#ifndef CLEAN_WORLD_DITHER_INCLUDED
#define CLEAN_WORLD_DITHER_INCLUDED

void CleanWorldDither_float(float InLightValue, float2 WorldPixelCoords, float DitherStrength, out float OutResult)
{
    float4x4 bayer = float4x4(
        0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
        12.0/16.0, 4.0/16.0, 14.0/16.0, 6.0/16.0,
        3.0/16.0, 11.0/16.0, 1.0/16.0,  9.0/16.0,
        15.0/16.0, 7.0/16.0, 13.0/16.0, 5.0/16.0
    );
    uint2 pixelIndex = uint2(floor(WorldPixelCoords + 10000.0)) % 4;

    float threshold = bayer[pixelIndex.x][pixelIndex.y];
    OutResult = step(threshold * DitherStrength, InLightValue);
}

#endif