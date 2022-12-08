using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
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
    public float maxSpeed;
}

public struct Direction : IComponentData
{
    public float3 directionToTarget;
}

public readonly partial struct SteeringAgentAspect : IAspect
{
    readonly RefRW<PhysicsVelocity> velocity;
    readonly RefRW<Direction> direction;

    readonly RefRO<SteeringAgent> steeringAgent;
    readonly RefRO<LocalTransform> transform;

    public float3 Position => transform.ValueRO.Position;
    public float3 Forward => transform.ValueRO.Forward();
    public float3 Right => transform.ValueRO.Right();

    public float AttractionForce => steeringAgent.ValueRO.attractionForce;
    public float AdditionalAttraction => steeringAgent.ValueRO.additionalAttraction;

    public float MaxSpeed => steeringAgent.ValueRO.maxSpeed;

    public float MaxForce => steeringAgent.ValueRO.maxForce;

    public void Steer(float3 targetPosition, float attractionForce)
    {
        var directionToTarget = targetPosition - Position;
        var desiredDirection = GetDesiredDirection(directionToTarget, out var targetIsBehind);

        var slowRaius = steeringAgent.ValueRO.slowRadius; //targetIsBehind ? 0 : steeringAgent.ValueRO.slowRadius;

        var distanceToTarget = math.length(directionToTarget);
        var desiredSpeed = GetDesiredSpeed(slowRaius, distanceToTarget);

        var desiredVelocity = MathUtils.SetMagnitude(desiredDirection, desiredSpeed);

        var steeringForce = desiredVelocity - velocity.ValueRO.Linear;

        steeringForce = MathUtils.ClampMagnitude(steeringForce, MaxForce);

        //if (distanceToTarget < steeringAgent.ValueRO.stopRange)
        //{
        //    attractionForce = -1;
        //}
        //else
        //{
        //    attractionForce = 1;
        //}

        var acceleration = steeringForce * attractionForce;
        velocity.ValueRW.Linear += acceleration;
    }

    private float3 GetDesiredDirection(float3 directionToTarget, out bool targetIsBehind)
    {
        var desiredDirection = directionToTarget;

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
           ? MaxSpeed
           : math.remap(0, slowRaius, 0, MaxSpeed, distanceToTarget);
    }
}