using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Movement/Aiming Variables
    private Rigidbody2D rb; // Reference to the player's rigid body
    private GameObject crosshair;
    private Camera cam;
    // We can remove "serialize" once we've settled on a speed
    [SerializeField] private float maxSpeed = 2f;
    private Vector3 mousePos;
    private Vector3 gpPos;
    private bool kbMoving;
    private bool gpMoving;
    private bool kbAiming;
    private bool gpAiming;


    void Awake()
    {        
        rb = GetComponent<Rigidbody2D>();
        crosshair = transform.Find("crosshair").gameObject;
        crosshair.SetActive(true);
        crosshair.transform.position = rb.transform.position + new Vector3(0, 2, 0);
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Kb/M movement
        if (kbMoving) {
            rb.transform.position = Vector2.MoveTowards(
                rb.transform.position,
                cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()),
                maxSpeed * Time.deltaTime);
        }
        // GP movement
        else if (gpMoving) {
            rb.transform.position = Vector2.MoveTowards(
                rb.transform.position,
                new Vector2(rb.transform.position.x + Gamepad.current.leftStick.x.value, rb.transform.position.y + Gamepad.current.leftStick.y.value),
                maxSpeed * Time.deltaTime);
        }

        // Kb/M aiming
        if (kbAiming) {
            mousePos = (Mouse.current.position.ReadValue() - new Vector2(Screen.width/2, Screen.height/2)).normalized * 2;
            crosshair.transform.position = rb.transform.position + mousePos;
        }
        // GP aiming
        else if (gpAiming) {
            gpPos = new Vector3(Gamepad.current.rightStick.value.x, Gamepad.current.rightStick.value.y, 0).normalized * 2;
            crosshair.transform.position = rb.transform.position + gpPos;
        }

        // Keeps the camera centered on the player
        cam.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y, rb.transform.position.z - 10);
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
        //change the color of the crosshair when actively aiming
    }
    public void OnGP_Aim(InputAction.CallbackContext context)
    {
        gpAiming = context.started || context.performed;
        //change the color of the crosshair when actively aiming
    }
    #endregion
}
