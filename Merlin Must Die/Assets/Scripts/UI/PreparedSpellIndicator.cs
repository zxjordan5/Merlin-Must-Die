using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows the control needed to cast a spell/menu action
/// </summary>
public class PreparedSpellIndicator : MonoBehaviour
{
    private bool _active = false;
    public bool Active {  get { return _active; } 
        set 
        {  
            _active = value;
            _background.enabled = value;
            _text.enabled = value;
        }  
    }

    private float _pulseTimer = 0;
    private int _increasing = 1;

    private Vector3 _baseSize;

    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        _baseSize = transform.localScale;

        //Updates the label based on the control scheme every second
        InvokeRepeating("UpdateLabel", 1f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// If the indicator is active, have a small pulse effect to show it
    /// </summary>
    public void Update()
    {
        if (_active)
        {
            if(_pulseTimer < -1)
            {
                _increasing = 1;
            }
            if(_pulseTimer > 1)
            {
                _increasing = -1;
            }

            _pulseTimer += Time.deltaTime * _increasing * 3;
            transform.localScale = _baseSize + new Vector3(_pulseTimer / 8, _pulseTimer / 8, _pulseTimer / 8);
        }
    }

    /// <summary>
    /// Update the label based on control scheme
    /// </summary>
    private void UpdateLabel()
    {
        switch (InputManager.CurrentDevice)
        {
            case InputManager.ControlScheme.Keyboard:
                _text.text = "<b>Space</b> to cast";
                break;
            case InputManager.ControlScheme.Controller:
                _text.text = "<b>LT</b> to cast";
                break;
        }
    }
}
