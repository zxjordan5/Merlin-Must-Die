/// Grimoire Class
/// Contains the full grimoire and a list of active spells in the level
/// Reads player input and finds the matching spell code
/// Shows cooldowns for spells
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
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


    // TODO: MOVE THESE TO A SCRIPTABLE OBJECT
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

    /// <summary>
    /// The spell codes actively being displayed
    /// </summary>
    private List<string> grimoireText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // get all spells from spell class; sort into .learned and .active
        playerInputSequence = new List<Direction>();
        GetSpells();
    }
    

    // Update is called once per frame
    void Update()
    {
      
    }

    // int is currently a placeholder type
    /// <summary>
    /// Gets all spells to store them; filters by learned and active
    /// </summary>
    /// <returns></returns>
    void GetSpells()
    {
        // allSpells = // get from list somewhere else in the code??
        
        // add learned and active spells to their lists as needed
        //for (int i = 0; i < allSpells.Count; i++)
        //{
            /// if (allSpells[i].learned)
            /// {
            ///     learnedSpells.Add(allSpells[i]);
            /// }
            /// if (allSpells[i].active)
            /// {
            ///     activeSpells.Add(allSpells[i]);
            /// }
        //}
    }

    /// <summary>
    /// Stores all grimoire text (spell names + spell codes) in a List to be presented on screen
    /// </summary>
    /// <returns></returns>
    int GetGrimoireText()
    {
        List<string> text = new List<string>();
        for (int i = 0; i < activeSpells.Count; i++)
        {
            text.Add(/*activeSpells[i].spellCode*/ "spellCode " + " | " + " Spell"); // + activeSpells[i].name
        }
        return 0;
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
        // TODO: Add debug stuff to check all this
        int codeLength = spell.spellCode.Count;
        if (playerInputSequence.Count < codeLength || spell.OnCooldown)
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
