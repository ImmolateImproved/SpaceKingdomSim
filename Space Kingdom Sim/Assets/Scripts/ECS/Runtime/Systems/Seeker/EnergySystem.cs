using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct EnergySystem : ISystem
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
        var dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var energyLookup = SystemAPI.GetComponentLookup<Energy>(true);

        //new TargetInDistanceJob
        //{
        //    energyLookup = energyLookup

        //}.ScheduleParallel();

        new EnergyJob
        {
            ecb = ecb.AsParallelWriter(),
            dt = dt

        }.ScheduleParallel();
    }

    //[BurstCompile]
    //partial struct TargetInDistanceJob : IJobEntity
    //{
    //    [ReadOnly]
    //    [NativeDisableContainerSafetyRestriction]
    //    public ComponentLookup<Energy> energyLookup;

    //    public void Execute(Entity e, ref Energy energy, ref TargetSeekResult targetSeekResult, in UnitType unitType)
    //    {
    //        energy.current += energyLookup[targetSeekResult.target].current;

    //        energy.current = math.clamp(energy.current, 0, energy.max);
    //    }
    //}

    [BurstCompile]
    partial struct EnergyJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public float dt;

        public void Execute(Entity e, [ChunkIndexInQuery] int chunkIndex, ref Energy energy)
        {
            energy.current -= energy.decreasePerSeconds * dt;

            if (energy.current <= 0)
            {
                ecb.DestroyEntity(chunkIndex, e);
            }
        }
    }
}