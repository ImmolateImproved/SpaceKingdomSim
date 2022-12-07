using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[System.Serializable]
public struct SeekerData
{
    public int unitType;
    public float attractionForce;
    public float additionalAttraction;
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

    public float slowRadius;
    [Min(1f)]
    public float stopRange = 1;

    public SeekerData seekerData;

    class SeekerBaker : Baker<SeekerAuthoring>
    {
        public override void Bake(SeekerAuthoring authoring)
        {
            AddComponent(new InactiveState());
            AddComponent(new TargetInRange());
            AddComponent(new TargetData());
            AddComponent(new MovementDestination());

            var seekerData = authoring.seekerData;

            AddComponent(new SteeringAgent
            {
                maxForce = authoring.maxForce,
                maxSpeed = authoring.maxSpeed,
                stopRange = authoring.stopRange,
                slowRadius = authoring.slowRadius,
                attractionForce = seekerData.attractionForce,
                additionalAttraction = seekerData.additionalAttraction,
                predictionAmount = seekerData.predictionAmount
            });

            AddComponent(new UnitType { value = seekerData.unitType });

            AddComponent(new PerceptionData
            {
                searchRadius = seekerData.searchRadius,

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