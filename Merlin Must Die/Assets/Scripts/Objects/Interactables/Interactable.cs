using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Abstact interactable class
/// Each interactable should have a collider attached as a child
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    private List<Interaction> _activeInteractions = new List<Interaction>();

    [SerializeField] private Collider2D _interactionArea;
    public bool BeingHeld { get; set; }

    public virtual void Awake()
    {
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
