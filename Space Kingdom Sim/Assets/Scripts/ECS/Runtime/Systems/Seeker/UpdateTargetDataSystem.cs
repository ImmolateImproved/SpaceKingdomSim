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
        var ltwLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);

        var translationLookup = SystemAPI.GetComponentLookup<Translation>(true);
        var rotationLookup = SystemAPI.GetComponentLookup<Rotation>(true);

        var mousePos = SystemAPI.GetSingleton<MousePosition>().value;

        new UpdateTargetDataJob
        {
            ltwLookup = ltwLookup,
            translationLookup = translationLookup,
            rotationLookup = rotationLookup

        }.ScheduleParallel();

        new SetTargetToMousePosition
        {
            mousePos = mousePos

        }.ScheduleParallel();

        var outOfBoundSteeringData = SystemAPI.GetSingleton<OutOfBoundSteering>();

        new OutOfBoundsJob
        {
            outOfBoundData = outOfBoundSteeringData

        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithNone(typeof(FollowMouse))]
    [WithEntityQueryOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    partial struct UpdateTargetDataJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalToWorld> ltwLookup;

        [ReadOnly]
        public ComponentLookup<Translation> translationLookup;

        [ReadOnly]
        public ComponentLookup<Rotation> rotationLookup;

        public void Execute(TargetDataAspect targetDataAspect)
        {
            targetDataAspect.IsTargetExist = ltwLookup.HasComponent(targetDataAspect.Target);
            targetDataAspect.IsTargetPositionValid = targetDataAspect.IsTargetExist;

            if (!targetDataAspect.IsTargetExist)
            {
                targetDataAspect.TargetInRange = false;
                return;
            }

            var targetPos = translationLookup[targetDataAspect.Target].Value;//ltwLookup[targetDataAspect.Target].Position;
            var targetDirection = ltwLookup[targetDataAspect.Target].Forward;// rotationLookup[targetDataAspect.Target].Value;

            targetDataAspect.Update(targetPos, targetDirection);
        }
    }

    [BurstCompile]
    [WithAll(typeof(FollowMouse))]
    [WithEntityQueryOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    partial struct SetTargetToMousePosition : IJobEntity
    {
        public float3 mousePos;

        public void Execute(TargetDataAspect targetDataAspect)
        {
            targetDataAspect.IsTargetExist = false;
            targetDataAspect.IsTargetPositionValid = true;

            mousePos.y = 0;
            targetDataAspect.Update(mousePos);
        }
    }

    [BurstCompile]
    [WithEntityQueryOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    partial struct OutOfBoundsJob : IJobEntity
    {
        public OutOfBoundSteering outOfBoundData;

        public void Execute(TargetDataAspect targetDataAspect)
        {
            if (!outOfBoundData.squareBounds.Contains(targetDataAspect.Position))
            {
                targetDataAspect.IsTargetExist = false;
                targetDataAspect.IsTargetPositionValid = true;

                targetDataAspect.Update(outOfBoundData.squareBounds.center);
            }
        }
    }
}