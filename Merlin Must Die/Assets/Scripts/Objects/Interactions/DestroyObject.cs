using System;
using UnityEngine;

/// <summary>
/// Simple interaction script to destroy an assigned object
/// </summary>
[System.Serializable]
public class DestroyObject : Interaction
{
    [SerializeField] private GameObject _target;

    public override void Behaviour()
    {
        Destroy(_target);
    }
}
