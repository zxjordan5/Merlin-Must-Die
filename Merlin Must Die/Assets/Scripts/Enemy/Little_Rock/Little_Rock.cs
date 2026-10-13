using UnityEngine;

public class Little_Rock : Wander_Enemy
{
    void Awake()
    {
        _wanderTime = (float)Random.Range(50, 150) / 50; // Wanders for 1-3 seconds
        _pauseTime = (float)Random.Range(50, 100) / 50; // Pauses for 0.5-1 seconds

        _detectionRadius = 4.0f;

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

}
