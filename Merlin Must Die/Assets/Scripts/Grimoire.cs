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


    // TODO: MOVE THESE TO A SCRIPTABLE OBJECT
    private List<Spell> _allSpells;
    private List<Spell> _learnedSpells;

    /// <summary>
    /// List of spells that the player currently has available within the level
    /// </summary>
    [SerializeField]
    public List<Spell> activeSpells;

    private List<Spell> _instantiatedActiveSpells;

    /// <summary>
    /// The currently prepared spell of the player. If this is null, the player has no spell prepared
    /// </summary>
    private Spell _preparedSpell;

    /// <summary>
    /// Current sequence of inputs the player has entered
    /// </summary>
    private List<Direction> _playerInputSequence;
    

    /// <summary>
    /// The spell codes actively being displayed
    /// </summary>
    private List<string> _grimoireText;

    /// <summary>
    /// The current player in the scene
    /// </summary>
    private Player _player;
    // Displayed text in the UI
    [SerializeField]
    private TextMeshProUGUI grimoireTextUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // get all spells from spell class; sort into .learned and .active
        _playerInputSequence = new List<Direction>();
        _instantiatedActiveSpells = new List<Spell>();
        _preparedSpell = null;
        _player = FindAnyObjectByType<Player>();
       //GetSpells();
        GetGrimoireText();

        foreach (Spell spell in activeSpells)
        {
            // Need to instantiate spells in order to track their timers
            _instantiatedActiveSpells.Add(Instantiate(spell, transform.position, Quaternion.identity));
        }
    }

    /// <summary>
    /// Gets all spells to store them; filters by learned and active
    /// MAY BE DEPRECATED
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
        _playerInputSequence.Add(input);
        CheckForMatches();
    }

    public void CheckForMatches()
    {
        //Debug.Log("ActiveSpells size:" + instantiatedActiveSpells.Count);
        //Debug.Log(instantiatedActiveSpells[0]);
        foreach(Spell spell in _instantiatedActiveSpells)
        {
            if (IsMatch(spell))
            { 
                // Do we want to block additional inputs until the spell is cast (do this through an event)
                _preparedSpell = spell;
                ResetInputSequence();
                break; // Exit the loop after preparing a spell
            }
        }
    }

    private bool IsMatch(Spell spell)
    {
        int codeLength = spell.spellCode.Count;
        if (_playerInputSequence.Count < codeLength || spell.OnCooldown)
        {
            return false;
        }

        for (int i = 0; i < codeLength; i++)
        {
            // Compare the last 'codeLength' inputs in playerInputSequence with the spell's spellCode
            // Quit out early after any mismatches
            if (_playerInputSequence[i] != spell.spellCode[i])
            {
                ResetInputSequence();
                return false;
            }
        }

        Debug.Log("INPUT CORRECT, SPELL PREPARED");
        return true;
    }

    public void ResetInputSequence()
    {
        _playerInputSequence.Clear();
    }

    public void OnCast(InputAction.CallbackContext context)
    {
        if (_preparedSpell != null)
        {
            _preparedSpell.Cast(_player);
            _preparedSpell = null;
        }
    }

    // ***** INPUT METHODS *****
    #region Input Methods
    public void OnPrepareSpellUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ProcessInput(Direction.Up);
        }
    }

    public void OnPrepareSpellDown(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            ProcessInput(Direction.Down);
        }
    }

    public void OnPrepareSpellLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ProcessInput(Direction.Left);
        }
    }

    public void OnPrepareSpellRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ProcessInput(Direction.Right);
        }
    }
    #endregion
    // *************************
}
