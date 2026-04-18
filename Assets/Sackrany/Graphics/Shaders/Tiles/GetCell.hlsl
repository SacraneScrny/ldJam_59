void GetDirectionCell_float(float2 Direction, out float2 Cell)
{
    float angle = atan2(Direction.y, Direction.x);

    float sector = 6.2831853 / 8.0;

    float snapped = round(angle / sector) * sector;

    float2 dir = float2(cos(snapped), sin(snapped));

    dir = sign(dir);

    Cell = dir + 1.0;
}