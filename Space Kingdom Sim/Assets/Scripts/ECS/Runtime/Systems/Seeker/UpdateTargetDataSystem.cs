﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
        var translationLookup = SystemAPI.GetComponentLookup<Translation>(true);
        var rotationLookup = SystemAPI.GetComponentLookup<Rotation>(true);

        var mousePos = SystemAPI.GetSingleton<MousePosition>().value;

        new UpdateTargetDataJob
        {
            translationLookup = translationLookup,
            rotationLookup = rotationLookup

        }.ScheduleParallel();

        new SetTargetToMousePosition
        {
            mousePos = mousePos

        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithNone(typeof(FollowMouse))]
    [WithEntityQueryOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    partial struct UpdateTargetDataJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<Translation> translationLookup;

        [ReadOnly]
        public ComponentLookup<Rotation> rotationLookup;

        public void Execute(TargetDataAspect targetDataAspect, in TargetSeekResult seekResult)
        {
            targetDataAspect.IsTargetValid = translationLookup.HasComponent(seekResult.target);
            targetDataAspect.IsTargetPositionValid = targetDataAspect.IsTargetValid;

            if (!targetDataAspect.IsTargetValid)
            {
                targetDataAspect.TargetInRange = false;
                return;
            }

            var targetPos = translationLookup[seekResult.target].Value;
            var targetDirection = math.forward(rotationLookup[seekResult.target].Value);

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
            targetDataAspect.IsTargetValid = false;
            targetDataAspect.IsTargetPositionValid = true;
            mousePos.y = 0;
            targetDataAspect.Update(mousePos, float3.zero);
        }
    }
}