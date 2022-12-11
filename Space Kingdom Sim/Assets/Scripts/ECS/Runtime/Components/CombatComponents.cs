using Unity.Collections;
using Unity.Entities;

public struct AIShootingDecision : IComponentData
{
    public float shootingDistance;
    public bool shoot;
}

public struct ShootingData : IComponentData
{
    public Entity projectilePrefab;
    public Entity projectileSpawnPoint;

    public float projectileSpeed;

    public Timer cooldown;
}

public struct DamageOnCollision : IComponentData
{
    public float damage;
    public float damageRadius;
}

public struct Health : IComponentData
{
    public float max;
    public float current;
}

[InternalBufferCapacity(0)]
public struct DamageAccumulator : IBufferElementData
{
    public float value;
}