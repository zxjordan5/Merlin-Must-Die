using System;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[System.Serializable]
public abstract class Wander_Enemy : MonoBehaviour
{
    #region Variables

    // Temp vars
    protected Renderer _renderer;
    protected Flash _flash;

    //Set in this class
    [SerializeField] protected Rigidbody2D _rb;
    protected Vector2 _previousPOS;
    protected float _timer;
    protected Player _player;
    protected Collider2D _enemyCollider;
    protected CircleCollider2D _detectionCollider;
    
    /// <summary>
    /// How much time the player is invincible for after being hit.
    /// </summary>
    [SerializeField]
    protected float damageOverTimeWindow = 1.0f;

    /// <summary>
    /// How much time has passed since the enemy last took damage from a damage over time effect.
    /// </summary>
    protected float _timeSinceDamageOverTimeTick;

    /// <summary>
    /// Whether or not the enemy is taking damage over time. True if they are within a damaging effect such as Back In The Stone's damaging field, false otherwise.
    /// </summary>
    protected bool _isTakingDamageOverTime;

    //Set in child class
    protected float _leftBound, _rightBound, _topBound, _bottomBound;
    protected float _wanderTime;
    protected float _pauseTime;
    protected GameObject _detectionCircle;
    protected Vector2 _target;
    /// <summary>
    /// Movement speed multiplier. Needed for status effects
    /// </summary>
    [SerializeField] private float _moveSpeedMult = 1f;
    public float MoveSpeedMult { get { return _moveSpeedMult; } private set { _moveSpeedMult = value; } }
    [SerializeField] protected int _health;
    protected int _initialHealth;
    private enum States { Wandering = 0, Attacking = 1, Paused = 2, Dead = 3 }
    private States CurrentState = States.Wandering;

    [SerializeField] protected float detectionRadius;
    /// <summary>
    /// Determines the radius of the detection collider once the enemy is Awake. If the player
    /// leaves this radius, the enemy loses aggro towards the player
    /// </summary>
    protected float _aggroRadius;
    
    #endregion


    /// <summary>
    /// Event that is invoked when an enemy dies.
    /// </summary>
    public event Action<Wander_Enemy> OnEnemyDeath;
    
    protected virtual void Awake()
    {
        _flash = GetComponent<Flash>();
        _enemyCollider = GetComponent<Collider2D>();

        // Will be changed in child class, this just makes Detection() work
        _health = _initialHealth = 1;
        
        _aggroRadius = 2 * detectionRadius;
        
        SetUpDetection();
    }

    //Check for player after awake to allow for the player to load in across scenes
    private void Start()
    {
        _player = GameManager.Player;
    }

