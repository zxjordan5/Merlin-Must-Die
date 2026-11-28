using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Currently the same as an instant interaction that plays dialogue
/// Checks if the NPC has dialogue
/// </summary>
public class NPC : Interactable
{
    private ShowDialogue _dialogue;

    public override void Awake()
    {
        base.Awake();

        _dialogue = _interactionObj.GetComponent<ShowDialogue>();
        if(_dialogue == null)
        {
            Debug.Log("NPC has no dialogue");
        }
    }

    public override void OnInteract()
    {
        ActivateInteractions();
    }

 
}
