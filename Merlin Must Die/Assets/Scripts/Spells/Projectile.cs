using UnityEngine;

/// <summary>
/// Default projectile class with functionality for moving an object at a velocity
/// Will delete the projectile object after "lifetime" amount of time has passed.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] float initialVelocity = 1;


    /// <summary>
    /// Time until entire projectile object is deleted
    /// Does not delete if set to 0
    /// </summary>
    [SerializeField] protected float lifetime;
    protected float lifetimeTimer;


    public float Velocity
    {
        get; set;
    } = 0;
    public Vector2 Direction
    {
        get; set;
    } = new Vector2();


    //Move projectile with velocity and direction
    // Using transform.Translate currently instead of simulating forces

    protected void Awake()
    {
        if(lifetime == 0)
        {
            lifetime = 999;
        }
    }

    protected void FixedUpdate()
    {
        gameObject.transform.Translate(Velocity * Time.deltaTime * Direction);
    }

    protected void Update()
    {
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer > lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void LaunchProjectile(Vector2 direction)
    {
        Velocity = initialVelocity;
        Direction = direction;
    }

}
