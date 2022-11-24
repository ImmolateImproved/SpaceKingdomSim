using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[System.Serializable]
public struct SeekerData
{
    public float unitType;
    public float attractionForce;
    public float additionalAttraction;
    public float slowRadius;
    [Min(1f)]
    public float stopRange;
    public float predictionAmount;
    public float searchRadius;
    public PhysicsCategoryTags belongsTo;
    public PhysicsCategoryTags collidesWith;
}

public class SeekerAuthoring : MonoBehaviour
{
    public bool followMouse;

    public float maxSpeed;

    public float maxForce;

    public SeekerData seekerData;

    class SeekerBaker : Baker<SeekerAuthoring>
    {
        public override void Bake(SeekerAuthoring authoring)
        {
            AddComponent(new InactiveState());
            AddComponent(new TargetInRange());
            AddComponent(new TargetData());
            AddComponent(new PhysicsData());
            AddComponent(new MovementTarget());

            AddComponent(new MaxSpeed
            {
                value = authoring.maxSpeed
            });

            var seekerData = authoring.seekerData;

            AddComponent(new SteeringAgent
            {
                attractionForce = seekerData.attractionForce,
                additionalAttraction = seekerData.additionalAttraction,
                predictionAmount = seekerData.predictionAmount,
                maxForce = authoring.maxForce,
                slowRadius = seekerData.slowRadius
            });

            AddComponent(new UnitType { value = seekerData.unitType });

            AddComponent(new TargetSeeker
            {
                searchRadius = seekerData.searchRadius,
                stopRange = seekerData.stopRange,

                targetLayer = new Unity.Physics.CollisionFilter
                {
                    BelongsTo = seekerData.belongsTo.Value,
                    CollidesWith = seekerData.collidesWith.Value
                }
            });

            if (authoring.followMouse)
            {
                AddComponent(new FollowMouse());
            }

        }
    }
}