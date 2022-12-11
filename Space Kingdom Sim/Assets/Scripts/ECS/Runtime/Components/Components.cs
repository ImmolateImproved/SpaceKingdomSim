using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Timer
{
    public float current;
    public float max;

    public bool IsCompleted => current >= max;

    public Timer(float max, bool currentEqualsMax) : this()
    {
        this.max = max;
        current = currentEqualsMax ? max : 0;
    }

    public void Tick(float dt)
    {
        current += dt;
    }

    public void Reset()
    {
        current = 0;
    }
}

public struct MousePosition : IComponentData
{
    public float3 value;
}

public struct OutOfBoundSteering : IComponentData
{
    public Bounds squareBounds;
    public float steeringForce;
}

public struct Energy : IComponentData
{
    public float max;
    public float current;
    public float decreasePerSeconds;
}

[System.Serializable]
public struct Range
{
    public float min;
    public float max;
}

public struct InactiveState : IComponentData
{
    public float duration;
    public float timer;
}