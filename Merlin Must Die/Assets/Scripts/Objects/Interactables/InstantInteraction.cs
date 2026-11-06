using UnityEngine;
using TMPro;

public class InstantInteraction : Interactable
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
