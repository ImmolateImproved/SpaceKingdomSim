using Unity.Entities;
using UnityEngine;

using Random = Unity.Mathematics.Random;

[System.Serializable]
public struct SpawnRequestData
{
    public GameObject prefab;
    public int count;
}

public class SpawnerAuthoring : MonoBehaviour
{
    public BoundsData boundsData;

    public SpawnRequestData[] spawnRequests;

    public float timeBetweenSpawns;

    class GridSpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var boundsData = authoring.boundsData;

            DependsOn(boundsData);
            DependsOn(boundsData.transform);

            var random = Random.CreateFromIndex((uint)(System.DateTime.Now.Millisecond + authoring.GetInstanceID()));

            if (authoring.timeBetweenSpawns > 0)
            {
                AddComponent(new SpawnTimer
                {
                    max = authoring.timeBetweenSpawns

                });
            }

            switch (boundsData.boundsType)
            {
                case BoundsEnum.Square:
                    {
                        AddComponent(new GridPositionFactory
                        {
                            maxPosition = boundsData.bounds,
                            minPosition = -boundsData.bounds,
                            offset = authoring.transform.position,
                            random = random
                        });

                        break;
                    }
                case BoundsEnum.Circle:
                    {
                        AddComponent(new CircularPositionFactory
                        {
                            center = authoring.transform.position,
                            maxRadius = boundsData.maxRadius,
                            random = random
                        });
                        break;
                    }
            }

            var requests = AddBuffer<SpawnRequest>();

            for (int i = 0; i < authoring.spawnRequests.Length; i++)
            {
                requests.Add(new SpawnRequest
                {
                    count = authoring.spawnRequests[i].count,
                    prefab = GetEntity(authoring.spawnRequests[i].prefab),
                });
            }
        }
    }
}