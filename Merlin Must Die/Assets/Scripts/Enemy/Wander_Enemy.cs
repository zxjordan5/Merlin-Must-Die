using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class Wander_Enemy : MonoBehaviour
{
    // Temp vars
    protected Renderer _renderer;

    //Set in this class
    [SerializeField] protected Rigidbody2D _rb;
    protected bool _wandering = false;
    protected bool _attacking = false;
    protected bool _dead = false;
    protected Vector2 _previousPOS;
    protected float _timer;
    protected Player _player;
    protected Collider2D _enemyCollider;

    //Set in child class
    protected float _leftBound, _rightBound, _topBound, _bottomBound;
    protected float _wanderTime;
    protected float _pauseTime;
    protected GameObject _detectionCircle;
    protected int _health;


    protected virtual void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        _enemyCollider = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        _timer += Time.deltaTime;
        _detectionCircle.transform.position = _rb.transform.position;

        // Wandering Logic
        if (!_attacking && !_dead)
        {
            // Make the enemy pause
            if (_wandering && _timer >= _wanderTime)
            {
                _wandering = false;
                _rb.linearVelocityX = 0;
                _rb.linearVelocityY = 0;
                _timer = 0;
            }

            // Once the enemy has paused for long enough, wander again
            if (!_wandering && _timer >= _pauseTime)
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
        }


        // Attacking Logic
        if (!_dead)
        {
            // If the player has not been detected, keep checking
            Detection();
            // Once the player is detected, the enemy allways goes towards their location
            if (_attacking)
            {
                _rb.transform.position = Vector2.MoveTowards(
                    _rb.transform.position,
                    _player.transform.position,
                    1.5f * Time.deltaTime);
            }

            // Flip the sprite properly
            if (_previousPOS.x > transform.position.x && transform.localScale.x > 0) 
            { 
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z); 
            }
            else if (_previousPOS.x < transform.position.x && transform.localScale.x < 0) 
            { 
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
        // Temp death logic
        else if (_dead)
        {
            _renderer.material.color = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, _renderer.material.color.a - 0.03f);
        }


        // To make sure the sprite flips
        _previousPOS = transform.position;
    }

    protected virtual void Wander()
    {
        _rb.linearVelocityX = (float)Random.Range(-100, 100)/50;
        _rb.linearVelocityY = (float) Random.Range(-100, 100)/50;
        _wandering = true;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DamageSpell"))
        {
            _health--;
        }
        if (_health <= 0)
        {
            _dead = true;
            _renderer = GetComponent<Renderer>();
            GetComponent<Rigidbody2D>().simulated = false;
            foreach (Collider2D collider in GetComponents<Collider2D>())
            {
                collider.enabled = false;
            }
        }
    }

    protected abstract void Detection();
}
