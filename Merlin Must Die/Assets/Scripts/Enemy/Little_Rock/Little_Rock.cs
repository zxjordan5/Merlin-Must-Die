using UnityEngine;

public class Little_Rock : Wander_Enemy
{
    protected override void Awake()
    {
        base.Awake();

        _wanderTime = (float)Random.Range(25, 50) / 50; // Wanders for 1-2 seconds
        _pauseTime = (float)Random.Range(1, 2) / 50; // Pauses for 0.5-1 seconds

        // Make the enemy's decection circle
        _detectionCircle = new GameObject("RockDetection");
        _detectionCircle.transform.position = _rb.transform.position;
        CircleCollider2D _collider = _detectionCircle.AddComponent<CircleCollider2D>();
        _collider.radius = 4;
        _collider.isTrigger = true;

        _health = 2;
        _initialHealth = _health;

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

    protected override void Detection()
    {
        // If the player's position overlaps with a point on the detection circle's collider or the enemy get hit, attack
        if ((_detectionCircle.GetComponent<Collider2D>().OverlapPoint(_player.transform.position) 
            || _initialHealth != _health) && !_attacking)
        {
            _attacking = true;
            _rb.linearVelocity = new Vector2(0,0);
        }
    }

}
