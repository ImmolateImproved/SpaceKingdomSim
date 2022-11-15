using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class MousePositionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<MousePosition>();
    }

    protected override void OnUpdate()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Entities.ForEach((ref MousePosition mousePosition) =>
        {
            var plane = new Plane(Vector3.up, Vector3.zero);

            plane.Raycast(ray, out var rayDistance);

            mousePosition.value = ray.GetPoint(rayDistance);

        }).Run();
    }
}
