using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

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