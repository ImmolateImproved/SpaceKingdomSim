using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(UpdateTargetDataSystem))]
public partial struct SteeringSystem : ISystem
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
        new SteeringJob
        {

        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithNone(typeof(InactiveState))]
    partial struct SteeringJob : IJobEntity
    {
        public void Execute(SteeringAgentAspect steeringAspect, in MovementDestination targetData)
        {
            if (!targetData.isTargetPositionValid)
                return;

            //var missingHpFraction = 1f - (health.current / health.max);

            var attractionForce = 1;// steeringAspect.AttractionForce + (missingHpFraction * steeringAspect.AdditionalAttraction);
            steeringAspect.Steer(targetData.targetPosition, attractionForce);
        }
    }
}