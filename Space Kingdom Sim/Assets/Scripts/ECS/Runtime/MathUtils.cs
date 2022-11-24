using Unity.Mathematics;

public static class MathUtils
{
    /// <summary>
    /// if target is on the right returns 1, else -1.
    /// </summary>
    public static float HorizontalDirectionToTarget(float3 right, float3 directionToTarget)
    {
        var dot = math.dot(right, directionToTarget);

        return dot >= 0 ? 1f : -1f;
    }

    public static float3 SetMagnitude(float3 vector, float length)
    {
        return math.normalizesafe(vector) * length;
    }

    public static float3 ClampMagnitude(float3 vector, float maxLength)
    {
        if (math.lengthsq(vector) > maxLength * maxLength)
            return SetMagnitude(vector, maxLength);

        return vector;
    }
}