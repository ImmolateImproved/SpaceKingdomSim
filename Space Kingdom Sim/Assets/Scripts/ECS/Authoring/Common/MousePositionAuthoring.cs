using Unity.Entities;
using UnityEngine;

public class MousePositionAuthoring : MonoBehaviour
{
    class InputBaker : Baker<MousePositionAuthoring>
    {
        public override void Bake(MousePositionAuthoring authoring)
        {
            AddComponent(new MousePosition());
        }
    }
}