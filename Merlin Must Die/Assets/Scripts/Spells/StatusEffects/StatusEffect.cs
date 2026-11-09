using UnityEngine;

/// <summary>
/// Generic status effect script. Can be attached to either static objects or an enemy. Static objects will not receive any
/// movement-based effects of the status effects.
/// </summary>
public class StatusEffect : MonoBehaviour
{
    [SerializeField] protected float effectDuration;
    [SerializeField] protected ParticleSystem effectParticles;
    
    /// <summary>
    /// The enemy the script is attached to. Null if not attached to an enemy.
    /// </summary>
    protected Wander_Enemy _enemy;
    
    /// <summary>
    /// Rigidbody of the object the script is attached to. Null if there is no rigidbody.
    /// </summary>
    protected Rigidbody2D _rb;

    private float elapsedTime = 0.0f;

    protected virtual void Start()
    {
        _enemy = GetComponent<Wander_Enemy>();
        _rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= effectDuration)
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Resets the status effect's timer. Called when a status effect is reapplied while the object still has the effect active.
    /// </summary>
    public void ResetTimer()
    {
        elapsedTime = 0.0f;
    }
}
