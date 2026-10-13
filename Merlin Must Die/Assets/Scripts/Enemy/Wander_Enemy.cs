using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class Wander_Enemy : MonoBehaviour
{
    //Set in this class
    [SerializeField] protected Rigidbody2D _rb;
    [SerializeField] protected bool _wandering = false;
    [SerializeField] protected float _timer;

    //Set in child class
    protected float _leftBound, _rightBound, _topBound, _bottomBound;
    [SerializeField] protected float _wanderTime;
    [SerializeField] protected float _pauseTime;
    protected float _detectionRadius;

    //Individual timer for the spells cooldown
    protected virtual void FixedUpdate()
    {
        _timer += Time.deltaTime;

        if (_wandering == true && _timer >= _wanderTime)
        {
            _wandering = false;
            _rb.linearVelocityX = 0;
            _rb.linearVelocityY = 0;
            _timer = 0;
        }

        // Keep the enemy in their bounds
        if (_rb.transform.position.x > _rightBound || _rb.transform.position.x < _leftBound)
        {
            _rb.linearVelocityX *= -1;
        }
        if (_rb.transform.position.y > _topBound || _rb.transform.position.y < _bottomBound)
        {
            _rb.linearVelocityY *= -1;
        }

        if (_wandering == false && _timer >= _pauseTime)
        {
            Wander();
            _timer = 0;
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
}
