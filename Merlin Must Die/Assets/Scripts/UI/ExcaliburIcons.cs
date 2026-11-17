using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Determines the control scheme the user is using and switches out the excalibur
/// ability icon to match the required input button for each ability
/// </summary>
public class ExcaliburIcons : MonoBehaviour
{
    // The sprite - keyboard/mouse and controller options
    [SerializeField] private Sprite kbIcon;
    [SerializeField] private Sprite controllerIcon;

    private Sprite currentIcon;

    // Reference to the Excalibur object to get its charge to tint currentSprite
    [SerializeField] private Excalibur excaliburRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.ControlScheme currentControls = InputManager.CurrentDevice;

        // this is a backup just in case something fails in the next check
        currentIcon = kbIcon;

        if (currentControls == InputManager.ControlScheme.Keyboard)
        {
            currentIcon = kbIcon;
        }
        else
        {
            currentIcon = controllerIcon;
        }

        this.GetComponent<Image>().sprite = currentIcon;
    }

    // Update is called once per frame
    void Update()
    {
        // tints currentIcon while excalibur isn't charged
        if (!excaliburRef.IsCharged)
        {
            this.GetComponent<Image>().color = new Color(118 / 255f, 66 / 255f, 138 / 255f, 145 / 255f);
        }
        else
        {
            this.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
