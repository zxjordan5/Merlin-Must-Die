using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class Wander_Enemy : MonoBehaviour
{
    // Test vars

    //Set in this class
    [SerializeField] protected Rigidbody2D _rb;
    protected bool _wandering = false;
    [SerializeField] protected bool _attacking = false;
    protected float _timer;
    protected Player _player;

    //Set in child class
    protected float _leftBound, _rightBound, _topBound, _bottomBound;
    protected float _wanderTime;
    protected float _pauseTime;
    protected GameObject _detectionCircle;

    protected virtual void Awake()
    {
        _player = FindAnyObjectByType<Player>();
    }

    protected virtual void FixedUpdate()
    {
        _timer += Time.deltaTime;
        _detectionCircle.transform.position = _rb.transform.position;

        // Make the enemy pause
        if (_wandering && _timer >= _wanderTime && !_attacking)
        {
            _wandering = false;
            _rb.linearVelocityX = 0;
            _rb.linearVelocityY = 0;
            _timer = 0;
        }
        
        // Once the enemy has paused for long enough, wander again
        if (!_wandering && _timer >= _pauseTime && !_attacking)
        {
            Wander();
            _timer = 0;
        }

        // If the player has not been detected, keep checking
        if (!_attacking) { Detection(); }
        // Once the player is detected, the enemy allways goes towards their location
        if (_attacking)
        {
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _player.transform.position,
                1.5f * Time.deltaTime);
        }

        // Flip the sprite properly (ONLY WORKS WHEN WANDERING RN)
        if (_rb.linearVelocityX > 0 && transform.localScale.x < 0) 
        { 
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z); 
        }
        else if (_rb.linearVelocityX < 0 && transform.localScale.x > 0) 
        { 
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        // Keep the enemy in their bounds when not attacking
        if ((_rb.transform.position.x > _rightBound || _rb.transform.position.x < _leftBound)
            && !_attacking)
        {
            _rb.linearVelocityX *= -1;
        }
        if ((_rb.transform.position.y > _topBound || _rb.transform.position.y < _bottomBound)
            && !_attacking)
        {
            _rb.linearVelocityY *= -1;
        }

    }

    protected virtual void Wander()
    {
        _rb.linearVelocityX = (float)Random.Range(-100, 100)/50;
        _rb.linearVelocityY = (float) Random.Range(-100, 100)/50;
        //_rb.transform.position = Vector2.MoveTowards(
        //    _rb.transform.position,
        //    new Vector2((Random.Range(-100,100)/100) + _rb.transform.position.x, (Random.Range(-100,100)/100) + _rb.transform.position.y),
        //    5);
        _wandering = true;
    }

    protected abstract void Detection();
}
