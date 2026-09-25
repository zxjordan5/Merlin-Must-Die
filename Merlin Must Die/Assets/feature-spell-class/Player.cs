using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] Vector2 aimDirRaw;
    [SerializeField] Spell testingSpell;

    public Vector2 AimDirNorm
    {
        get { return Vector2.Normalize(aimDirRaw); }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("casting");
        testingSpell.Cast(this);
    }
}
