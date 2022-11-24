using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(SpawnerSystem))]
[UpdateAfter(typeof(SeekerSpawnerSystem))]
public partial struct InactiveStateSystem : ISystem
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

        var dt = SystemAPI.Time.DeltaTime;

        new InactiveStateJob
        {
            dt = dt,
            ecb = ecb

        }.Schedule();
    }

    [BurstCompile]
    partial struct InactiveStateJob : IJobEntity
    {
        public EntityCommandBuffer ecb;

        public float dt;

        public void Execute(Entity e, ref InactiveState inactiveState)
        {
            inactiveState.timer += dt;

            if (inactiveState.timer >= inactiveState.duration)
            {
                ecb.RemoveComponent<InactiveState>(e);
            }
        }
    }
}