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
    // While spells class is still being created, using 'int' as placeholder types
    private List<int> allSpells;
    private List<int> learnedSpells;
    private List<int> activeSpells;

    // The input from the player of the spell they're trying to cast
    private List<KeyCode> playerInput;

    // The spell codes actively being displayed
    private List<string> grimoireText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get all spells from spell class; sort into .learned and .active
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
        for (int i = 0; i < allSpells.Count; i++)
        {
            /// if (allSpells[i].learned)
            /// {
            ///     learnedSpells.Add(allSpells[i]);
            /// }
            /// if (allSpells[i].active)
            /// {
            ///     activeSpells.Add(allSpells[i]);
            /// }
        }
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


    // ***** INPUT METHODS *****
    #region Input Methods
    public void OnPrepareSpellUp(InputAction.CallbackContext context)
    {
        Debug.Log("Preparing Spell Up");
    }

    public void OnPrepareSpellDown(InputAction.CallbackContext context)
    {
        Debug.Log("Preparing Spell Down");
    }

    public void OnPrepareSpellLeft(InputAction.CallbackContext context)
    {
        Debug.Log("Preparing Spell Left");
    }

    public void OnPrepareSpellRight(InputAction.CallbackContext context)
    {
        Debug.Log("Preparing Spell Right");
    }
    #endregion
    // *************************
}
