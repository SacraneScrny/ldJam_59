#ifndef PIXEL_ART_UTILS_INCLUDED
#define PIXEL_ART_UTILS_INCLUDED

void QuantizeValue_float(float In, float Steps, out float Out)
{
    Out = floor(In * Steps + 0.5) / Steps;
}

void StableDither_float(float InVal, float2 WorldPos, float PixelsPerUnit, float DitherStrength, out float Out)
{
    float4x4 bayer = float4x4(
        0.0, 8.0, 2.0, 10.0,
        12.0, 4.0, 14.0, 6.0,
        3.0, 11.0, 1.0, 9.0,
        15.0, 7.0, 13.0, 5.0
    ) / 16.0;
    
    int2 pixelGrid = int2(floor(WorldPos * PixelsPerUnit + 10000.0));
    
    int x = pixelGrid.x % 4;
    int y = pixelGrid.y % 4;
    
    float threshold = bayer[y][x];
    
    Out = step(threshold * DitherStrength, InVal);
}

#endif