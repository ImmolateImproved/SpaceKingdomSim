using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(UpdateTargetDataSystem))]
public partial struct DestroyNearestTargetSystem : ISystem
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
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var energyLookup = SystemAPI.GetComponentLookup<Energy>(true);

        state.Dependency = new DestroyNearestTargetJob
        {
            ecb = ecb.AsParallelWriter(),
            energyLookup = energyLookup

        }.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(TargetInRange))]
    partial struct DestroyNearestTargetJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<Energy> energyLookup;

        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute([ChunkIndexInQuery] int chunkIndex, in TargetData targetData, in Energy energy)
        {
            if (!targetData.isTargetExist)
                return;

            if (energy.current >= energyLookup[targetData.target].current)
            {
                ecb.DestroyEntity(chunkIndex, targetData.target);
            }
        }
    }
}