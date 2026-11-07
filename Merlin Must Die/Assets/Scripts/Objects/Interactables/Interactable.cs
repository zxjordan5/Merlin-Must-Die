using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Abstact interactable class
/// Each interactable should have a collider attached as a child
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    protected List<Interaction> activeInteractions = new List<Interaction>();

    protected Collider2D interactionArea;

    public virtual void Awake()
    {
        interactionArea = transform.Find("InteractionCollider").GetComponent<Collider2D>();

        //Get the reference to all attached interactions
        foreach (Interaction interaction in GetComponents<Interaction>()) {
            activeInteractions.Add(interaction);
        }
    }

    public abstract void OnInteract();

    /// <summary>
    /// Runs the interaction behavior for each attached interactable
    /// </summary>
    public virtual void ActivateInteractions()
    {
        foreach (Interaction interaction in activeInteractions)
        {
            interaction.Behaviour();
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other) 
    {
        foreach(Interaction interaction in activeInteractions)
        {
            interaction.Release();
        }
    }
}
