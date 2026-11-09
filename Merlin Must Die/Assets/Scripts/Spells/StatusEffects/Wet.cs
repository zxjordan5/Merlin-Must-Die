using UnityEngine;

/// <summary>
/// Halves an enemy's current velocity and movement speed for the effect's duration.
/// </summary>
public class Wet : StatusEffect
{
    private const float MovementSpeedModifier = 0.5f;
    private float _previousMovementSpeedMultiplier = 1f;
    

    protected override void Start()
    {
        base.Start();
        effectDuration = 5.0f;

        // Set enemy movement speed multiplication
        if (_enemy)
        {
            _previousMovementSpeedMultiplier = _enemy.MovementSpeedMultiplier;
            _enemy.SetMovementSpeedMultiplier(_previousMovementSpeedMultiplier * MovementSpeedModifier);
        }

        // Ensure enemy and rigidbody velocity remains consistent
        if (_enemy && _rb)
        {
            _rb.linearVelocity *= MovementSpeedModifier;
        }
    }

    private void OnDestroy()
    {
        if (_enemy)
        {
            _enemy.SetMovementSpeedMultiplier(_previousMovementSpeedMultiplier);
        }
    }
}
