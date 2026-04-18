#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

void VertexTransform_float(
    float3 Position,
    float3 Normal,
    float3 Scale,
    float3 Rotation, // xyz rotation in radians
    out float3 OutPosition,
    out float3 OutNormal)
{
    // scale
    float3 pos = Position * Scale;

    // rotation around X
    float cx = cos(Rotation.x);
    float sx = sin(Rotation.x);
    float3x3 rotX = float3x3(
        1, 0, 0,
        0, cx, -sx,
        0, sx, cx
    );

    // rotation around Y
    float cy = cos(Rotation.y);
    float sy = sin(Rotation.y);
    float3x3 rotY = float3x3(
        cy, 0, sy,
        0, 1, 0,
        -sy, 0, cy
    );

    // rotation around Z
    float cz = cos(Rotation.z);
    float sz = sin(Rotation.z);
    float3x3 rotZ = float3x3(
        cz, -sz, 0,
        sz, cz, 0,
        0, 0, 1
    );

    // combined rotation (XYZ order)
    float3x3 rot = mul(rotZ, mul(rotY, rotX));

    // apply rotation
    pos = mul(rot, pos);
    OutPosition = pos;

    // transform normal (without scale)
    float3 nrm = mul(rot, Normal);
    OutNormal = normalize(nrm);
}

#endif
