#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED
void VertexTransform_float(float3 Position, float3 Normal, float3 Scale, float RotationZ, out float3 OutPosition, out float3 OutNormal)
{
    float3 pos = Position * Scale;

    float c = cos(RotationZ);
    float s = sin(RotationZ);
    float x = pos.x * c - pos.y * s;
    float y = pos.x * s + pos.y * c;
    pos.x = x;
    pos.y = y;

    OutPosition = pos;

    float nx = Normal.x * c - Normal.y * s;
    float ny = Normal.x * s + Normal.y * c;
    OutNormal = normalize(float3(nx, ny, Normal.z));
}
#endif