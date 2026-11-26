using System;
using UnityEngine;

[System.Serializable]
public class UnlockGear : Interaction
{
    private Canvas _gearCanvas;

    private void Awake()
    {
        _gearCanvas = GameObject.Find("GrimoireUI").GetComponent<Canvas>();
        _gearCanvas.sortingOrder = -1;
    }

    public override void Behaviour()
    {
        FindAnyObjectByType<Player>().HasGear = true;

        //todo: add transition?
        _gearCanvas.sortingOrder = 12;
    }
}
