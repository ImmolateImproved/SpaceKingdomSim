using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct PhysicsData : IComponentData
{
    public float3 resultantForce;
    public float3 velocity;
}

public struct MaxSpeed : IComponentData
{
    public float value;
}

public readonly partial struct PhysicsBodyAspect : IAspect
{
    readonly RefRW<PhysicsData> physicsData;
    readonly RefRW<MaxSpeed> maxSpeed;

    public float3 ResultantForce
    {
        get => physicsData.ValueRO.resultantForce;
        set => physicsData.ValueRW.resultantForce = value;
    }

    public float3 Velocity
    {
        get => physicsData.ValueRO.velocity;
        set => physicsData.ValueRW.velocity = value;
    }

    public float MaxSpeed
    {
        get => maxSpeed.ValueRO.value;
        set => maxSpeed.ValueRW.value = value;
    }

    public void CalculateVelocity()
    {
        var acceleration = ResultantForce;

        Velocity += acceleration;
        Velocity = MathUtils.ClampMagnitude(Velocity, MaxSpeed);

        ResultantForce = 0;
    }
}