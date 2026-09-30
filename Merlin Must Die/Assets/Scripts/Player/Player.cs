using UnityEngine;
using UnityEngine.InputSystem;

//Another testing class for launching the projectile
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
        testingSpell.Cast(this);
    }

}
