using System;
using UnityEngine;

/// <summary>
/// Interaction script that toggles the players gear variable
/// and shows the grimoire UI layer
/// </summary>
[System.Serializable]
public class UnlockGear : Interaction
{
    
    private Canvas _gearCanvas;

    private void Awake()
    {
        _gearCanvas = GameObject.Find("GrimoireUI").GetComponent<Canvas>();
    }

    public override void Behaviour()
    {
        FindAnyObjectByType<Player>().HasGear = true;

        _gearCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
    }
}
