using UnityEngine;
using TMPro;
using System.Collections.Generic;


/// <summary>
/// Currently the same as an instant interaction that plays dialogue
/// </summary>
public class NPC : Interactable
{

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnInteract()
    {
        ActivateInteractions();
    }

 
}
