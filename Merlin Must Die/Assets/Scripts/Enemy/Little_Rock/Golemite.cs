using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class Golemite : Wander_Enemy
{
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _rollTime;

    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackCooldownTimer;

    private Animator _animator;

    public enum State{ Wandering = 0, Attacking = 1, Charging = 2, Rolling = 3, Dead = 4 }
    public State CurrentState { get; private set; } = State.Wandering;
    [SerializeField] private int _state = 0;
    public State MoveState
    {
        get
        {
            if (CurrentState == State.Charging)
                return State.Charging;
            if (CurrentState == State.Rolling)
                return State.Rolling;

            if (CurrentState == State.Attacking)
                return State.Attacking;

            return State.Wandering;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();

        _wanderTime = (float)Random.Range(25, 50) / 50; // Wanders for 1-2 seconds
        _pauseTime = (float)Random.Range(1, 2) / 50; // Pauses for 0.5-1 seconds
        _target = new Vector2(1,1);

        // Make the enemy's decection circle
        _detectionCircle = new GameObject("RockDetection");
        _detectionCircle.transform.position = _rb.transform.position;
        CircleCollider2D _collider = _detectionCircle.AddComponent<CircleCollider2D>();
        _collider.radius = 3.5f;
        _collider.isTrigger = true;

        _initialHealth = 2;
        _health = _initialHealth;

        _leftBound = _rb.transform.position.x - 3;
        _rightBound = _rb.transform.position.x + 3;
        _topBound = _rb.transform.position.y + 3;
        _bottomBound = _rb.transform.position.y - 3;
    }

    protected void Update()
    {
        _animator.SetInteger("State", (int)MoveState);
        
        if (Detection() && CurrentState == State.Wandering)
        {
            CurrentState = State.Attacking;
            _rb.linearVelocity = new Vector2(0, 0);
        }

        _state = (int)CurrentState;

        //_attackCooldownTimer += Time.deltaTime;

        //if (_attackCooldownTimer > _attackCooldown)
        //{
        //    _attacking = false;
        //}
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
                1.5f * MovementSpeedMultiplier * Time.deltaTime);
        }

        if (CurrentState == State.Rolling)
        {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _target,
                5 * MovementSpeedMultiplier * Time.deltaTime);
        }
        
    }

    protected override void Attack()
    {
        // If the player gets close enough, the rock stops, rolls for a sec, then dashes towards the players POS
        if (Vector2.Distance(_player.transform.position, _rb.transform.position) < 2)
        {
            _rb.linearVelocity = new Vector2(1, 1) * MovementSpeedMultiplier;
            _target = _player.transform.position;
            StartCoroutine("RollTimer");
        }
    }

    protected override void Wander()
    {
        _wanderTime = (float)Random.Range(25, 50) / 50; 
        _pauseTime = (float)Random.Range(10, 25) / 50; 

        base.Wander();
    }

    IEnumerator RollTimer()
    {
        CurrentState = State.Charging;
        yield return new WaitForSeconds(_chargeTime);
        _target = _player.transform.position + (_player.transform.position - _rb.transform.position)*2;
        CurrentState = State.Rolling;
        yield return new WaitForSeconds(_rollTime);
        CurrentState = State.Attacking;
        _target = new Vector2(1,1);
        //_attackCooldownTimer = 0;
    }

}
