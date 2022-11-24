using Unity.Entities;
using UnityEngine;

public class OutOfBoundSteeringAuthoring : MonoBehaviour
{
    public BoundsData boundsData;
    public float steeringForce;

    class OutOfBoundSteerinBaker : Baker<OutOfBoundSteeringAuthoring>
    {
        public override void Bake(OutOfBoundSteeringAuthoring authoring)
        {
            var boundsData = authoring.boundsData;

            DependsOn(boundsData);
            DependsOn(boundsData.transform);

            var bounds = default(Bounds);

            bounds.center = boundsData.transform.position;
            bounds.extents = boundsData.bounds;

            AddComponent(new OutOfBoundSteering
            {
                squareBounds = bounds,
                steeringForce = authoring.steeringForce

            });
        }
    }
}