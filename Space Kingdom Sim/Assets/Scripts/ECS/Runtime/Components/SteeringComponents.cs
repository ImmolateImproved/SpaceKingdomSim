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

    public float stopRange;
    public float slowRadius;
    public float maxForce;
}

public readonly partial struct SteeringAgentAspect : IAspect
{
    readonly PhysicsBodyAspect physicsBodyAspect;
    readonly RefRO<SteeringAgent> steeringAgent;
    readonly RefRO<LocalTransform> transform;

    public float3 Position => transform.ValueRO.Position;
    public float3 Forward => transform.ValueRO.Forward();
    public float3 Right => transform.ValueRO.Right();

    public float AttractionForce => steeringAgent.ValueRO.attractionForce;
    public float AdditionalAttraction => steeringAgent.ValueRO.additionalAttraction;

    public float MaxForce => steeringAgent.ValueRO.maxForce;

    public float SlowRadius => steeringAgent.ValueRO.slowRadius;

    public void Steer(float3 targetPosition, float attractionForce)
    {
        var desiredDirection = GetDesiredDirection(targetPosition, out var targetIsBehind);

        var slowRaius = targetIsBehind ? 0 : SlowRadius;

        var distanceToTarget = math.length(desiredDirection);
        var desiredSpeed = GetDesiredSpeed(slowRaius, distanceToTarget);
        
        var desiredVelocity = MathUtils.SetMagnitude(desiredDirection, desiredSpeed);

        var steeringForce = desiredVelocity - physicsBodyAspect.Velocity;

        steeringForce = MathUtils.ClampMagnitude(steeringForce, MaxForce);

        physicsBodyAspect.ResultantForce += steeringForce * attractionForce;
    }

    private float3 GetDesiredDirection(float3 targetPosition, out bool targetIsBehind)
    {
        var desiredDirection = targetPosition - Position;

        targetIsBehind = math.dot(Forward, desiredDirection) < 0;

        if (targetIsBehind)
        {
            var right = Right;

            var horizontalDirection = MathUtils.HorizontalDirectionToTarget(right, desiredDirection);
            desiredDirection = right * horizontalDirection;
        }

        return desiredDirection;
    }

    private float GetDesiredSpeed(float slowRaius, float distanceToTarget)
    {
        return distanceToTarget > slowRaius
            ? physicsBodyAspect.MaxSpeed
            : math.remap(0, slowRaius, 0, physicsBodyAspect.MaxSpeed, distanceToTarget);
    }
}