using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct FollowMouse : IComponentData
{

}

public struct InBounds : IComponentData, IEnableableComponent
{

}

public struct SteeringAgent : IComponentData
{
    public float attractionForce;
    public float additionalAttraction;
    public float predictionAmount;

    public float slowRadius;
    public float maxForce;
}

public readonly partial struct SteeringAgentAspect : IAspect
{
    readonly PhysicsBodyAspect physicsBodyAspect;
    readonly RefRO<SteeringAgent> steeringAgent;
    readonly RefRO<LocalToWorld> ltw;

    public float3 Position => ltw.ValueRO.Position;

    public float AttractionForce => steeringAgent.ValueRO.attractionForce;
    public float AdditionalAttraction => steeringAgent.ValueRO.additionalAttraction;

    public float MaxForce => steeringAgent.ValueRO.maxForce;

    public float SlowRadius => steeringAgent.ValueRO.slowRadius;

    public void Steer(float3 targetPosition, float attractionForce)
    {
        var targetOffset = targetPosition - Position;

        var slowRaius = SlowRadius;

        var targetIsBehind = math.dot(ltw.ValueRO.Forward, targetOffset) < 0;

        if (targetIsBehind)
        {
            targetOffset = MathUtils.DirectionToTarget(ltw.ValueRO.Right, targetOffset);
            slowRaius = 0;
        }

        var distanceToTarget = math.length(targetOffset);

        var desiredSpeed = distanceToTarget > slowRaius
        ? physicsBodyAspect.MaxSpeed
        : math.remap(0, slowRaius, 0, physicsBodyAspect.MaxSpeed, distanceToTarget);

        var desiredVelocity = MathUtils.SetMagnitude(targetOffset, desiredSpeed);

        var steeringForce = desiredVelocity - physicsBodyAspect.Velocity;

        steeringForce = MathUtils.ClampMagnitude(steeringForce, MaxForce);

        physicsBodyAspect.ResultantForce += steeringForce * attractionForce;
    }
}