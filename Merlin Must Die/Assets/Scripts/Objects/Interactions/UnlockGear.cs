using System;
using UnityEngine;

[System.Serializable]
public class UnlockGear : Interaction
{
    [SerializeField] private Canvas _gearCanvas;

    public override void Behaviour()
    {
        FindAnyObjectByType<Player>().HasGear = true;

        //todo: add transition?
        _gearCanvas.sortingOrder = 12;
    }
}
