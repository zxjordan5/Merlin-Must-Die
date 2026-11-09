using System;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField] protected int _health;
    protected int _initialHealth;
    private enum States { Wandering = 0, Attacking = 1, Paused = 2, Dead = 3 }
    private States CurrentState = States.Wandering;
    
    
    #endregion

    protected virtual void Awake()
    {
        _flash = GetComponent<Flash>();
        _player = FindAnyObjectByType<Player>();
        _enemyCollider = GetComponent<Collider2D>();

        // Will be changed in child class, this just makes Detection() work
        _health = _initialHealth = 1;
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
        }

        // Temp death logic
        if (CurrentState == States.Dead)
        {
            _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, _renderer.material.color.a - 0.03f);
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
        _rb.linearVelocityX = (float)Random.Range(-100, 100)/25;
        _rb.linearVelocityY = (float) Random.Range(-100, 100)/25;
        CurrentState = States.Wandering;
    }

    protected bool Detection()
    {
        // If the player's position overlaps with a point on the detection circle's collider or the enemy get hit, attack
        if (_detectionCircle.GetComponent<Collider2D>().OverlapPoint(_player.transform.position) ||
            _initialHealth != _health) //&& _attackCooldownTimer > _attackCooldown)
        {
            return true;
        }
        return false;
    }

    protected void TakeDamage(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<DamageObject>())
        {
            _health -= collision.gameObject.GetComponent<DamageObject>().Damage; 
        }

        if (_health <= 0)
        {
            CurrentState = States.Dead;
            Destroy(gameObject);

            // TODO: Make a shader for enemy death or make it a destructible object
            // DEPRECATED FADE OUT CODE
            // _renderer = GetComponent<Renderer>();
            // GetComponent<Rigidbody2D>().simulated = false;
            // foreach (Collider2D collider in GetComponents<Collider2D>())
            // {
            //     collider.enabled = false;
            // }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
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
        // Damage over time effect
        if (!collision.gameObject.GetComponent<DamageObject>())
        {
            return;
        }
        
        if (_timeSinceDamageOverTimeTick >= damageOverTimeWindow)
        {
           //Debug.Log("Damage Over Time");
           ResetDamageOverTimeTick();
           TakeDamage(collision);
           if (_flash != null)
           {
              _flash.SingleFlash();
           }
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

    protected abstract void Attack();
}
