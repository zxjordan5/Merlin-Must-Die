using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class Player : MonoBehaviour
{

    #region Variables
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
    private int _health = 3;

    /// <summary>
    /// Max health of Morgana
    /// </summary>
    [SerializeField]
    private int maxHealth = 3;

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

    /// <summary>
    /// Histop component on the character
    /// </summary>
    private HitStop _hitStop;
    #endregion

    public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3 }
    //***** STATE PROPERTIES *****
    #region StateProperties
    /// <summary>
    /// Returns the players aiming angle in degrees between 0 and 360.
    /// </summary>
    public float AimAngle
    {
        get { return Globals.GetAngleInDegFromVec(_aimDirRaw); }
    } 
    /// <summary>
    /// Returns the players movement angle in degrees between 0 and 360.
    /// </summary>
    public float MoveAngle
    {
        get{ return Globals.GetAngleInDegFromVec(_targetDirection); }
    }

    public bool Moving
    {
        get { return _targetVelocity != Vector2.zero; }
    }

    [SerializeField] private bool _hasGear = true;
    public bool HasGear { 
        get { return _hasGear; }
        set { _hasGear = value; } 
    }

    [SerializeField] private bool _canMove = true;
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }

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

    //***** INTERACTABLE PROPERTIES *****
    #region InteractionProperties
    CircleCollider2D _interactionCollider;
    private List<Collider2D> _interactResults = new List<Collider2D>();

    /// <summary>
    /// True if an interactable object is found, false otherwise
    /// </summary>
    public bool CanInteract
    {
        get { return FindInteractable() != null; }
    }

    /// <summary>
    /// Set in ShowDialogue script
    /// </summary>
    public bool ReadingDialogue
    {
        get; set;
    } = false;
    #endregion

    private Camera _cam;
    [SerializeField] private float maxSpeed = 5.0f;
    private Vector2 _targetDirection;
    private Vector2 _targetVelocity;

    public float Velocity
    {
        get { return RB.linearVelocity.magnitude; }
    }

    public float MaxSpeed
    {
        get => maxSpeed;
        set => maxSpeed = value;
    }

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

    //********** CROSSHAIR PROPERTIES **********//
    #region Crosshair
    /// <summary>
    /// Tracks which logic the crosshair should use to calculate it's position
    /// </summary>
    public enum CrosshairState { Default, MarkTrajectory }
    private CrosshairState _crosshairState = CrosshairState.Default;
    private GameObject _crosshair;
    private SpriteRenderer _chSpriteRenderer;

    [Header("Crosshair Setup")]
    [SerializeField] private Sprite _defaultCrosshairSprite;
    [SerializeField] private Sprite _landingCrosshairSprite;

    /// <summary>
    /// Used when marking trajectory for projecitle spells to hold information on where they will land
    /// </summary>
    private float _chBaseDistance = 0;
    /// <summary>
    /// The upper bound of the prepared spell travel distance
    /// </summary>
    private float _chMaxDistance = 0;
    /// <summary>
    /// Should the crosshair be affected by the players velocity
    /// </summary>
    private bool _chUsePlayerVelocity = false;
    #endregion


    #endregion 
    //*** END VARIABLES ***

    void Awake()
    {
        _health = maxHealth;
        _hitStop = GetComponent<HitStop>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _interactionCollider = transform.Find("InteractionCollider").GetComponent<CircleCollider2D>();
        _crosshair = transform.Find("Crosshair").gameObject;
        _chSpriteRenderer = _crosshair.GetComponent<SpriteRenderer>();
        _crosshair.SetActive(true);
        _crosshair.transform.position = _rb.transform.position + new Vector3(0, 2, 0);
        _cam = Camera.main;
    }

    //Contains animation logic
    void Update()
    {
        // Update animator controller with the players aim and movement direction, and if they are moving
        _animator.SetBool("Moving", Moving);
        _animator.SetBool("HasGear", HasGear);
        _animator.SetFloat("AimDirection", (int)AimDirection);
        _animator.SetFloat("MoveDirection", (int)MoveDirection);

        #region InvincibilityTimer
        if (_isInvincible)
        {
            _timeSinceDamageTaken += Time.deltaTime;

            if (_timeSinceDamageTaken > invincibilityWindow)
            {
                ResetInvincibility();
            }
        }
        #endregion
    }

    //Contains movement logic
    void FixedUpdate()
    {
        // Reset the target velocity so the player slows down
        _targetVelocity = new Vector2(0,0);

        // GP movement
        if (_canMove)
        {
            CalculateMovement();      
        }

        // GP aiming
        CalculateAim();
        // Calculate Crosshair Position
        CalculateCrosshair();

        // Slows player movement when not actively moving
        if (!_kbMoving && !_gpMoving && !_dashing)
        {
            _rb.linearVelocity *= new Vector2(0.8f,0.8f);
        }

        // Keeps the camera centered on the player
        _cam.transform.position = new Vector3(_rb.transform.position.x, _rb.transform.position.y, _rb.transform.position.z - 10);
    }

    /// <summary>
    /// Called every frame in fixed update, moves rigidbody.
    /// </summary>
    private void CalculateMovement()
    {

        if (_gpMoving)
        {
            _targetDirection = Gamepad.current.leftStick.value;
            _targetVelocity = _targetDirection * maxSpeed;
            _rb.AddForce((_targetVelocity - _rb.linearVelocity) * 2);

            //Autoaim when not moving on controller
            if (!_gpAiming)
            {
                _gpPos = new Vector3(Gamepad.current.leftStick.value.x, Gamepad.current.leftStick.value.y, 0).normalized * 2;
                _aimDirRaw = _gpPos;
            }
        }
        // Kb/M movement
        else if (_kbMoving)
        {
            _targetDirection = (Mouse.current.position.ReadValue() - new Vector2(Screen.width / 2, Screen.height / 2)).normalized;
            _targetVelocity = _targetDirection * maxSpeed;
            _rb.AddForce((_targetVelocity - _rb.linearVelocity) * 2);
        }
    }

    /// <summary>
    /// Called every frame in fixed update
    /// </summary>
    private void CalculateAim()
    {
        if (_gpAiming)
        {
            _gpPos = new Vector3(Gamepad.current.rightStick.value.x, Gamepad.current.rightStick.value.y, 0).normalized * 2;
            _aimDirRaw = _gpPos;
        }
        // Kb/M aiming
        else if (_kbAiming && InputManager.CurrentDevice == InputManager.ControlScheme.Keyboard)
        {
            _mousePos = (Mouse.current.position.ReadValue() - new Vector2(Screen.width / 2, Screen.height / 2)).normalized * 2;
            _aimDirRaw = _mousePos;
        }
    }

    /// <summary>
    /// Updates crosshair position based on the current crosshair state
    /// </summary>
    private void CalculateCrosshair()
    {
        switch (_crosshairState)
        {
            case CrosshairState.Default:
                _crosshair.transform.position = _aimDirRaw + (Vector2)_rb.transform.position;
                break;
            case CrosshairState.MarkTrajectory:
                float chDistance = _chBaseDistance;

                //Add players velocity to the trajectory
                //Does not allow player velocity smaller than 1 because no spell in the game does either
                if (_chUsePlayerVelocity && Velocity > 1)
                    chDistance *= Velocity;

                //Limit crosshair distance if there is a max velocity and it's being exceeded
                if (_chMaxDistance > 0 && chDistance > _chMaxDistance)
                    chDistance = _chMaxDistance;

                //Hard limit on crosshair distance
                if (chDistance > 4) chDistance = 4;

                //Set the crosshair to be at the end of the aim vector with the magnitude of the distance traveled
                _crosshair.transform.position = (Vector2)transform.position + (AimDirNorm * chDistance);

                break;
        }
        
    }

    /// <summary>
    /// Changes the current crosshair state to match the spell
    /// 
    /// TODO: make some sort of categorization of crosshair type that can be held in spell information
    /// TODO: think of other indicators that may be helpful
    /// </summary>
    public void ChangeCrosshair(Spell spell)
    {
        if(spell.SpellName.Contains("Earth Barrier"))
        {
            _crosshairState = CrosshairState.MarkTrajectory;
            _chSpriteRenderer.sprite = _landingCrosshairSprite;
            float airTime = spell.spellObj.GetComponent<EarthBarrierObject>().AirTime;
            float velocity = spell.spellObj.GetComponent<Projectile>().InitialVelocity;
            _chUsePlayerVelocity = true;
            _chBaseDistance = airTime * velocity;
            _chMaxDistance = spell.spellObj.GetComponent<Projectile>().MaxVelocity;
        }
    }
    public void ResetCrosshair()
    {
        _crosshairState = CrosshairState.Default;
        _chSpriteRenderer.sprite = _defaultCrosshairSprite;
    }

    /// <summary>
    /// Player logic for spell casting
    /// Reset the crosshair and play proper animations
    /// </summary>
    public void SpellCast()
    {
        ResetCrosshair();

        _animator.SetBool("AbilityActive", true);
        StartCoroutine("ResumeAnimation", 0.7f);

        if (Moving)
            _animator.Play("Moving (Cast)");
        else
            _animator.Play("Idling (Cast)");
    }

    public void ExcaliburActivated()
    {
        _animator.SetBool("AbilityActive", true);
        StartCoroutine("ResumeAnimation", 0.7f);

        if (Moving)
            _animator.Play("Moving (Excalibur)");
        else
            _animator.Play("Idling (Excalibur)");
    }

    // ***** INTERACTION METHODS *****
    #region Interaction
    public void Interact(InputAction.CallbackContext context)
    {
        Interactable _currentTarget = FindInteractable();

        if (_currentTarget == null)
            return;

        if(context.performed)
            _currentTarget.OnInteract();
    }

    public Interactable FindInteractable()
    {
        Physics2D.OverlapCollider(_interactionCollider, _interactResults);

        if (_interactResults.Count > 0)
        {
            return _interactResults[0].GetComponentInParent<Interactable>();
        }
        
        _interactResults.Clear();
        return null;
    }

    #endregion

    // ***** MOVEMENT METHODS *****
    #region Movement
    public void OnKBM_AutoMove(InputAction.CallbackContext context)
    {
        _kbMoving = context.performed;
    }
    public void OnGP_Move(InputAction.CallbackContext context)
    {
        _gpMoving = context.started || context.performed;
    }
    public void Dash(float dashSpeed, float dashTime)
    {
        _speedMult = dashSpeed;
        _dashTarget = _crosshair.transform.localPosition;
        _targetVelocity = _dashTarget * maxSpeed * _speedMult;
        _rb.AddForce((_targetVelocity - _rb.linearVelocity) * 15);
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
    private IEnumerator ResumeAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        _animator.SetBool("AbilityActive", false);
    }

    // ***** DAMAGE HANDLING METHODS *****
    #region DamageHandlers
    public void OnTriggerEnter2D(Collider2D collision)
    {
        HandleDamageAfterCollision(collision);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        HandleDamageAfterCollision(collision);
    }

    /// <summary>
    /// Heals the player one chunk. The player can only heal if they are below max health
    /// </summary>
    public void Heal()
    {
        if (_health < maxHealth)
        {
            _health++;
            Healthbar.Instance.UpdateHealthbar(_health);
        }
    }

    /// <summary>
    /// Checks if the player is invincible and if the other collision object
    /// is an enemy. If the player is not invincible and the other object is an enemy, the player takes damage.
    /// </summary>
    /// <param name="collision"></param>
    private void HandleDamageAfterCollision(Collider2D collision)
    {
        if (_isInvincible || !(collision.CompareTag("Enemy") || collision.CompareTag("Hazard")))
        {
            // Not an enemy or hazard? Return
            // Also return if the player is currently invincible
            // May need to change this functionality later for damaging environmental effects
            return;
        }

        TakeDamage();
    }

    /// <summary>
    /// Subtracts one health from the player. Calls static health UI update methods. If health is 0 or less, the player dies.
    /// </summary>
    private void TakeDamage()
    {
        _health--;
        _isInvincible = true;
        Healthbar.Instance.UpdateHealthbar(_health);
        if(_hitStop != null && _health > 0)
        {
            // Don't call hitstop if the player is dead, it will break time scale
            _hitStop.Stop();
        }
        if(_health <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// Calls the static scene manager to switch to the GameOver screen
    /// </summary>
    private void Death()
    {
        // Need a reference to the scriptable object that contains the scene paths
        //LevelScenePaths levelScenePaths = ScriptableObject.CreateInstance<LevelScenePaths>();
        GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.GameOverScene);
    }

    /// <summary>
    /// Reset invincibility variables
    /// </summary>
    private void ResetInvincibility()
    {
        _timeSinceDamageTaken = 0;
        _isInvincible = false;
    }


    #endregion
}