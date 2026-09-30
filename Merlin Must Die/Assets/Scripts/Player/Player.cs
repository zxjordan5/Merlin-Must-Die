using UnityEngine;
using UnityEngine.InputSystem;

//Another testing class for launching the projectile

public class Player : MonoBehaviour
{
    [SerializeField] Vector2 aimDirRaw;
    [SerializeField] Spell testingSpell;

    // Movement/Aiming Variables
    private Rigidbody2D rb; // Reference to the player's rigid body
    private GameObject crosshair;
    // We can remove "serialize" once we've settled on a speed
    [SerializeField] private float maxSpeed = 2f;
    private Vector3 mousePos;
    private bool kbMoving;
    private bool gpMoving;
    private bool kbAiming;
    private bool gpAiming;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        crosshair = transform.Find("crosshair").gameObject;
    }

    private void FixedUpdate()
    {
        // Kb/M movement
        if (kbMoving)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()),
                maxSpeed * Time.deltaTime);
        }
        // GP movement
        else if (gpMoving)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(transform.position.x + Gamepad.current.leftStick.x.value, transform.position.y + Gamepad.current.leftStick.y.value),
                maxSpeed * Time.deltaTime);
        }
        // Kb/M aiming
        if (kbAiming)
        {
            // Centered around the middle of the screen, not the character... NEED TO FIX
            mousePos = rb.transform.position + Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).normalized * 3;

            crosshair.transform.position = mousePos;
        }
        else if (gpAiming)
        {
            crosshair.transform.position = transform.position + new Vector3(Gamepad.current.rightStick.value.x, Gamepad.current.rightStick.value.y, 0).normalized * 2;
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
        kbMoving = context.started || context.performed;
    }
    public void OnGP_Move(InputAction.CallbackContext context)
    {
        gpMoving = context.started || context.performed;
    }
    #endregion

    // ***** AIMING METHODS *****
    #region Aiming
    public void OnKBM_Aim(InputAction.CallbackContext context)
    {
        kbAiming = context.started || context.performed;
        crosshair.SetActive(context.started || context.performed);
    }
    public void OnGP_Aim(InputAction.CallbackContext context)
    {
        gpAiming = context.started || context.performed;
        Debug.Log(transform.position.y);
    }
    #endregion
}
