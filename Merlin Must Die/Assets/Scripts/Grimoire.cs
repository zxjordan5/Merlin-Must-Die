/// Grimoire Class
/// Contains the full grimoire and a list of active spells in the level
/// Reads player input and finds the matching spell code
/// Shows cooldowns for spells
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
        // get all spells from spell class
        allSpells = GetSpells();

        // learnedSpells = spells that are .learned

        // activeSpells = spells that are .active (ones being used in this level)
    }

    // Update is called once per frame
    void Update()
    {
        // Currently only reads for keyboard input; need to do a little more research
        // on new input system

        // Player can only cast using 1 key per frame
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerInput.Add(KeyCode.W);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            playerInput.Add(KeyCode.A);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            playerInput.Add(KeyCode.S);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            playerInput.Add(KeyCode.D);
        }
    }

    // int is currently a placeholder type
    /// <summary>
    /// Gets all spells to store them; filters by learned and active
    /// </summary>
    /// <returns></returns>
    List<int> GetSpells()
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

        return new List<int> { };
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
}
