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
        foreach (var (spawner, gridPositionFabric, mutationData) in SystemAPI.Query<SpawnerAspect, RefRW<GridPositionFactory>, RefRW<InitialSeekerStats>>().WithNone<SpawnTimer>())
        {
            for (int i = 0; i < spawner.SpawnRequestCount; i++)
            {
                var seekerEntities = spawner.Spawn(ref state, ref gridPositionFabric.ValueRW, i);

                for (int j = 0; j < seekerEntities.Length; j++)
                {
                    var steeringAgent = SystemAPI.GetComponent<SteeringAgent>(seekerEntities[j]);
                    steeringAgent.maxForce = mutationData.ValueRW.GetMaxForce();
                    SystemAPI.SetComponent(seekerEntities[j], steeringAgent);
                    
                    var maxSpeed = SystemAPI.GetComponent<MaxSpeed>(seekerEntities[j]);
                    maxSpeed.value = mutationData.ValueRW.GetMaxSpeed();
                    SystemAPI.SetComponent(seekerEntities[j], maxSpeed);

                    //var foodSeekerData = targetSeekerLookup[seekerEntities[j]];
                    //foodSeekerData.attractionForce = mutationData.ValueRW.GetAttractionForce();
                    //foodSeekerData.searchRadius = mutationData.ValueRW.GetFoodSearchRadius();
                    //SystemAPI.SetComponent(seekerEntities[j], foodSeekerData);
                }
            }

            spawner.Clear();
        }
    }
}