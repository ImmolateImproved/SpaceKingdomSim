using UnityEngine;

public enum BoundsEnum
{
    Square, Circle
}

public class BoundsData : MonoBehaviour
{
    public BoundsEnum boundsType;

    public Vector3 bounds;

    public float maxRadius;
}