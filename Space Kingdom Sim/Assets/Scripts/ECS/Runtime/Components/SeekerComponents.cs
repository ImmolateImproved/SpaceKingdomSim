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
    public int targetType;
    public bool isTargetExist;
}

public struct MovementDestination : IComponentData
{
    public float3 targetPosition;

    public float distanceToTarget;

    public bool isTargetPositionValid;
}

public struct PerceptionData : IComponentData
{
    public float searchRadius;

    public CollisionFilter targetLayer;
}

public struct UnitType : IComponentData
{
    public int value;
}

public readonly partial struct TargetDataAspect : IAspect
{
    readonly RefRW<TargetData> targetData;
    readonly RefRW<MovementDestination> movementTarget;
    readonly RefRO<SteeringAgent> steeringAgent;
    readonly RefRO<LocalTransform> transfrom;

    readonly EnabledRefRW<TargetInRange> enabledRefRW;

    private float PredictionAmount => steeringAgent.ValueRO.predictionAmount;

    public float3 Position => transfrom.ValueRO.Position;

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

    public void SetTargetPosition(float3 targetPos)
    {
        movementTarget.ValueRW.targetPosition = targetPos;
        movementTarget.ValueRW.distanceToTarget = math.distance(Position, targetPos);

        var targetInRange = movementTarget.ValueRO.distanceToTarget <= steeringAgent.ValueRO.stopRange;

        TargetInRange = targetInRange;
    }

    public void SetTargetPosition(float3 targetPos, float3 targetDirection)
    {
        targetPos += targetDirection * PredictionAmount;

        SetTargetPosition(targetPos);
    }
}