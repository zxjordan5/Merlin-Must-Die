using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Abstact interactable class
/// Each interactable should have a collider attached as a child
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    private List<Interaction> _activeInteractions = new List<Interaction>();

    private Collider2D _interactionArea;

    public virtual void Awake()
    {
        _interactionArea = transform.Find("InteractionHitbox").GetComponent<Collider2D>();

        //Get the reference to all attached interactions
        foreach (Interaction interaction in GetComponents<Interaction>()) {
            _activeInteractions.Add(interaction);
        }
    }

    public abstract void OnInteract();
    public virtual void Release() { }
    
    /// <summary>
    /// Runs the interaction behavior for each attached interactable
    /// </summary>
    public virtual void ActivateInteractions()
    {
        foreach (Interaction interaction in _activeInteractions)
        {
            interaction.Behaviour();
        }
    }
}
