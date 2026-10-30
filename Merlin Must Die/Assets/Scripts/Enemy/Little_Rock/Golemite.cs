using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class Golemite : Wander_Enemy
{
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _rollTime;

    [SerializeField] private float _attackCooldown;
    private float _attackCooldownTimer;

    private Animator _animator;
    private bool _charging;

    public enum State { Idle = 0, Walking = 1, Charging = 2, Rolling = 3}
    public State MoveState
    {
        get
        {
            if (_charging)
                return State.Charging;
            if (_rolling)
                return State.Rolling;

            if (_wandering)
                return State.Walking;

            
            return State.Idle;
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
        _collider.radius = 4;
        _collider.isTrigger = true;

        _health = 2;
        _initialHealth = _health;
        _rolling = false;

        _leftBound = _rb.transform.position.x - 3;
        _rightBound = _rb.transform.position.x + 3;
        _topBound = _rb.transform.position.y + 3;
        _bottomBound = _rb.transform.position.y - 3;
    }

    protected override void Wander()
    {
        _wanderTime = (float)Random.Range(25, 50) / 50; // Wanders for 1-2 seconds
        _pauseTime = (float)Random.Range(1, 2) / 50; // Pauses for 0.5-1 seconds

        base.Wander();
    }

    protected void Update()
    {
        _animator.SetInteger("State", (int)MoveState);
        _attackCooldownTimer += Time.deltaTime;

        if (_attackCooldownTimer > _attackCooldown)
        {
            _attacking = false;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Once the player is detected, the enemy allways goes towards their location to attack
        if (_attacking && _target == new Vector2(1,1))
        {
            Attack();
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _player.transform.position,
                1.5f * Time.deltaTime);
        }

        if (_rolling)
        {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _target,
                5 * Time.deltaTime);
        }

        // Flip the sprite properly when rolling up
        if (_player.transform.position.x - _rb.transform.position.x < 0 && transform.localScale.x > 0 && !_rolling)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        if (_player.transform.position.x - _rb.transform.position.x > 0 && transform.localScale.x < 0 && !_rolling)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    protected override void Detection()
    {
        // If the player's position overlaps with a point on the detection circle's collider or the enemy get hit, attack
        if ((_detectionCircle.GetComponent<Collider2D>().OverlapPoint(_player.transform.position) 
            || _initialHealth != _health) && !_attacking && _attackCooldownTimer > _attackCooldown)
        {
            _attacking = true;
            _rb.linearVelocity = new Vector2(0,0);
        }
    }

    protected override void Attack()
    {
        // If the player gets close enough, the rock stops, rolls for a sec, then dashes towards the players POS
        if (Vector2.Distance(_player.transform.position, _rb.transform.position) < 2.5)
        {
            _rb.linearVelocity = new Vector2(1,1);
            _target = _player.transform.position;
            StartCoroutine("RollTimer");
        }
    }

    IEnumerator RollTimer()
    {
        _charging = true;
        yield return new WaitForSeconds(_chargeTime);
        _target = _player.transform.position + (_player.transform.position - _rb.transform.position)*2;
        _charging = false;
        _rolling = true;
        yield return new WaitForSeconds(_rollTime);
        _rolling = false;
        _target = new Vector2(1,1);
        _attackCooldownTimer = 0;
    }

}
