//using UnityEditor.Rendering;
using UnityEngine;

public class SpellObject : MonoBehaviour
{
    [SerializeField] protected float excaliburChargePerHit;
    [SerializeField] protected ElementalType elementalType;

    protected Excalibur _excalibur;

    public ElementalType ElementalType
    {
        get => elementalType;
    }
    
    public virtual void Awake()
    {
        _excalibur = FindAnyObjectByType<Excalibur>();
    }

    /// <summary>
    /// Calls collision methods based on what the spell trigger hits
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        //Using object collision becuase there is no current need for different logic
        if (LayerMask.LayerToName(other.gameObject.layer) == "Environment")
        {
            ObjectCollision(other);
        }

        if (other.gameObject.CompareTag("EnvironmentObject") || other.gameObject.CompareTag("Destructible"))
        {
            ObjectCollision(other);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyCollision(other);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other) { }

    /// <summary>
    /// Called when the spell hits something on the environment layer
    /// </summary>
    /// <param name="other"></param>
    protected virtual void EnvironmentCollision(Collider2D other) { }

    /// <summary>
    /// Called when the spell hits an object
    /// </summary>
    /// <param name="other"></param>
    protected virtual void ObjectCollision(Collider2D other) { }

    /// <summary>
    /// Called when the spell hits an enemy
    /// </summary>
    /// <param name="other"></param>
    protected virtual void EnemyCollision(Collider2D other) 
    {
        _excalibur.IncreaseCharge(excaliburChargePerHit);
    }
}
