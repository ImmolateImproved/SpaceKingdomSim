using Unity.Entities;
using Unity.Physics;

public struct Selector : IComponentData
{
    public CollisionFilter layers;
}

public struct SelectorInit : IComponentData
{

}

public struct Selected : IComponentData
{

}
