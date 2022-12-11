using Unity.Entities;
using UnityEngine;

public class DamageOnCollisionAuthoring : MonoBehaviour
{
    public float damage;
    public float damageRadius;

    class DamageOnCollisionBaker : Baker<DamageOnCollisionAuthoring>
    {
        public override void Bake(DamageOnCollisionAuthoring authoring)
        {
            AddComponent(new DamageOnCollision
            {
                damage = authoring.damage,
                damageRadius = authoring.damageRadius
            });
        }
    }
}