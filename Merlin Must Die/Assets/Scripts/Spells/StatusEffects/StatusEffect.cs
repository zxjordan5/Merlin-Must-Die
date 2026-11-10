using System.Collections;
using UnityEngine;

/// <summary>
/// Generic status effect script. Can be attached to either static objects or an enemy. Static objects will not receive any
/// movement-based effects of the status effects.
/// </summary>
public class StatusEffect : MonoBehaviour
{
    // Behavioral fields. Since status effects are only created at runtime, these
    // are only serialized for debugging in the editor.
    [SerializeField] protected float effectDuration;
    [SerializeField] protected GameObject effectParticlesObj;
    protected  GameObject _instantiatedParticleObjRef;
    
    
    /// <summary>
    /// The enemy the script is attached to. Null if not attached to an enemy.
    /// </summary>
    protected Wander_Enemy _enemy;
    
    /// <summary>
    /// Rigidbody of the object the script is attached to. Null if there is no rigidbody.
    /// </summary>
    protected Rigidbody2D _rb;

    private float _elapsedTime = 0.0f;

    protected virtual void Start()
    {
        _enemy = GetComponent<Wander_Enemy>();
        _rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= effectDuration)
        {
            Destroy(this);
            Destroy(_instantiatedParticleObjRef);
        }
    }

    /// <summary>
    /// Resets the status effect's timer. Called when a status effect is reapplied while the object still has the effect active.
    /// </summary>
    public void ResetTimer()
    {
        _elapsedTime = 0.0f;
    }

    /// <summary>
    /// Loads the particle effect objects for the given status effect. Should always be overridden in child classes
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator LoadParticleEffects()
    {
        yield return null;
    }
}
