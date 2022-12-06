using Unity.Burst;
using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(SpawnerSystem))]
public partial struct SeekerSpawnerSystem : ISystem
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
        var steeringAgentsLookup = SystemAPI.GetComponentLookup<SteeringAgent>();
        var maxSpeedLookup = SystemAPI.GetComponentLookup<MaxSpeed>();

        foreach (var (spawner, gridPositionFabric, initialStats) in SystemAPI.Query<SpawnerAspect, RefRW<GridPositionFactory>, RefRW<InitialSeekerStats>>().WithNone<SpawnTimer>())
        {
            ref var stats = ref initialStats.ValueRW;

            for (int i = 0; i < spawner.SpawnRequestCount; i++)
            {
                var seekerEntities = spawner.Spawn(ref state, ref gridPositionFabric.ValueRW, i);

                for (int j = 0; j < seekerEntities.Length; j++)
                {
                    var seeker = seekerEntities[j];

                    ref var steeringAgent = ref steeringAgentsLookup.GetRefRW(seeker, false).ValueRW;
                    steeringAgent.maxForce = stats.GetMaxForce();

                    ref var maxSpeed = ref maxSpeedLookup.GetRefRW(seeker, false).ValueRW;
                    maxSpeed.value = stats.GetMaxSpeed();
                }
            }

            spawner.Clear();
        }
    }
}