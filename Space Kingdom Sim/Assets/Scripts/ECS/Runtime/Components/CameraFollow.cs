using Unity.Entities;
using Unity.Mathematics;

public struct CameraFollow : IComponentData
{
    public bool enabled;

    public Entity currentTarget;
    public float3 offset;

    public Range fromRange;
    public Range moveSpeedRange;
    public Range dragSpeedRange;
    public Range scrollSpeedRange;

    public float GetMoveSpeed(float x)
    {
        return math.remap(fromRange.min, fromRange.max, moveSpeedRange.min, moveSpeedRange.max, x);
    }

    public float GetDragSpeed(float x)
    {
        return math.remap(fromRange.min, fromRange.max, dragSpeedRange.min, dragSpeedRange.max, x);
    }

    public float GetScrollSpeed(float x)
    {
        return math.remap(fromRange.min, fromRange.max, scrollSpeedRange.min, scrollSpeedRange.max, x);
    }
}
