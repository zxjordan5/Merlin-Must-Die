using System.Collections.Generic;
using UnityEngine;

public class InteractAfterDialogue : MonoBehaviour
{
    [SerializeField] protected GameObject _interactionObj;

    /// <summary>
    /// Interactions to activate after all NPC dialogue has been seen
    /// </summary>
    private List<Interaction> _finishedInteractions = new List<Interaction>();
    protected bool _finishedInteractionsActivated = false;
    private ShowDialogue _dialogue;

    /// <summary>
    /// What count of dialogue should the activation happen at, 
    /// if less than 0, the interactions will activate after all dialogue has been seen
    /// </summary>
    [SerializeField] private int _activateAtDialogueCount = -1;

    public void Awake()
    {
        _dialogue = GetComponent<ShowDialogue>();

        foreach (Interaction interaction in _interactionObj.GetComponents<Interaction>())
        {
            _finishedInteractions.Add(interaction);
        }
    }

    public void Update()
    {
        if (!_finishedInteractionsActivated && (_dialogue.DialogueSeen == _activateAtDialogueCount || _dialogue.AllDialogueSeen))
        {
            DialogueFinished();
        }
    }

    /// <summary>
    /// When all dialogue has been seen for the first time, activate all finished interactions 
    /// 
    /// TO DO: Replace with a Unity event
    /// </summary>
    public void DialogueFinished()
    {
        foreach (Interaction interaction in _finishedInteractions)
        {
            interaction.Behaviour();
            _finishedInteractionsActivated = true;
        }
    }
}
