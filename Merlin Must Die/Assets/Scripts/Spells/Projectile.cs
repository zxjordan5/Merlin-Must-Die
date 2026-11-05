using UnityEngine;

/// <summary>
/// Default projectile class with functionality for moving an object at a velocity
/// Will delete the projectile object after "lifetime" amount of time has passed.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _initialVelocity = 1;
   
    /// <summary>
    /// Limit on the objects velocity
    /// Will not work if set to 0
    /// </summary>
    [SerializeField] private float _maxVelocity = 0;

    private Rigidbody2D _rb;

    /// <summary>
    /// Whether or not the initial player velocity field should be used
    /// in calculations of the projectiles speed
    /// </summary>
    private bool _usePlayerVelocity = false;
    private float _initialPlayerVelocity;

    /// <summary>
    /// Whether or not the entire game object should be rotated based on the players aim direction
    /// </summary>
    [SerializeField] private bool _rotateSpell;
    

    /// <summary>
    /// Time until entire projectile object is deleted
    /// Does not delete if set to 0
    /// </summary>
    [SerializeField] private float _lifetime;
    private float _lifetimeTimer;

    public float Velocity
    {
        get; set;
    } = 0;
    public Vector2 Direction
    {
        get; set;
    } = new Vector2();

   
    protected void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if(_lifetime == 0)
        {
            _lifetime = 9999;
        }
    }

    protected void FixedUpdate()
    {
        float tempVelocity = Velocity;

        if (_usePlayerVelocity)
        {
            tempVelocity *= _initialPlayerVelocity;
        }

        //Cap velocity
        if (_maxVelocity != 0 && tempVelocity > _maxVelocity)
        {
            tempVelocity = _maxVelocity;
        }

        Vector3 positionUpdate = tempVelocity * Time.deltaTime * Direction;

        gameObject.transform.Translate(positionUpdate);
    }

    protected void Update()
    {
        _lifetimeTimer += Time.deltaTime;
        if (_lifetimeTimer > _lifetime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Starts moving the projectile based on it's starting velocity
    /// and adds a direction for the projectile to move
    /// </summary>
    /// <param name="direction"></param>
    public void LaunchProjectile(Vector2 direction)
    {
        Velocity = _initialVelocity;
        Direction = Vector2.Normalize(direction);

        //Rotates the object instead of the projectile
        if (_rotateSpell)
        {
            transform.eulerAngles = new Vector3(0, 0, Globals.GetAngleInDegFromVec(direction));
            Direction = Vector2.right;
        }
        
        //Potential for using rigidbodies
        //_rb.AddForce(Velocity * 50 * Direction);
    }

    public void UsePlayerVelocity(float playerVelocity) {
        _usePlayerVelocity = true;
        _initialPlayerVelocity = playerVelocity;
    }
}