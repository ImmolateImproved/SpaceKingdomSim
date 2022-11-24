using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(FindTargetSystem))]
[UpdateBefore(typeof(UpdateTargetDataSystem))]
[DisableAutoCreation]
public partial struct OutOfBoundsSteeringSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OutOfBoundSteering>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var outOfBoundSteeringData = SystemAPI.GetSingleton<OutOfBoundSteering>();

        new BoundsJob
        {
            outOfBoundData = outOfBoundSteeringData

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct BoundsJob : IJobEntity
    {
        public OutOfBoundSteering outOfBoundData;

        public void Execute(TargetDataAspect targetDataAspect)
        {
            if (!outOfBoundData.squareBounds.Contains(targetDataAspect.Position))
            {
                targetDataAspect.IsTargetExist = false;
                targetDataAspect.IsTargetPositionValid = true;

                targetDataAspect.Update(outOfBoundData.squareBounds.center);
            }
        }
    }
}