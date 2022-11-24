using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

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

        new FindTargetJob
        {
            physicsWorld = physicsWorld,
            unitTypeLookup = unitTypeLookup,
            inactiveStateLookup = inactiveStateLookup

        }.ScheduleParallel();
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

        public void Execute(Entity e, in Translation translation, in TargetSeeker seeker, ref TargetData targetData)
        {
            if (unitTypeLookup.HasComponent(targetData.target)) return;

            var input = new PointDistanceInput
            {
                Position = translation.Value,
                MaxDistance = seeker.searchRadius,
                Filter = seeker.targetLayer
            };

            var collector = new FindTargetCollector(e, seeker.searchRadius, unitTypeLookup, inactiveStateLookup);
            physicsWorld.CalculateDistance(input, ref collector);

            var result = collector.FindTarget();

            targetData.target = result.target;
            targetData.targetType = result.targetType;
        }
    }

    public struct TargetSeekResult
    {
        public Entity target;
        public float distanceToTarget;
        public float targetType;
    }

    private struct FindTargetCollector : ICollector<DistanceHit>
    {
        public bool EarlyOutOnFirstHit => false;

        public float MaxFraction { get; private set; }

        public int NumHits { get; private set; }

        [ReadOnly]
        public ComponentLookup<UnitType> unitTypeLookup;

        [ReadOnly]
        public ComponentLookup<InactiveState> inactiveStateLookup;

        public NativeList<TargetSeekResult> targetSeekResults;

        private Entity entityToIgnore;

        public FindTargetCollector(Entity entityToIgnore, float maxDistance, ComponentLookup<UnitType> unitTypeLookup, ComponentLookup<InactiveState> inactiveStateLookup)
        {
            MaxFraction = maxDistance;
            NumHits = 0;

            targetSeekResults = new NativeList<TargetSeekResult>(4, Allocator.Temp);

            this.entityToIgnore = entityToIgnore;
            this.unitTypeLookup = unitTypeLookup;
            this.inactiveStateLookup = inactiveStateLookup;
        }

        public bool AddHit(DistanceHit hit)
        {
            var ignoreUnit = hit.Entity == entityToIgnore
                || inactiveStateLookup.HasComponent(hit.Entity)
                || !unitTypeLookup.HasComponent(hit.Entity);

            if (ignoreUnit) return false;

            var targetSeekResult = new TargetSeekResult
            {
                target = hit.Entity,
                distanceToTarget = hit.Distance,
                targetType = unitTypeLookup[hit.Entity].value
            };

            targetSeekResults.Add(targetSeekResult);

            return false;
        }

        public TargetSeekResult FindTarget()
        {
            var result = new TargetSeekResult
            {
                distanceToTarget = float.PositiveInfinity
            };

            for (int i = 0; i < targetSeekResults.Length; i++)
            {
                var targetData = targetSeekResults[i];

                if (targetData.distanceToTarget <= result.distanceToTarget)
                {
                    result = targetData;
                }
            }

            return result;
        }
    }
}