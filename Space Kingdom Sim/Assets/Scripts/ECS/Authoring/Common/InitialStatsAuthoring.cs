using Unity.Entities;
using UnityEngine;

public class InitialStatsAuthoring : MonoBehaviour
{
    public InitialSeekerStats mutationData;

    public class MutationDataBaker : Baker<InitialStatsAuthoring>
    {
        public override void Bake(InitialStatsAuthoring authoring)
        {
            var random = new Unity.Mathematics.Random((uint)(System.DateTime.Now.Millisecond + authoring.GetInstanceID()));

            authoring.mutationData.random = random;

            AddComponent(authoring.mutationData);
        }
    }
}