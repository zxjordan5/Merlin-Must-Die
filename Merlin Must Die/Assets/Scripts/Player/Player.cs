using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System.IO;

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
    private float _speedMult = 1f; //Should be reset to 1 always

    private Vector3 _mousePos;
    private Vector3 _gpPos;
    private bool _kbMoving;
    private bool _gpMoving;
    private bool _kbAiming;
    private bool _gpAiming;
    private bool _dashing; //Tracks if the player is dashing
    private Vector2 _dashTarget; //Target to dash towards


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

        // Dashing movement
        // Move towards a fixed location determined at the moment the dash spell is cast
        if (_dashing)
        {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _dashTarget,
                maxSpeed * _speedMult * Time.deltaTime);
        }

        // Kb/M movement
        if (_kbMoving)
        {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()),
                maxSpeed * _speedMult * Time.deltaTime);
        }
        // GP movement
        else if (_gpMoving)
        {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                new Vector2(_rb.transform.position.x + Gamepad.current.leftStick.x.value, _rb.transform.position.y + Gamepad.current.leftStick.y.value),
                maxSpeed * _speedMult * Time.deltaTime);
        }
            


        // Kb/M aiming
        if (_kbAiming) {
            _mousePos = (Mouse.current.position.ReadValue() - new Vector2(Screen.width/2, Screen.height/2)).normalized * 2;
            _crosshair.transform.position = _rb.transform.position + _mousePos;
            aimDirRaw = _crosshair.transform.position - _rb.transform.position;
        }
        // GP aiming
        else if (_gpAiming) {
            _gpPos = new Vector3(Gamepad.current.rightStick.value.x, Gamepad.current.rightStick.value.y, 0).normalized * 2;
            _crosshair.transform.position = _rb.transform.position + _gpPos;
            aimDirRaw = _crosshair.transform.position - _rb.transform.position;
        }

        // Keeps the camera centered on the player
        _cam.transform.position = new Vector3(_rb.transform.position.x, _rb.transform.position.y, _rb.transform.position.z - 10);
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
    public void Dash(float dashSpeed, float dashTime)
    {
        _speedMult = dashSpeed;
        _dashTarget = _crosshair.transform.localPosition;
        _dashTarget *= 20;
        Debug.Log(_dashTarget + " + " + AimDirNorm);
        if (!_dashing)
        {
            StartCoroutine(DashTimer(dashTime));
        }
    }
    IEnumerator DashTimer(float dashTime)
    {
        _dashing = true;
        yield return new WaitForSeconds(dashTime);
        _dashing = false;
        _speedMult = 1;
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
