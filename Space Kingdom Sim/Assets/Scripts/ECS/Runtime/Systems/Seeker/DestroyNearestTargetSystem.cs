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

        var translationLookup = SystemAPI.GetComponentLookup<Translation>(true);
        var energyLookup = SystemAPI.GetComponentLookup<Energy>(true);

        state.Dependency = new DestroyNearestTargetJob
        {
            ecb = ecb.AsParallelWriter(),
            translationLookup = translationLookup,
            energyLookup = energyLookup

        }.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(TargetInRange))]
    partial struct DestroyNearestTargetJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<Translation> translationLookup;

        [ReadOnly]
        public ComponentLookup<Energy> energyLookup;

        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute([ChunkIndexInQuery] int chunkIndex, ref TargetSeekResult seekResult, ref TargetSeekTimer targetSeekTimer, in Energy energy)
        {
            if (!seekResult.isTargetExist)
                return;

            if (energy.current >= energyLookup[seekResult.target].current)
            {
                targetSeekTimer.timer = targetSeekTimer.delayBetweenTargetSearch;
                ecb.DestroyEntity(chunkIndex, seekResult.target);
            }
        }
    }
}