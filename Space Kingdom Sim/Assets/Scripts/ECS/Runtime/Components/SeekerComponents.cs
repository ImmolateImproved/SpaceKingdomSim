using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct TargetInRange : IComponentData, IEnableableComponent
{

}

public struct FollowMouse : IComponentData
{

}

public struct TargetSeekResult : IComponentData
{
    public Entity target;
    public float targetType;
}

public struct TargetData : IComponentData
{
    public float3 targetPosition;

    public float distanceToTarget;

    public bool isTargetPositionValid;
    public bool isTargetEntityValid;
}

public struct TargetSeeker : IComponentData
{
    public float searchRadius;
    public float stopRange;
}

public struct TargetSeekTimer : IComponentData
{
    public float timer;
    public float delayBetweenTargetSearch;
}

public struct UnitType : IComponentData
{
    public float value;
}

[System.Serializable]
public struct InitialSeekerStats : IComponentData
{
    public Range attarctionFroce;
    public Range repultionForce;
    public Range maxSpeed;
    public Range maxForce;
    public Range foodSearchRadius;
    public Range poisonSearchRadius;

    public Unity.Mathematics.Random random;

    public float GetAttractionForce()
    {
        return random.NextFloat(attarctionFroce.min, attarctionFroce.max);
    }

    public float GetRepultionForce()
    {
        return random.NextFloat(repultionForce.min, repultionForce.max);
    }

    public float GetMaxSpeed()
    {
        return random.NextFloat(maxSpeed.min, maxSpeed.max);
    }

    public float GetMaxForce()
    {
        return random.NextFloat(maxForce.min, maxForce.max);
    }

    public float GetFoodSearchRadius()
    {
        return random.NextFloat(foodSearchRadius.min, foodSearchRadius.max);
    }

    public float GetPoisonSearchRadius()
    {
        return random.NextFloat(poisonSearchRadius.min, poisonSearchRadius.max);
    }
}

public readonly partial struct TargetDataAspect : IAspect
{
    readonly RefRW<TargetData> targetData;
    readonly RefRO<TargetSeeker> targetSeeker;
    readonly RefRO<SteeringAgent> steeringAgent;
    readonly RefRO<Translation> translation;

    readonly EnabledRefRW<TargetInRange> enabledRefRW;

    private float PredictionAmount => steeringAgent.ValueRO.predictionAmount;

    public bool TargetInRange
    {
        get => enabledRefRW.ValueRO;
        set => enabledRefRW.ValueRW = value;
    }

    public bool IsTargetValid
    {
        get => targetData.ValueRO.isTargetEntityValid;
        set => targetData.ValueRW.isTargetEntityValid = value;
    }

    public bool IsTargetPositionValid
    {
        get => targetData.ValueRO.isTargetPositionValid;
        set => targetData.ValueRW.isTargetPositionValid = value;
    }

    public void Update(float3 targetPos, float3 targetDirection)
    {
        targetPos += targetDirection * PredictionAmount;
        targetData.ValueRW.targetPosition = targetPos;
        targetData.ValueRW.distanceToTarget = math.distance(translation.ValueRO.Value, targetPos);

        var targetInRange = targetData.ValueRO.distanceToTarget <= targetSeeker.ValueRO.stopRange;

        TargetInRange = targetInRange;
    }
}