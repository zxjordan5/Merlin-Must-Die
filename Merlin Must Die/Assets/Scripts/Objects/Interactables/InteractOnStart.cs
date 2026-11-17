using UnityEngine;
using TMPro;

public class InteractOnStart : MonoBehaviour
{
    [SerializeField] protected GameObject _interactionObj;

    public void Awake() {

        foreach (Interaction interaction in _interactionObj.GetComponents<Interaction>())
        {
            interaction.Behaviour();
        }
    }
}
