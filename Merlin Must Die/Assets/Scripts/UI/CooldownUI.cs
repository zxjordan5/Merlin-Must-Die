using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CooldownUI : MonoBehaviour
{
    /// <summary>
    /// List of every spell cooldown bar
    /// </summary>
    private List<ProgressBar> _cooldownBars = new List<ProgressBar>();

    int m_Version = 0;
 
    /// <summary>
    /// Connect the panel renderer to update on UI calls
    /// 
    /// TODO:
    /// Understand what calls ui reloads...
    /// </summary>
    private void OnEnable()
    {
        PanelRenderer _root = GetComponent<PanelRenderer>();

        _root.RegisterUIReloadCallback(OnUIReload);

    }

    /// <summary>
    /// Add all changes to CooldownUI here, using rootElement to find other visual elements.
    /// </summary>
    public void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        if (version == m_Version)
            return; // UI already up-to-date, no need to reload

        m_Version = version;

        
        //Add each progress bar
        VisualElement container = rootElement.Q("Container");
        if(container.childCount != _cooldownBars.Count)
        {
            for (int i = 0; i < _cooldownBars.Count; i++)
            {
                container.Add(_cooldownBars[i]);
            }
        }
    }

    /// <summary>
    /// Creates a progress bar UI element using the spells information
    /// </summary>
    /// <param name="spell">Spell name and cooldown will be connected to the progress bar</param>
    public void AddSpell(Spell spell)
    {
        ProgressBar spellBar = new ProgressBar
        {
            title = spell.name,
            lowValue = 0f,
            highValue = 100f,
            value = 0f,
        };

        _cooldownBars.Add(spellBar);
    }
}