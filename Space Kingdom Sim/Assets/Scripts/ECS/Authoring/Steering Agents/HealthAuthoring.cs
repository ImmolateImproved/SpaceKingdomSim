using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    public float max;
    public float startHealth;

    class HealthBaker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            AddComponent(new Health
            {
                max = authoring.max,
                current = authoring.startHealth
            });

            AddBuffer<DamageAccumulator>();
        }
    }
}