using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(AIShootingDecisionSystem))]
public partial struct ShootingSystem : ISystem
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
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new ShootingJob
        {
            ecb = ecb.AsParallelWriter(),
            transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
            dt = SystemAPI.Time.DeltaTime

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct ShootingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        [ReadOnly]
        public ComponentLookup<LocalTransform> transformLookup;

        public float dt;

        public void Execute([ChunkIndexInQuery] int chunkIndex, ref ShootingData shootingData, in AIShootingDecision aIShootingDecision, in MovementDestination destination)
        {
            shootingData.cooldown.Tick(dt);

            var shoot = aIShootingDecision.shoot && shootingData.cooldown.IsCompleted;

            if (shoot)
            {
                shootingData.cooldown.Reset();

                var projectile = ecb.Instantiate(chunkIndex, shootingData.projectilePrefab);

                var transform = transformLookup[shootingData.projectilePrefab];

                ref readonly var spawnPoint = ref transformLookup.GetRefRO(shootingData.projectileSpawnPoint).ValueRO;
                transform.Position = spawnPoint.Position;

                ecb.SetComponent(chunkIndex, projectile, transform);
                ecb.SetComponent(chunkIndex, projectile, new PhysicsVelocity { Linear = destination.directionToTarget * shootingData.projectileSpeed });
            }
        }
    }
}