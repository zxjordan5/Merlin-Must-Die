
using UnityEngine;

public class LightningBoltObject : DamageObject
{
    // TODO: When animation is done playing, delete this object instead of having timer logic
    // We will need the animated sprite/particle effect first

    [SerializeField] private float lifetimeTimer = 0.1f;
    private float _elapsedLifetime = 0.0f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedLifetime += Time.deltaTime;
        if (_elapsedLifetime >= lifetimeTimer)
        {
            Destroy(gameObject);
        }
    }
}
