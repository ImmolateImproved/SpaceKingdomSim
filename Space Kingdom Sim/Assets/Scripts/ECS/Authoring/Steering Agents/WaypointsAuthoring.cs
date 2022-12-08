using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Waypoint : IBufferElementData
{
    public float3 position;
}


public class WaypointsAuthoring : MonoBehaviour
{
    public Transform[] waypoints;

    class WaypointsBaker : Baker<WaypointsAuthoring>
    {
        public override void Bake(WaypointsAuthoring authoring)
        {
            var buffer = AddBuffer<Waypoint>();

            for (int i = 0; i < authoring.waypoints.Length; i++)
            {
                DependsOn(authoring.waypoints[i]);
                
                buffer.Add(new Waypoint
                {
                    position = authoring.waypoints[i].position
                });
            }
        }
    }
}