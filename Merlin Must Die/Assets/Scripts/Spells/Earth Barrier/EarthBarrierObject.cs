using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

/// <summary>
/// Fireball projectile that on collision with an enemy or destructible object will explode
/// </summary>
public class EarthBarrierObject : DamageObject
{
    
    [SerializeField] protected AudioClip landSound;
    [SerializeField] protected ParticleSystem landingParticles;

    /// <summary>
    /// Reference to the collider to be disabled while the object is in the air
    /// </summary>
    private Collider2D _collider;
    private Projectile _projectile;

    /// <summary>
    /// How long the object should stay in the air before it lands
    /// </summary>
    [SerializeField] private float _airTime = 2.0f;
    private float _airTimeCounter = 0;
    private bool _landed = false;

    /// <summary>
    /// How far in front of other objects should the object be while it is in the air
    /// </summary>
    private float _zIncrease = 5f;

    public void Awake()
    {
        _airTimeCounter = 0;
        _landed = false;

        //Move the object above other objects
        transform.Translate(0.0f, 0.0f, -_zIncrease);
       
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;

        _projectile = GetComponent<Projectile>();
    }

    public void Update()
    {
        //Track how long until the object lands
        if (_airTimeCounter < _airTime)
        {
            _airTimeCounter += Time.deltaTime;
        }

        if (_airTimeCounter >= _airTime && !_landed)
        {
            Land();
        }
    }

    /// <summary>
    /// When the object lands, it stops moving and plays all effects.
    /// </summary>
    private void Land()
    {
        _landed = true;

        //Reset the object's forward transform above other objects
        transform.Translate(0.0f, 0.0f, _zIncrease);

        _projectile.Velocity = 0;

        _collider.enabled = true;
        AudioSource.PlayClipAtPoint(landSound, transform.position);
        Instantiate(landingParticles, transform.position, transform.rotation);
    }
}
