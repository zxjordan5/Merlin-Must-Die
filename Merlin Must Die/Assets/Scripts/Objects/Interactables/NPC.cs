using UnityEngine;
using TMPro;
using System.Collections.Generic;


/// <summary>
/// Interactable class that will activate interactions after all NPC dialogue has been seen
/// Requires ShowDialogue as a component
/// </summary>
public class NPC : Interactable
{
    /// <summary>
    /// Interactions to activate after all NPC dialogue has been seen
    /// </summary>
    [SerializeField] protected List<Interaction> finishedInteractions;
    protected bool finishedInteractionsActivated = false;
    protected ShowDialogue dialogue;

    public override void Awake()
    {
        base.Awake();

        dialogue = GetComponent<ShowDialogue>();

        //Dialogue is required for the NPC to detect when all dialogue has seen
        if (dialogue == null)
        {
            Debug.Log("SHOW DIALOGUE MISSING FROM NPC: " + gameObject.name);
        }

        foreach(Interaction interaction in finishedInteractions)
        {
            activeInteractions.Remove(interaction);
        }
    }

    public void Update()
    {
        if (!finishedInteractionsActivated && dialogue.AllDialogueSeen)
        {
            DialogueFinished();
        }
    }

    public override void OnInteract()
    {
        ActivateInteractions();
    }

    /// <summary>
    /// When all dialogue has been seen for the first time, activate all finished interactions 
    /// </summary>
    public void DialogueFinished()
    {
        foreach (Interaction interaction in finishedInteractions)
        {
            interaction.Behaviour();
            finishedInteractionsActivated = true;
        }
    }
}
