using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(FindTargetSystem))]
public partial struct UpdateTargetDataSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var localTransfrom = SystemAPI.GetComponentLookup<LocalTransform>(true);

        new UpdateTargetDataJob
        {
            localTransfromLookup = localTransfrom

        }.ScheduleParallel();

        if (SystemAPI.TryGetSingleton<MousePosition>(out var mousePos))
        {
            new SetTargetToMousePosition
            {
                mousePos = mousePos.value

            }.ScheduleParallel();
        }

        if (!SystemAPI.TryGetSingleton<OutOfBoundSteering>(out var outOfBoundSteeringData))
            return;

        new OutOfBoundsJob
        {
            outOfBoundData = outOfBoundSteeringData

        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithNone(typeof(FollowMouse))]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    partial struct UpdateTargetDataJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> localTransfromLookup;

        public void Execute(TargetDataAspect targetDataAspect)
        {
            targetDataAspect.IsTargetExist = localTransfromLookup.HasComponent(targetDataAspect.Target);
            targetDataAspect.IsTargetPositionValid = targetDataAspect.IsTargetExist;

            if (!targetDataAspect.IsTargetExist)
            {
                targetDataAspect.TargetInRange = false;
                return;
            }

            var targetPos = localTransfromLookup[targetDataAspect.Target].Position;
            var targetDirection = localTransfromLookup[targetDataAspect.Target].Forward();

            targetDataAspect.SetTargetPosition(targetPos, targetDirection);
        }
    }

    [BurstCompile]
    [WithAll(typeof(FollowMouse))]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    partial struct SetTargetToMousePosition : IJobEntity
    {
        public float3 mousePos;

        public void Execute(TargetDataAspect targetDataAspect)
        {
            targetDataAspect.IsTargetExist = false;
            targetDataAspect.IsTargetPositionValid = true;

            mousePos.y = 0;
            targetDataAspect.SetTargetPosition(mousePos);
        }
    }

    [BurstCompile]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    partial struct OutOfBoundsJob : IJobEntity
    {
        public OutOfBoundSteering outOfBoundData;

        public void Execute(TargetDataAspect targetDataAspect)
        {
            if (!outOfBoundData.squareBounds.Contains(targetDataAspect.Position))
            {
                targetDataAspect.IsTargetExist = false;
                targetDataAspect.IsTargetPositionValid = true;

                targetDataAspect.SetTargetPosition(outOfBoundData.squareBounds.center);
            }
        }
    }
}