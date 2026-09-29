using UnityEngine;
using UnityEngine.InputSystem;

//Another testing class for launching the projectile

public class Player : MonoBehaviour
{
    [SerializeField] Vector2 aimDirRaw;
    [SerializeField] Spell testingSpell;

    // Movement Variables
    private Rigidbody2D rb; // Reference to the player's rigid body
    // We can remove "serialize" once we've settled on a speed
    [SerializeField] private float maxSpeed = 2f; 
    private bool holding;
    private bool moving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Keyboard/Mouse movement
        if (holding)
        {
            rb.transform.position = Vector2.MoveTowards(
                transform.position,
                Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()),
                maxSpeed * Time.deltaTime);
        }
        // Controller movement
        else if (moving)
        {
            rb.transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(transform.position.x + Gamepad.current.leftStick.x.value, transform.position.y + Gamepad.current.leftStick.y.value),
                maxSpeed * Time.deltaTime);
        }
    }

    public Vector2 AimDirNorm
    {
        get { return Vector2.Normalize(aimDirRaw); }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        testingSpell.Cast(this);
    }

    // ***** MOVEMENT METHODS *****
    #region Movement
    public void OnKBM_AutoMove(InputAction.CallbackContext context)
    {
        holding = context.started || context.performed;
    }
    public void OnGPMove(InputAction.CallbackContext context)
    {
        moving = context.started || context.performed;
    }
    #endregion
}
