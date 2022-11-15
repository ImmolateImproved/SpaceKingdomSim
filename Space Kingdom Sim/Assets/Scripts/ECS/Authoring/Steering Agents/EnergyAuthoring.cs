using Unity.Entities;
using UnityEngine;

public class EnergyAuthoring : MonoBehaviour
{
    public float maxEnergy;
    public float startEnergy;
    public float decreasePerSeconds;

    class EnergyBaker : Baker<EnergyAuthoring>
    {
        public override void Bake(EnergyAuthoring authoring)
        {
            AddComponent(new Energy
            {
                max = authoring.maxEnergy,
                current = authoring.startEnergy,
                decreasePerSeconds = authoring.decreasePerSeconds
            });
        }
    }
}