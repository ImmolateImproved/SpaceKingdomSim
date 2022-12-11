using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(UpdateTargetDataSystem))]
public partial struct AIShootingDecisionSystem : ISystem
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
        new AIShootingDecisionJob
        {

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct AIShootingDecisionJob : IJobEntity
    {
        public void Execute(ref AIShootingDecision shootingDecision, in MovementDestination movementDestination)
        {
            shootingDecision.shoot = shootingDecision.shootingDistance >= movementDestination.distanceToTarget;
        }
    }
}