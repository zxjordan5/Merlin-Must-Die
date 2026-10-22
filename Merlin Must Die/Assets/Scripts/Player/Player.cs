using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private Vector2 _aimDirRaw;

    /// <summary>
    /// Returns a normalzied vector of the current aim direction
    /// </summary>
    public Vector2 AimDirNorm
    {
        get => Vector2.Normalize(_aimDirRaw);
        private set => _aimDirRaw = value;
    }

    private Rigidbody2D _rb; 
    public Rigidbody2D RB
    {
        get => _rb;
        private set => _rb = value;
    }
    
    //***** HEALTH AND DAMAGE PROPERTIES *****//
    #region HealthAndDamageProperties
    /// <summary>
    /// Health of Morgana. Serializable only for testing.
    /// </summary>
    [SerializeField]
    private int health = 3;

    /// <summary>
    /// How much time the player is invincible for after being hit.
    /// </summary>
    [SerializeField]
    private float invincibilityWindow = 1.0f;

    /// <summary>
    /// How much time has passed since the player has taken damage.
    /// </summary>
    private float _timeSinceDamageTaken;

    /// <summary>
    /// Is the player currently invincible. True if timeSinceDamageTaken < invincibility, false otherwise.
    /// </summary>
    private bool _isInvincible;
    #endregion


    //***** ANGLE PROPERTIES *****
    #region AngleProperties
    /// <summary>
    /// Returns the players aiming angle in degrees between 0 and 360.
    /// </summary>
    public float AimAngle
    {
        get { return GetAngleInDegFromVec(_aimDirRaw); }
    } 
    /// <summary>
    /// Returns the players movement angle in degrees between 0 and 360.
    /// </summary>
    public float MoveAngle
    {
        get{ return GetAngleInDegFromVec(_targetDirection); }
    }


    /// <summary>
    /// Returns the angle in degrees a vector would be pointing in.
    /// 
    /// TODO? :
    /// Make a global utility class for methods like this that are used in other files as well such as the dash particles
    /// </summary>
    private float GetAngleInDegFromVec(Vector2 vector)
    {
        float angle = (float)(Math.Atan2(vector.y, vector.x) * 57.295); //convert from radians to degrees
        //Converts range from -180 to 180 to 0 to 360.
        if (angle < 0)
        {
            angle = 180 + (180 + angle);
        }
        return angle;
    }

    /// <summary>
    /// Returns the direction the player is aiming as an enumerator
    /// </summary>
    public Direction AimDirection
    {
        get
        {
            float aimAngle = AimAngle;
            if (aimAngle < 45 || aimAngle > 315)
            {
                return Direction.Right;
            }
            else if (45 < aimAngle && aimAngle < 135)
            {
                return Direction.Up;
            }
            else if (135 < aimAngle && aimAngle < 225)
            {
                return Direction.Left;
            }
            else
            {
                return Direction.Down;
            }
        }
    }

    /// <summary>
    /// Returns the direction the player is moving as an enumerator
    /// </summary>
    public Direction MoveDirection
    {
        get
        {
            float moveAngle = MoveAngle;
            if (moveAngle < 45 || moveAngle > 315)
            {
                return Direction.Right;
            }
            else if (45 < moveAngle && moveAngle < 135)
            {
                return Direction.Up;
            }
            else if (135 < moveAngle && moveAngle < 225)
            {
                return Direction.Left;
            }
            else
            {
                return Direction.Down;
            }
        }
    }
    #endregion

    public enum Direction {Up = 0, Right = 1, Down = 2, Left = 3}

    private GameObject _crosshair;
    private Camera _cam;
    // We can remove "serialize" once we've settled on a speed
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private Vector2 _targetDirection;
    [SerializeField] private Vector2 _targetVelocity;

    private float _speedMult = 1f; //Should be reset to 1 always

    private Vector3 _mousePos;
    private Vector3 _gpPos;
    private bool _kbMoving;
    private bool _gpMoving;
    private bool _kbAiming;
    private bool _gpAiming;
    private bool _dashing; //Tracks if the player is dashing
    private Vector2 _dashTarget; //Target to dash towards


    private Animator _animator;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _crosshair = transform.Find("crosshair").gameObject;
        _crosshair.SetActive(true);
        _crosshair.transform.position = _rb.transform.position + new Vector3(0, 2, 0);
        _cam = Camera.main;
    }

    //Contains animation logic
    void Update()
    {
        // Update animator controller with the players aim and movement direction, and if they are moving
        _animator.SetBool("Moving", _targetVelocity != Vector2.zero);
        _animator.SetInteger("AimDirection", (int)AimDirection);
        _animator.SetInteger("MoveDirection", (int)MoveDirection);

        #region InvincibilityTimer
        if (_isInvincible)
        {
            _timeSinceDamageTaken += Time.deltaTime;
            if (_timeSinceDamageTaken > invincibilityWindow)
            {
                _timeSinceDamageTaken = 0;
                _isInvincible = false;
            }
        }
        #endregion
    }

    //Contains movement logic
    void FixedUpdate()
    {
        // Reset the target velocity so the player slows down
        _targetVelocity = new Vector2(0,0);

        // Dashing movement
        // Move the player towards the dash target, which is the crosshair
        // Add the players current position to the crosshair so the target continually gets further.
        if (_dashing)
        {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                (Vector2)_rb.transform.position + _dashTarget,
                maxSpeed * _speedMult * Time.deltaTime);
        }
       
        // GP movement
        if (_gpMoving)
        {   
            _targetDirection = Gamepad.current.leftStick.value;
            _targetVelocity = _targetDirection * maxSpeed;
            _rb.AddForce((_targetVelocity - _rb.linearVelocity)*2);

            if (!_gpAiming) {
                _gpPos = new Vector3(Gamepad.current.leftStick.value.x, Gamepad.current.leftStick.value.y, 0).normalized * 2;
            _crosshair.transform.position = _rb.transform.position + _gpPos;
            _aimDirRaw = _crosshair.transform.position - _rb.transform.position;
            }
        } 
        // Kb/M movement
        else if (_kbMoving)
        {
            _targetDirection = (Mouse.current.position.ReadValue() - new Vector2(Screen.width / 2, Screen.height / 2)).normalized * 2;
            _targetVelocity = _targetDirection * maxSpeed;
            _rb.AddForce((_targetVelocity - _rb.linearVelocity) * 2);
        }

        // GP aiming
        if (_gpAiming)
        {
            _gpPos = new Vector3(Gamepad.current.rightStick.value.x, Gamepad.current.rightStick.value.y, 0).normalized * 2;
            _crosshair.transform.position = _rb.transform.position + _gpPos;
            _aimDirRaw = _crosshair.transform.position - _rb.transform.position;
        }
        // Kb/M aiming
        else if (_kbAiming)
        {
            _mousePos = (Mouse.current.position.ReadValue() - new Vector2(Screen.width / 2, Screen.height / 2)).normalized * 2;
            _crosshair.transform.position = _rb.transform.position + _mousePos;
            _aimDirRaw = _crosshair.transform.position - _rb.transform.position;
        }

        // Slows player movement when not actively moving
        if (!_kbMoving && !_gpMoving)
        {
            _rb.linearVelocity *= new Vector2(0.8f,0.8f);
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

    // ***** ANIMATION METHODS *****

    
}