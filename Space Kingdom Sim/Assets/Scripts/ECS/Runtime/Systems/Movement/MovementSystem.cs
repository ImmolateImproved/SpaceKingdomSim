using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Transforms;

[BurstCompile]
public partial struct MovementSystem : ISystem
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
        var dt = SystemAPI.Time.DeltaTime;

        new CalculateVelocityJob
        {

        }.ScheduleParallel();

        new MovementJob
        {
            dt = dt

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct CalculateVelocityJob : IJobEntity
    {
        public float dt;

        public void Execute(PhysicsBodyAspect physicsBodyAspect)
        {
            physicsBodyAspect.CalculateVelocity();
        }
    }

    [BurstCompile]
    [WithNone(typeof(InactiveState), typeof(TargetInRange))]
    partial struct MovementJob : IJobEntity
    {
        public float dt;

        public void Execute(ref Translation translation, ref PhysicsData physicsData)
        {
            translation.Value += physicsData.velocity * dt;
        }
    }
}