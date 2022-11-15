using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using UnityEngine;

[BurstCompile]
public partial struct FindTargetSystem : ISystem
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
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var unitTypeLookup = SystemAPI.GetComponentLookup<UnitType>(true);
        var inactiveStateLookup = SystemAPI.GetComponentLookup<InactiveState>(true);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var dt = SystemAPI.Time.DeltaTime;

        new InactiveStateJob
        {
            dt = dt,
            ecb = ecb

        }.Schedule();

        new FindTargetJob
        {
            physicsWorld = physicsWorld,
            unitTypeLookup = unitTypeLookup,
            inactiveStateLookup = inactiveStateLookup,
            dt = dt

        }.ScheduleParallel();
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

    [BurstCompile]
    [WithNone(typeof(InactiveState))]
    partial struct FindTargetJob : IJobEntity
    {
        [ReadOnly]
        public PhysicsWorldSingleton physicsWorld;

        [ReadOnly]
        public ComponentLookup<UnitType> unitTypeLookup;

        [ReadOnly]
        public ComponentLookup<InactiveState> inactiveStateLookup;

        public float dt;

        public void Execute(in ColliderAspect colliderAspect, in TargetSeeker seeker, ref TargetData targetData, ref TargetSeekResult targetSeekResult, ref TargetSeekTimer targetSeekTimer)
        {
            targetSeekTimer.timer += dt;

            if (targetSeekTimer.timer < targetSeekTimer.delayBetweenTargetSearch) return;

            var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
            physicsWorld.CalculateDistance(colliderAspect, seeker.searchRadius, ref distanceHits);

            var target = Entity.Null;
            var targetType = 0f;
            var distanceToTarget = math.INFINITY;

            for (int j = 0; j < distanceHits.Length; j++)
            {
                var distanceHit = distanceHits[j];

                var ignoreUnit = inactiveStateLookup.HasComponent(distanceHit.Entity)
                                 || !unitTypeLookup.HasComponent(distanceHit.Entity);

                if (ignoreUnit)
                    continue;

                var currentTargetType = unitTypeLookup[distanceHit.Entity].value;

                var currentDistanceToTarget = distanceHit.Distance;

                if (currentDistanceToTarget <= distanceToTarget)
                {
                    target = distanceHit.Entity;
                    targetType = currentTargetType;
                    distanceToTarget = currentDistanceToTarget;
                }
            }

            if (target != Entity.Null)
            {
                targetSeekTimer.timer = 0;
            }

            targetData.distanceToTarget = distanceToTarget;
            targetSeekResult.target = target;
            targetSeekResult.targetType = targetType;
        }
    }

    private struct IngnoreSelfCollector : ICollector<DistanceHit>
    {
        public bool EarlyOutOnFirstHit => false;

        public float MaxFraction { get; private set; }

        public int NumHits { get; private set; }

        public DistanceHit ClosestHit;

        private Entity entityToIgnore;

        public IngnoreSelfCollector(Entity entityToIgnore, float maxDistance)
        {
            this.entityToIgnore = entityToIgnore;

            MaxFraction = maxDistance;
            ClosestHit = default;
            NumHits = 0;
        }

        public bool AddHit(DistanceHit hit)
        {
            if (hit.Entity == entityToIgnore)
            {
                return false;
            }

            ClosestHit = hit;
            MaxFraction = hit.Fraction;
            NumHits = 1;

            return true;
        }
    }
}