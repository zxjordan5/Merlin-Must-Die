using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A button / input sequence in a menu with one action associated
/// Has fields to assign arrows and input labels to each
/// </summary>
public class MenuOption : MonoBehaviour
{
    /// <summary>
    /// Which menu action the option should activate
    /// </summary>
    [SerializeField] private MenuAction _action;
    public MenuAction Action { get { return _action; } }


    [SerializeField] private Image[] _inputArrows;
    public Image[] InputArrows { get { return _inputArrows; } }


    [SerializeField] private TMP_Text[] _inputLabels;
    public TMP_Text[] InputLabels { get { return _inputLabels; } }

    /// <summary>
    /// Series of input required to activate the action
    /// </summary>
    [SerializeField] protected List<Globals.Direction> _inputSequence = new List<Globals.Direction>();
    public List<Globals.Direction> InputSequence { get => _inputSequence; }

    private void Awake()
    {
        UpdateLabels();

        //Checks if the control scheme has changed once a second instead of every frame
        InvokeRepeating("UpdateLabels", 1f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Dims the arrow when pressed
    /// </summary>
    public void ArrowPressed(int index)
    {
        _inputArrows[index].color = new Color(0.5f, 0.5f, 0.5f);
    }

    public void ArrowReleased(int index)
    {
        _inputArrows[index].color = Color.white;
    }

    /// <summary>
    /// Loops through every arrow and resets the color using ArrowReleased
    /// </summary>
    public void ResetArrows()
    {
        for (int i = 0; i < _inputArrows.Length; i++)
        {
            ArrowReleased(i);
        }
    }

    /// <summary>
    /// Changes the text label above the icon to match the control scheme
    /// 
    /// TODO: Probably use icons and handle this logic better
    /// </summary>
    private void UpdateLabels()
    {
        for(int i = 0; i < _inputSequence.Count(); i++)
        {
            string newLabel = "?";
            switch (_inputSequence[i])
            {
                case(Globals.Direction.Up):
                    if (InputManager.CurrentDevice == InputManager.ControlScheme.Controller)
                        newLabel = "Y"; //Controller
                    else
                        newLabel = "W"; //Keyboard
                    break;
                case (Globals.Direction.Right):
                    if (InputManager.CurrentDevice == InputManager.ControlScheme.Controller)
                        newLabel = "B"; //Controller
                    else
                        newLabel = "D"; //Keyboard
                    break;
                case (Globals.Direction.Down):
                    if (InputManager.CurrentDevice == InputManager.ControlScheme.Controller)
                        newLabel = "A"; //Controller
                    else
                        newLabel = "S"; //Keyboard
                    break;
                case (Globals.Direction.Left):
                    if (InputManager.CurrentDevice == InputManager.ControlScheme.Controller)
                        newLabel = "X"; //Controller
                    else
                        newLabel = "A"; //Keyboard
                    break;
            }
            _inputLabels[i].text = newLabel;
        }
    }
}
