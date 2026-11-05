using UnityEngine;
using TMPro;


public class InteractionIndicator : MonoBehaviour
{
    //Player used to find interactions
    [SerializeField] private Player _player;
    //Object used to show the interactions
    [SerializeField] private GameObject _interactionIndicator;
    private TMP_Text _interactionText;

    private void Awake()
    {
        if (_player == null)
            _player = FindAnyObjectByType<Player>();
        if (_interactionIndicator == null)
            _interactionIndicator = GameObject.Find("InteractionIndicator");

        _interactionText = _interactionIndicator.GetComponentInChildren<TMP_Text>();

    }

    /// <summary>
    /// Check if the player is near in interactable, if so enable the indicator and move it below the player
    /// </summary>
    private void Update()
    {
        bool visible = _player.CanInteract;

        _interactionIndicator.SetActive(visible);

        if (visible)
        {
            _interactionIndicator.transform.position = _player.FindInteractable().transform.position;
            _interactionIndicator.transform.Translate(new Vector2(0.0f, -0.75f));
        }

        //If the player has dialogue open, do not show the interaction
        if (_player.ReadingDialogue)
        {
            //Debug.Log("reading dialogue");
            _interactionIndicator.SetActive(false);
        }

        //Update the interaction prompt to match the control scheme
        if(InputManager.CurrentDevice == InputManager.ControlScheme.Controller)
        {
            _interactionText.text = "[LB] Interact";
        }
        else
        {
            _interactionText.text = "[F] Interact";
        }
    }
}
