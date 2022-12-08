using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class StatisticsSystem : SystemBase
{
    private EntityQuery selectedQuery;

    private EntityQuery seekerQuery;

    protected override void OnCreate()
    {
        seekerQuery = GetEntityQuery(typeof(PerceptionData));
    }

    protected override void OnUpdate()
    {
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

        Entities.WithAll<Selected>()
            .ForEach((Entity e, in TargetData targetData, in MovementDestination movementTarget, in LocalTransform transform) =>
            {
                ui.AddLabel($"My pos: {(int3)transform.Position}");
                ui.AddLabel($"In range: {EntityManager.IsComponentEnabled<TargetInRange>(e)}");
                ui.AddLabel($"Distance to target: {(int)movementTarget.distanceToTarget}");

                var target = "";
                var targetPos = default(int3);

                if (targetData.isTargetExist)
                {
                    target = EntityManager.GetName(targetData.target);
                    targetPos = (int3)EntityManager.GetComponentData<LocalTransform>(targetData.target).Position;
                }

                ui.AddLabel($"Target: {target}");
                ui.AddLabel($"Target pos: {targetPos}");

            }).WithoutBurst().WithStoreEntityQueryInField(ref selectedQuery).Run();

    }
}