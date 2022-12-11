using Unity.Entities;
using UnityEngine;

public class ShootingAuthoring : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject projectileSpawnPoint;
    public float projectileSpeed;
    public float shootingDistance;
    public float timeBetweenShot;

    class ShootingBaker : Baker<ShootingAuthoring>
    {
        public override void Bake(ShootingAuthoring authoring)
        {
            AddComponent(new AIShootingDecision
            {
                shootingDistance = authoring.shootingDistance
            });

            AddComponent(new ShootingData
            {
                projectilePrefab = GetEntity(authoring.projectilePrefab),
                projectileSpawnPoint = GetEntity(authoring.projectileSpawnPoint),
                projectileSpeed = authoring.projectileSpeed,
                cooldown = new Timer(authoring.timeBetweenShot, true)
            });
        }
    }
}