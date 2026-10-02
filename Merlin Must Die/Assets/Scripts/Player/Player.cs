using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

//Another testing class for launching the projectile
public class Player : MonoBehaviour
{
    [SerializeField] Vector2 aimDirRaw;
    
    public Vector2 AimDirNorm
    {
        get => Vector2.Normalize(aimDirRaw);
        private set => aimDirRaw = value;
    }
    
    
    // Movement/Aiming Variables
    private Rigidbody2D _rb; // Reference to the player's rigid body
    private GameObject _crosshair;
    private Camera _cam;
    // We can remove "serialize" once we've settled on a speed
    [SerializeField] private float maxSpeed = 2f;
    private Vector3 _mousePos;
    private Vector3 _prevMousePos;
    private Vector3 _gpPos;
    private bool _kbMoving;
    private bool _gpMoving;
    private bool _kbAiming;
    private bool _gpAiming;


    void Awake()
    {        
        _rb = GetComponent<Rigidbody2D>();
        _crosshair = transform.Find("crosshair").gameObject;
        _crosshair.SetActive(true);
        _crosshair.transform.position = _rb.transform.position + new Vector3(0, 2, 0);
        _cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Keeps the camera centered on the player
        _cam.transform.position = new Vector3(_rb.transform.position.x, _rb.transform.position.y, _rb.transform.position.z - 10);

        // Kb/M movement
        if (_kbMoving) {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()),
                maxSpeed * Time.deltaTime);
        }
        // GP movement
        else if (_gpMoving) {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                new Vector2(_rb.transform.position.x + Gamepad.current.leftStick.x.value, _rb.transform.position.y + Gamepad.current.leftStick.y.value),
                maxSpeed * Time.deltaTime);
        }

        // GP aiming
        if (_gpAiming)
        {
            _gpPos = new Vector3(Gamepad.current.rightStick.value.x, Gamepad.current.rightStick.value.y, 0).normalized * 2;
            _crosshair.transform.position = _rb.transform.position + _gpPos;
            aimDirRaw = _crosshair.transform.position - _rb.transform.position;
        }
        // Kb/M aiming
        if (_kbAiming) {
            _mousePos = (Mouse.current.position.ReadValue() - new Vector2(Screen.width/2, Screen.height/2)).normalized * 2;
            if (_mousePos == _prevMousePos)
            {
                return;
            }
            _crosshair.transform.position = _rb.transform.position + _mousePos;
            aimDirRaw = _crosshair.transform.position - _rb.transform.position;
            _prevMousePos = _mousePos;
        }
    }

    // ***** MOVEMENT METHODS *****
    #region Movement
    public void OnKBM_AutoMove(InputAction.CallbackContext context)
    {
        _kbMoving = context.started || context.performed;
    }
    public void OnGP_Move(InputAction.CallbackContext context)
    {
        _gpMoving = context.started || context.performed;
    }
    #endregion

    // ***** AIMING METHODS *****
    #region Aiming
    public void OnKBM_Aim(InputAction.CallbackContext context)
    {
        _kbAiming = context.started || context.performed;
        //change the color of the crosshair when actively aiming
    }
    public void OnGP_Aim(InputAction.CallbackContext context)
    {
        _gpAiming = context.started || context.performed;
        //change the color of the crosshair when actively aiming
    }
    #endregion
}
