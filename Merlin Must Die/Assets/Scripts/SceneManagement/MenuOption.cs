using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class MenuOption : MonoBehaviour
{

    [SerializeField] private MenuAction _action;
    public MenuAction Action { get { return _action; } }


    [SerializeField] private Image[] _inputArrows;
    public Image[] InputArrows { get { return _inputArrows; } }

    [SerializeField] protected List<Globals.Direction> _inputSequence = new List<Globals.Direction>();
    public List<Globals.Direction> InputSequence { get => _inputSequence; }


    public void ArrowPressed(int index)
    {
        _inputArrows[index].color = new Color(0.5f, 0.5f, 0.5f);
    }

    public void ArrowReleased(int index)
    {
        _inputArrows[index].color = Color.white;
    }

    public void ResetArrows()
    {
        for (int i = 0; i < _inputArrows.Length; i++)
        {
            ArrowReleased(i);
        }
    }
}
