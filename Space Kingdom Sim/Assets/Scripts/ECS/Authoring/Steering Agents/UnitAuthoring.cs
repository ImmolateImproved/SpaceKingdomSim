using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public int unitType;

    class TargetBaker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            AddComponent(new UnitType
            {
                value = authoring.unitType
            });
        }
    }
}