using UnityEngine;

/// <summary>
/// Script for objects that move along a spline of some sort. Holds data of where the parent object is along the spline.
/// </summary>
public class SplineObject : MonoBehaviour
{
    /// <summary>
    /// The position of the object on the spline. Serialized for debugging purposes.
    /// </summary>
    [SerializeField] private float distanceAlongSpline = 0.0f;

    public float DistanceAlongSpline { get => distanceAlongSpline; set => distanceAlongSpline = value; }
}
