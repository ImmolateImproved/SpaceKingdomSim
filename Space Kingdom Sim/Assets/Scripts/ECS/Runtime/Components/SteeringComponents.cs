using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
    public quaternion Rotation => ltw.ValueRO.Rotation;

    public float AttractionForce => steeringAgent.ValueRO.attractionForce;
    public float AdditionalAttraction => steeringAgent.ValueRO.additionalAttraction;

    public float MaxForce => steeringAgent.ValueRO.maxForce;

    public float SlowRadius => steeringAgent.ValueRO.slowRadius;

    public void Steer(float3 targetPosition, float attractionForce)
    {
        var desiredVelocity = targetPosition - Position;

        var slowRaius = SlowRadius;

        var dot = math.dot(ltw.ValueRO.Forward, desiredVelocity);
        if (dot < 0)
        {
            var targetDir = math.dot(ltw.ValueRO.Right, desiredVelocity);
            var rotationDir = (targetDir >= 0f) ? 1f : (-1f);
            desiredVelocity = math.mul(Rotation, new float3(rotationDir, 0, 0));
            slowRaius = 0;
        }

        var distanceToTarget = math.length(desiredVelocity);

        var desiredSpeed = distanceToTarget > slowRaius
            ? physicsBodyAspect.MaxSpeed
            : math.remap(0, slowRaius, 0, physicsBodyAspect.MaxSpeed, distanceToTarget);

        var steeringForce = MathUtils.SetMagnitude(desiredVelocity, desiredSpeed);

        steeringForce -= physicsBodyAspect.Velocity;

        steeringForce = MathUtils.ClampMagnitude(steeringForce, MaxForce);

        physicsBodyAspect.ResultantForce += steeringForce * attractionForce;
    }
}