using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public struct TargetInRange : IComponentData, IEnableableComponent
{

}

public struct TargetData : IComponentData
{
    public Entity target;
    public float distanceToTarget;
    public float targetType;
    public bool isTargetExist;
}

public struct MovementTarget : IComponentData
{
    public float3 targetPosition;

    public float distanceToTarget;

    public bool isTargetPositionValid;
}

public struct TargetSeeker : IComponentData
{
    public float searchRadius;
    public float stopRange;

    public CollisionFilter targetLayer;
}

public struct UnitType : IComponentData
{
    public float value;
}

public readonly partial struct TargetDataAspect : IAspect
{
    readonly RefRW<TargetData> targetData;
    readonly RefRW<MovementTarget> movementTarget;
    readonly RefRO<TargetSeeker> targetSeeker;
    readonly RefRO<SteeringAgent> steeringAgent;
    readonly RefRO<Translation> translation;

    readonly EnabledRefRW<TargetInRange> enabledRefRW;

    private float PredictionAmount => steeringAgent.ValueRO.predictionAmount;

    public float3 Position => translation.ValueRO.Value;

    public Entity Target => targetData.ValueRO.target;

    public bool TargetInRange
    {
        get => enabledRefRW.ValueRO;
        set => enabledRefRW.ValueRW = value;
    }

    public bool IsTargetExist
    {
        get => targetData.ValueRO.isTargetExist;
        set => targetData.ValueRW.isTargetExist = value;
    }

    public ref bool IsTargetPositionValid => ref movementTarget.ValueRW.isTargetPositionValid;

    public void Update(float3 targetPos)
    {
        movementTarget.ValueRW.targetPosition = targetPos;
        movementTarget.ValueRW.distanceToTarget = math.distance(translation.ValueRO.Value, targetPos);

        var targetInRange = movementTarget.ValueRO.distanceToTarget <= targetSeeker.ValueRO.stopRange;

        TargetInRange = targetInRange;
    }

    public void Update(float3 targetPos, float3 targetDirection)
    {
        targetPos += targetDirection * PredictionAmount;

        Update(targetPos);
    }
}