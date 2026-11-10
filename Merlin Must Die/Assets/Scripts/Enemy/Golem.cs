using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class Golem : Wander_Enemy
{
    [SerializeField] private float _aimTime;
    [SerializeField] private float _reloadTime;

    [SerializeField] private GameObject _rockPrefab;
    private GameObject[] _rocks;
    private Vector3[] _rockPOS;
    private Rigidbody2D[] _rockRB;

    // ***** ROTATION FIELDS ***** //

    /// <summary>
    /// The orbit speed of the Golem's rocks in Degrees/Second
    /// </summary>
    private float _orbitSpeed = 45.0f;

    private Animator _animator;

    public enum State{ Sleeping = 0, Attacking = 1, Aiming = 2, Shooting = 3, Slaming = 4, Dead = 5 }
    public State CurrentState { get; private set; } = State.Sleeping;
    [SerializeField] private int _state = 0;
    public State MoveState
    {
        get
        {
            /*
            if (CurrentState == State.Charging)
                return State.Charging;
            if (CurrentState == State.Rolling)
                return State.Rolling;

            if (CurrentState == State.Attacking)
                return State.Attacking;
            */
            return State.Sleeping; 
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();
        _target = new Vector2(1,1);

        _aimTime = (float)Random.Range(25, 50) / 50; // Aims for 0.5-1 seconds
        _reloadTime = (float)Random.Range(100, 200) / 50; // Reloads for 2-4 seconds

        // Make the enemy's decection circle
        _detectionCircle = new GameObject("RockDetection");
        //_detectionCircle.transform.SetParent(transform);
        _detectionCircle.transform.position = _rb.transform.position;
        CircleCollider2D _collider = _detectionCircle.AddComponent<CircleCollider2D>();
        _collider.radius = 5f;
        _collider.isTrigger = true;

        _initialHealth = 4;
        _health = _initialHealth;

        // Set up variables for the golem's projectiles
        _rocks = new GameObject[5];
        _rockRB = new Rigidbody2D[5];
        _rockPOS = new Vector3[5];
        _rockPOS[0] = new Vector3(-0.35f, 0.29f, 0.05f);
        _rockPOS[1] = new Vector3(0.35f, 0.29f, 0.05f);
        _rockPOS[2] = new Vector3(0, -0.53f, -0.05f);
        _rockPOS[3] = new Vector3(-0.61f, -0.2f, -0.05f);
        _rockPOS[4] = new Vector3(0.61f, -0.2f, -0.05f);
    }

    protected void Update()
    {
        //_animator.SetInteger("State", (int)MoveState);
        
        if (Detection() && CurrentState == State.Sleeping)
        {
            Debug.Log("Player Detected");
            CurrentState = State.Attacking;
            
        }

        _state = (int)CurrentState;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Once the player is detected, the enemy allways goes towards their location to attack
        if (CurrentState == State.Attacking && _target == new Vector2(1,1))
        {
            Attack();
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _player.transform.position,
                0.5f * Time.deltaTime);
        }

        // The parent sprite flipping, also flips the rocks completely 
        if(transform.localScale.x < 0)
        {
            // (this stops the flipping)
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        // TODO: Surround this with state check
        foreach(Rigidbody2D rb in _rockRB)
        {
            rb.transform.RotateAround(transform.position, Vector3.up, _orbitSpeed * Time.deltaTime);
        }
        
        // Top Y bound = 0.48
        // Bottom Y bound = -0.54
        // Right X bound = 0.9
        // Left X bound = -0.9
        // Rotate the rocks
        /*
        if (CurrentState != State.Sleeping)
        {
            for (int i = 0; i < _rocks.Length; i++)
            {
                if (_rocks[i].transform.localPosition.x >= 0 && _rocks[i].transform.localPosition.y < 0.053)
                {
                    Debug.Log("movin");
                    _rockRB[i].MovePosition(new Vector2(0.015f + _rockRB[i].transform.position.x, 0.0093f + _rockRB[i].transform.position.y));
                }
                else if (_rocks[i].transform.localPosition.x >= 0 && _rocks[i].transform.localPosition.y > 0.053)
                {
                    _rockRB[i].MovePosition(new Vector2(-0.015f + _rockRB[i].transform.position.x, 0.0075f + _rockRB[i].transform.position.y));
                }
                else if (_rocks[i].transform.localPosition.x <= 0 && _rocks[i].transform.localPosition.y > 0.053)
                {
                    _rockRB[i].MovePosition(new Vector2(-0.015f + _rockRB[i].transform.position.x, -0.0075f + _rockRB[i].transform.position.y));
                }
                else if (_rocks[i].transform.localPosition.x <= 0 && _rocks[i].transform.localPosition.y < 0.053)
                {
                    _rockRB[i].MovePosition(new Vector2(0.015f + _rockRB[i].transform.position.x, -0.0093f + _rockRB[i].transform.position.y));
                }
            }
        }
        */

    }

    protected override void Attack()
    {
        // If the player gets too close, the golem stomps
        if (Vector2.Distance(_player.transform.position, _rb.transform.position) < 2)
        {
            CurrentState = State.Slaming;
            _target = _player.transform.position;
        }

        // If the player gets close enough, the golem stops, aims for a sec, then fires a rock towards the players POS
        else if (Vector2.Distance(_player.transform.position, _rb.transform.position) < 4)
        {
            _rb.linearVelocity = new Vector2(0,0);
            _target = _player.transform.position;
            StartCoroutine("AimTimer");
        }
    }

    protected override bool Detection()
    {
        // If the player's position overlaps with a point on the detection circle's collider or the enemy get hit, attack
        if (_detectionCircle.GetComponent<CircleCollider2D>().OverlapPoint(_player.transform.position) ||
            _initialHealth != _health)  
        {
            // Once the player is detected, create the rocks
            for (int i = 0; i < _rocks.Length; i++)
            {
                if (_rocks[i] == null) // Should only be called once, but just in case
                {
                    _rocks[i] = Instantiate(_rockPrefab, transform.position + _rockPOS[i], Quaternion.identity);
                    _rocks[i].transform.SetParent(transform);
                    _rockRB[i] = _rocks[i].GetComponent<Rigidbody2D>();
                    if (i == 0 || i == 1)
                    {
                        _rocks[i].transform.position = new Vector3(_rocks[i].transform.position.x, _rocks[i].transform.position.y, 0.12f);
                    }
                    else
                    {
                        _rocks[i].transform.position = new Vector3(_rocks[i].transform.position.x, _rocks[i].transform.position.y, 0.05f);
                    }
                    
                }
            }
            
            return true;
        }
        return false;
    }

    protected override void TakeDamage(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<DamageObject>() && CurrentState != State.Sleeping)
        {
            _health -= collision.gameObject.GetComponent<DamageObject>().Damage; 
        }

        if (_health <= 0)
        {
            CurrentState = State.Dead;
            Destroy(gameObject);
        }
    }

    protected override void Wander()
    {
        // Stops the golem from wandering
        _rb.linearVelocity = new Vector2(0,0);
    }

    IEnumerator AimTimer()
    {
        CurrentState = State.Aiming;
        yield return new WaitForSeconds(_aimTime);
        _target = _player.transform.position + (_player.transform.position - _rb.transform.position)*2;
        CurrentState = State.Shooting;
        yield return new WaitForSeconds(_reloadTime);
        CurrentState = State.Attacking;
        _target = new Vector2(1,1);
        //_attackCooldownTimer = 0;
    }

}
