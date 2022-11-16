using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public struct UnitInfo
{
    public string target;
    public string energy;
    public string distanceToTarget;
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class StatisticsSystem : SystemBase
{
    private EntityQuery selectedQuery;

    private EntityQuery seekerQuery;

    protected override void OnCreate()
    {
        seekerQuery = GetEntityQuery(typeof(TargetSeeker));
    }

    protected override void OnUpdate()
    {
        var unitInfo = new UnitInfo();
        unitInfo.target = "";

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        Entities.WithNone<SelectorInit>()
            .ForEach((Entity e, ref Selector selector) =>
            {
                if (seekerQuery.CalculateEntityCount() == 0)
                    return;

                var seeker = seekerQuery.ToEntityArray(Unity.Collections.Allocator.Temp)[0];

                EntityManager.AddComponent<Selected>(seeker);

                EntityManager.AddComponent<SelectorInit>(e);

            }).WithStructuralChanges().Run();

        Entities.ForEach((Entity e, ref Selector selector) =>
        {
            if (Input.GetMouseButtonDown(0))
            {
                var input = new RaycastInput
                {
                    Start = ray.origin,
                    End = ray.origin + ray.direction * 3000,
                    Filter = selector.layers
                };

                if (physicsWorld.CastRay(input, out var closets))
                {
                    EntityManager.RemoveComponent<Selected>(selectedQuery);
                    EntityManager.AddComponent<Selected>(closets.Entity);
                }
            }

        }).WithStructuralChanges().Run();

        var ui = UISingleton.singleton;
        if (!ui) return;
        ui.MaxLabelCount = 3;
        Entities.WithAll<Selected>()
            .ForEach((Entity e, in TargetSeekResult seekResult, in MovementTarget targetData, in TargetSeekTimer seekTimer) =>
            {
                ui.SetUnitDebugInfo($"SeekTimer {seekTimer.timer}");
                ui.SetUnitDebugInfo($"Distance to target {(int)targetData.distanceToTarget}");
                ui.SetUnitDebugInfo($"In range {EntityManager.IsComponentEnabled<TargetInRange>(e)}");
                //unitInfo.energy = $"SeekTimer {seekTimer.timer}";
                //unitInfo.distanceToTarget = $"Distance to target {(int)targetData.distanceToTarget}";
                //unitInfo.target = $"In range {EntityManager.IsComponentEnabled<TargetInRange>(e)}";// EntityManager.GetName(seekResult.target);

            }).WithoutBurst().WithStoreEntityQueryInField(ref selectedQuery).Run();

    }
}