using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Aspects;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(ShootingSystem))]
public partial struct DamageOnCollisionSystem : ISystem
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
        state.Dependency = new DamageOnCollisionJob
        {
            physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>(),
            damageBufferLookup = SystemAPI.GetBufferLookup<DamageAccumulator>(),

        }.ScheduleParallel(state.Dependency);

        new ApplyDamageJob().ScheduleParallel();
    }

    [BurstCompile]
    partial struct DamageOnCollisionJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        public BufferLookup<DamageAccumulator> damageBufferLookup;

        [ReadOnly]
        public PhysicsWorldSingleton physicsWorld;

        public void Execute(ColliderAspect colliderAspect, in DamageOnCollision attack)
        {
            if (physicsWorld.CalculateDistance(colliderAspect, attack.damageRadius, out var closestHit))
            {
                var damageBuffer = damageBufferLookup[closestHit.Entity];

                damageBuffer.Add(new DamageAccumulator { value = 5 });
            }
        }
    }

    [BurstCompile]
    [WithChangeFilter(typeof(DamageAccumulator))]
    partial struct ApplyDamageJob : IJobEntity
    {
        public void Execute(ref Health health, ref DynamicBuffer<DamageAccumulator> damageBuffer)
        {
            for (int i = 0; i < damageBuffer.Length; i++)
            {
                health.current -= damageBuffer[i].value;
            }

            damageBuffer.Clear();
        }
    }
}