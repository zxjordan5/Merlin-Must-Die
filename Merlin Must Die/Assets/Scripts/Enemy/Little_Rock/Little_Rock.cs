using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class Little_Rock : Wander_Enemy
{
    protected override void Awake()
    {
        base.Awake();

        _wanderTime = (float)Random.Range(50, 150) / 50; // Wanders for 1-3 seconds
        _pauseTime = (float)Random.Range(50, 100) / 50; // Pauses for 0.5-1 seconds

        // Make the enemy's decection circle
        _detectionCircle = new GameObject("RockDetection");
        _detectionCircle.transform.position = _rb.transform.position;
        CircleCollider2D _collider = _detectionCircle.AddComponent<CircleCollider2D>();
        _collider.radius = 4;
        _collider.isTrigger = true;

        _health = 1;

        _leftBound = _rb.transform.position.x - 3;
        _rightBound = _rb.transform.position.x + 3;
        _topBound = _rb.transform.position.y + 3;
        _bottomBound = _rb.transform.position.y - 3;
    }

    protected override void Wander()
    {
        _wanderTime = (float)Random.Range(50, 150) / 50; // Wanders for 1-3 seconds
        _pauseTime = (float)Random.Range(50, 100) / 50; // Pauses for 0.5-1 seconds

        base.Wander();
    }

    protected override void Detection()
    {
        // If the player's position overlaps with a point on the detection circle's collider, attack
        if (_detectionCircle.GetComponent<Collider2D>().OverlapPoint(_player.transform.position) && !_attacking)
        {
            _attacking = true;
            _rb.linearVelocity = new Vector2(0,0);
        }
    }

}