    protected virtual void FixedUpdate()
    {
        _timer += Time.deltaTime;
        _detectionCircle.transform.position = _rb.transform.position;

        if (CurrentState == States.Wandering || CurrentState == States.Paused)
        {
            // Make the enemy pause
            if (CurrentState == States.Wandering && _timer >= _wanderTime)
            {
                CurrentState = States.Paused;
                _rb.linearVelocityX = 0;
                _rb.linearVelocityY = 0;
                _timer = 0;
            }

            // Once the enemy has paused for long enough, wander again
            if (CurrentState == States.Paused && _timer >= _pauseTime)
            {
                Wander();
                _timer = 0;
            }

            // Keep the enemy in their bounds when not attacking
            if (_rb.transform.position.x > _rightBound || _rb.transform.position.x < _leftBound)
            {
                _rb.linearVelocityX *= -1;
            }
            if (_rb.transform.position.y > _topBound || _rb.transform.position.y < _bottomBound)
            {
                _rb.linearVelocityY *= -1;
            }

            // If the player has not been detected, keep checking
            if (Detection())
            {
                CurrentState = States.Attacking;
                //_rb.linearVelocity = new Vector2(0, 0);
            }

            // Flip the sprite properly when wandering
            if (_previousPOS.x > transform.position.x && transform.localScale.x > 0) 
            { 
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z); 
            }
            else if (_previousPOS.x < transform.position.x && transform.localScale.x < 0) 
            { 
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
        
        // Flip the sprite properly when attacking
        else if (CurrentState == States.Attacking)
        {
            if (_player.transform.position.x - _rb.transform.position.x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            if (_player.transform.position.x - _rb.transform.position.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }

            if (!Detection())
            {
                // PLayer is out of detection range
                PlayerOutOfRange();
            }
        }

        // To make sure the sprite flips
        _previousPOS = transform.position;
        
        #region DamageOverTimeTimer

        if (_isTakingDamageOverTime)
        {
            _timeSinceDamageOverTimeTick += Time.deltaTime;
            //Debug.Log("DoT Tick: " + _timeSinceDamageOverTimeTick);
        }

        #endregion
    }

    protected virtual void Wander()
    {
        _rb.linearVelocityX = (float)Random.Range(-100, 100) / 25 * MoveSpeedMult;
        _rb.linearVelocityY = (float)Random.Range(-100, 100) / 25 * MoveSpeedMult;
        CurrentState = States.Wandering;
    }

    public void SetMovementSpeedMultiplier(float multiplier)
    {
        MoveSpeedMult = Mathf.Max(0f, multiplier);
    }

    protected virtual bool Detection()
    {
        // If the player's position overlaps with a point on the detection circle's collider or the enemy get hit, attack
        if (_detectionCircle.GetComponent<Collider2D>().OverlapPoint(_player.transform.position) ||
            _initialHealth != _health) //&& _attackCooldownTimer > _attackCooldown)
        {
            return true;
        }
        return false;
    }

    protected virtual void TakeDamage(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<DamageObject>())
        {
            _health -= collision.gameObject.GetComponent<DamageObject>().Damage; 
        }

        if (_health <= 0)
        {
            //_dead = true;
            CurrentState = States.Dead;

            // Enemy death event is invoked. Do not destroy the enemy here.
            // Instead, pass it to the LevelManager so the event dependencies can
            // be undone, and then destroy the gameobject there
            OnEnemyDeath?.Invoke(this);
            gameObject.SetActive(false);

            // TODO: Make a shader for enemy death or make it a destructible object
        }
    }

    /// <summary>
    /// Updates the status effects of the enemy after they have been hit by a spell.
    /// </summary>
    /// <param name="spellObject"></param>
    protected void UpdateStatusEffects(SpellObject spellObject)
    {
        ElementalType type = spellObject.ElementalType;
        switch (type)
        {
            case ElementalType.Water:
                #region WetLogic
                Wet wetStatus = gameObject.GetComponent<Wet>(); 
                if (wetStatus)
                {
                    // If the status effect is an existing component on this object, reset it
                    // there are no additional prerequisites for applying this status effect
                    wetStatus.ResetTimer();
                }
                else
                {
                    // Wet status doesn't exist, add the component to this game object
                    gameObject.AddComponent<Wet>();
                }
                #endregion
                break;
            case ElementalType.Lightning:
                #region ShockedLogic
                // To become Shocked, the enemy must have the Wet status effect first
                
                Shocked shockedStatus = gameObject.GetComponent<Shocked>(); 
                if (gameObject.GetComponent<Wet>())
                {
                    if (shockedStatus)
                    {
                        // If the enemy is already Shocked AND the prerequisites for
                        // being shocked are met (enemy is Wet), reset the timer of the
                        // shocked status
                        shockedStatus.ResetTimer();
                    }
                    else
                    {
                        gameObject.AddComponent<Shocked>();
                    }
                }
                #endregion
                break;
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        SpellObject spellObject = collision.gameObject.GetComponent<SpellObject>();
        if (spellObject)
        {
            // A spell has hit the object, we need to update the status effects of the enemy
            UpdateStatusEffects(spellObject);
        }
        
        
        if (!collision.gameObject.GetComponent<DamageObject>())
        {
            return;
        }
        
        // This check prevents repeated damage calls
        if (!_isTakingDamageOverTime)
        {
            // Initial damage
            TakeDamage(collision);
            _isTakingDamageOverTime = true;  
        }
    }
    
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.GetComponent<DamageObject>())
        {
            return;
        }
            
        if (_timeSinceDamageOverTimeTick >= damageOverTimeWindow) 
        { 
            //Debug.Log("Damage Over Time");
            ResetDamageOverTimeTick();
            TakeDamage(collision); 
            if (_flash != null) { _flash.SingleFlash(); }
        }
    }
    
    protected void OnTriggerExit2D(Collider2D other)
    {
        ResetDamageOverTimeFields();
    }

    protected void ResetDamageOverTimeTick()
    {
        _timeSinceDamageOverTimeTick = 0.0f;
    }

    protected void ResetDamageOverTimeFields()
    {
        _isTakingDamageOverTime = false;
        _timeSinceDamageOverTimeTick = 0.0f;
    }

    /// <summary>
    /// Initializes enemy detection collider
    /// </summary>
    protected void SetUpDetection()
    {
        // Make the enemy's detection circle
        _detectionCircle = new GameObject($"{this.name}Detection");
        _detectionCircle.transform.position = _rb.transform.position;
        _detectionCollider = _detectionCircle.AddComponent<CircleCollider2D>();
        _detectionCollider.radius = detectionRadius;
        _detectionCollider.isTrigger = true;
    }

    /// <summary>
    /// Method that handles the attacking logic of the enemy
    /// </summary>
    protected abstract void Attack();

    /// <summary>
    /// Method that handles enemy behavior when the player exits detection range
    /// </summary>
    protected abstract void PlayerOutOfRange();
}
