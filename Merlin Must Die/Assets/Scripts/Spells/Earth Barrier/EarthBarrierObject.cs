using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Fireball projectile that on collision with an enemy or destructible object will explode
/// </summary>
public class EarthBarrierObject : SpellObject
{
    
    /// <summary>
    /// How long the object should stay in the air before it lands
    /// </summary>
    [SerializeField] private float _airTime = 2.0f; 
    public float AirTime { get { return _airTime; } }
    private float _airTimeCounter = 0;


    
    //Sprites
    SpriteRenderer _spriteRenderer;

    [Header("Object Setup")]
    [SerializeField] private SpriteRenderer _inAirSprite;
    [SerializeField] private GameObject _holeObject;
    
    /// <summary>
    /// Reference to the collider to be disabled while the object is in the air
    /// </summary>
    private Collider2D _collider;
    [SerializeField] private Collider2D _damageCollider;
    private Projectile _projectile;

    private bool _landed = false;

    //Used to determine if the barrier should be sinking
    private bool _overWater = false;
    private bool _sinking = false;

    /// <summary>
    /// How far in front of other objects should the object be while it is in the air
    /// </summary>
    private float _zIncrease = 5f;

    /// <summary>
    /// The starting scale of the object is saved so its scale can be modified non-destructively
    /// </summary>
    private Vector3 _baseScale;

  

    [Header("Sounds & Effects")]
    [SerializeField] protected AudioClip landSound;
    [SerializeField] protected ParticleSystem landingParticles;
    [SerializeField] protected AudioClip splashSound;
    [SerializeField] protected ParticleSystem splashParticles;


  

    public override void Awake()
    {
        base.Awake();

        _baseScale = transform.localScale;

        _airTimeCounter = 0;
        _landed = false;

        //Move the object above other objects
        transform.Translate(0.0f, 0.0f, -_zIncrease);
       
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;

        _projectile = GetComponent<Projectile>();

        Physics2D.IgnoreCollision(_collider, _damageCollider);
        _damageCollider.enabled = false;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
        _inAirSprite.enabled = true;

        GameObject hole = Instantiate(_holeObject, new Vector3(transform.position.x, transform.position.y - 0.2f, 1), transform.rotation);
    }

    public void Update()
    {
        //Track how long until the object lands
        if (_airTimeCounter < _airTime)
        {
            _airTimeCounter += Time.deltaTime;

            //Scale the object based on how long it has been in the air
            if(_airTimeCounter < _airTime / 2f) //First half - grow
            {
                transform.localScale = _baseScale * (1 + _airTimeCounter / _airTime);
            }
            else //Second half - return to normal size
            {
                transform.localScale = _baseScale * (1 + (_airTime - _airTimeCounter) / _airTime);
            }
        }

        if (_airTimeCounter >= _airTime && !_landed)
        {
            Land();
        }

        _sinking = _overWater && _landed;

        if (_sinking)
        {
            Color newColor = _spriteRenderer.color;
            newColor.a -= Time.deltaTime;
            _spriteRenderer.color = newColor;

            if(newColor.a <= 0)
            {
                Destroy(this);
            }
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

        _projectile.Velocity = 0; //Stop the earth barrier

        _collider.enabled = true; //Re-enable collision

        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D
        {
            useTriggers = true
        };
        _collider.Overlap(filter, results);

        foreach (Collider2D collider in results)
        {
            if (LayerMask.LayerToName(collider.gameObject.layer) == "Water")
            {
                _overWater = true;
                break;
            }
        }

        if (!_overWater)
        {
            AudioSource.PlayClipAtPoint(landSound, transform.position);
            Instantiate(landingParticles, transform.position, transform.rotation);
        }
        else
        {
            AudioSource.PlayClipAtPoint(splashSound, transform.position);
            Instantiate(splashParticles, transform.position, transform.rotation);
        }

        //Enable damage collider to cause damage at impact
        _damageCollider.enabled = true;
        StartCoroutine("DisableDamageCollider");

        _spriteRenderer.enabled = true;
        _inAirSprite.enabled = false;
        
    }

    private IEnumerator DisableDamageCollider()
    {
        yield return new WaitForSeconds(0.3f);
        _damageCollider.enabled = false;
    }
}
