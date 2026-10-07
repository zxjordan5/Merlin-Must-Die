using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CooldownUI : MonoBehaviour
{
    [SerializeField]
    VerticalLayoutGroup spellbarArea;

    [SerializeField]
    GameObject spellbar;

    private List<Image> _currentSpellbars = new List<Image>();



    /// <summary>
    /// Creates a progress bar UI element using the spells information
    /// </summary>
    /// <param name="spell">Spell name and cooldown will be connected to the progress bar</param>
    public void AddSpell(Spell spell)
    {
        GameObject newBar = Instantiate(spellbar, spellbarArea.transform);
        newBar.GetComponentInChildren<TMP_Text>().text = spell.SpellName;
        newBar.name = spell.SpellName + " bar";

        _currentSpellbars.Add(newBar.GetComponent<Image>());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spellIndex"></param>
    /// <param name="cooldownPercentage"></param>
    public void UpdateSpell(int spellIndex, float cooldownPercentage)
    {
        _currentSpellbars[spellIndex].fillAmount = cooldownPercentage / 100;
    }
}