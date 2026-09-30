/// Grimoire Class
/// Contains the full grimoire and a list of active spells in the level
/// Reads player input and finds the matching spell code
/// Shows cooldowns for spells
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class Grimoire : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private List<Spell> allSpells;
    private List<Spell> learnedSpells;

    /// <summary>
    /// List of spells that the player currently has available within the level
    /// </summary>
    [SerializeField]
    private List<Spell> activeSpells;

    /// <summary>
    /// Current sequence of inputs the player has entered
    /// </summary>
    private List<Direction> playerInputSequence;


    /// <summary>
    /// The index of the current input in the playerInput list. Reset to zero when spell input sequence is restarted
    /// </summary>
    private int inputSequenceIndex;

    // Displayed text in the UI
    [SerializeField]
    private TextMeshProUGUI grimoireTextUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get all spells from spell class; sort into .learned and .active
        playerInputSequence = new List<Direction>();
        GetGrimoireText();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    /// <summary>
    /// Gets all spells to store them; filters by learned and active
    /// MAY BE DEPRECATED
    /// </summary>
    /// <returns></returns>
    // void GetSpells()
    // {   
    //     // add learned and active spells to their lists as needed
    //     for (int i = 0; i < allSpells.Count; i++)
    //     {
    //         if (allSpells[i])
    //         {
    //             activeSpells.Add(activeSpells[i]);
    //         }
    //     }
    // }

    /// <summary>
    /// Gets all grimoire text and displays it in the UI
    /// </summary>
    /// <returns></returns>
    void GetGrimoireText()
    {
        // The initial text as a string
        string text = "";

        for (int i = 0; i < activeSpells.Count; i++)
        {
            text += activeSpells[i].name;
            text += " | ";
            List<Direction> activeSpellCode = activeSpells[i].spellCode;
            for (int j = 0; j < activeSpellCode.Count; j++)
            {
                switch (activeSpellCode[j])
                {
                    case Direction.Up:
                        text += "Up";
                        break;
                    case Direction.Down:
                        text += "Down";
                        break;
                    case Direction.Left:
                        text += "Left";
                        break;
                    case Direction.Right:
                        text += "Right";
                        break;
                }
                text += " ";
            }
            text += '\n';
        }

        // Setting the UI component
        grimoireTextUI.text = text;
    }

    /// <summary>
    /// Updates the player input sequence with the new input and checks for matches with active spell codes
    /// </summary>
    /// <param name="input">The direction of the new input</param>
    public void ProcessInput(Direction input)
    {
        playerInputSequence.Add(input);
        inputSequenceIndex++;
        CheckForMatches();
    }

    public void CheckForMatches()
    {
        foreach(Spell spell in activeSpells)
        {
            if (IsMatch(spell))
            { 
                // TODO: Get a reference to the player and set it's currentSpell
                // Block additional inputs until the spell is cast (do this through an event)
                ResetInputSequence();
                break; // Exit the loop after casting a spell
            }
        }
    }

    private bool IsMatch(Spell spell)
    {
        int codeLength = spell.spellCode.Count;
        if (playerInputSequence.Count < codeLength)
            return false;

        for (int i = 0; i < codeLength; i++)
        {
            // Compare the last 'codeLength' inputs in playerInputSequence with the spell's spellCode
            // Quit out early after any mismatches
            //if (playerInputSequence[playerInputSequence.Count - codeLength + i] != spell.spellCode[i])
            // return false;
            if (playerInputSequence[i] != spell.spellCode[i])
                return false;
        }
        return true;
    }

    public void ResetInputSequence()
    {
        playerInputSequence.Clear();
        inputSequenceIndex = 0;
    }





    // ***** INPUT METHODS *****
    #region Input Methods
    public void OnPrepareSpellUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Preparing Spell Up");
            ProcessInput(Direction.Up);
        }
    }

    public void OnPrepareSpellDown(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("Preparing Spell Down");
            ProcessInput(Direction.Down);
        }
    }

    public void OnPrepareSpellLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("Preparing Spell Left");
            ProcessInput(Direction.Left);
        }
    }

    public void OnPrepareSpellRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("Preparing Spell Right");
            ProcessInput(Direction.Right);
        }
    }
    #endregion
    // *************************
}
