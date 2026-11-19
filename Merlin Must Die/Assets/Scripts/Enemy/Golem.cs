using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.U2D;
using Unity.Mathematics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Golem : Wander_Enemy
{
    [SerializeField] private float aimTime = 1.0f;
    [SerializeField] private float reloadTime = 2.0f;
    [SerializeField] private float slamColliderRadius = 1.0f;
    [SerializeField] private float slamTriggerRadius = 2.0f;
    [SerializeField] private float slamCooldown = 2.0f;

    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private ParticleSystem slamParticles;

    [SerializeField] private SplineContainer rockSplinePath;

    [SerializeField] private int initialRockAmount = 5;

    /// <summary>
    /// The orbit speed of the Golem's rocks around their rotation path.
    /// </summary>
    [SerializeField] private float _orbitSpeed = 0.1f;
    private float _splineLength = 0.0f;
    private const int SplineSampleCount = 512;
    private float[] _splineParameters;
    private float[] _splineDistances;


    private List<GameObject> _rocks;
    private List<Rigidbody2D> _rockRigidBodies;
    private List<SplineObject> _rockSplineObjects;
    private CircleCollider2D _slamCollider;
    private Animator _animator;

    /// <summary>
    /// Simple state check to see if the initial set of rocks have been spawned
    /// </summary>
    private bool _initialRocksSpawned = false;

    public enum State{ Sleeping = 0, Attacking = 1, Aiming = 2, Shooting = 3, Slamming = 4, Dead = 5 }
    public State CurrentState { get; private set; } = State.Sleeping;
    [SerializeField] private int _state = 0;
    public State MoveState
    {
        get
        {
            /*
            if (CurrentState == State.Charging)
                return State.Charging;
            if (CurrentState == State.Rolling)
                return State.Rolling;

            if (CurrentState == State.Attacking)
                return State.Attacking;
            */
            return State.Sleeping; 
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();
        _target = new Vector2(1,1);

        // Make the enemy's detection circle
        _detectionCircle = new GameObject("RockDetection");
        _detectionCircle.transform.position = _rb.transform.position;
        CircleCollider2D _collider = _detectionCircle.AddComponent<CircleCollider2D>();
        _collider.radius = detectionRadius;
        _collider.isTrigger = true;

        _initialHealth = 4;
        _health = _initialHealth;

        // Set up fields for the golem's projectiles
        _rocks = new List<GameObject>(initialRockAmount);
        _rockRigidBodies = new List<Rigidbody2D>(initialRockAmount);
        _rockSplineObjects = new List<SplineObject>(initialRockAmount);
        CacheSplineArcLength();

        // Set up fields for slam attacks
        _slamCollider = this.AddComponent<CircleCollider2D>();
        _slamCollider.radius = slamColliderRadius;
        _slamCollider.isTrigger = true;
        _slamCollider.enabled = false;
    }

    protected void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // Forces the material to explicitly bind the texture to the URP stencil block
            spriteRenderer.material.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        }
    }

    protected void Update()
    {
        if (Detection() && CurrentState == State.Sleeping)
        {
            CurrentState = State.Attacking;
        }

        _state = (int)CurrentState;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Once the player is detected, the enemy always goes towards their location to attack
        if (CurrentState == State.Attacking && _target == new Vector2(1,1))
        {
            Attack();
            _rb.transform.position = Vector2.MoveTowards(
                _rb.transform.position,
                _player.transform.position,
                0.5f * Time.deltaTime);
        }

        // The parent sprite flipping, also flips the rocks completely 
        if(transform.localScale.x < 0)
        {
            // (this stops the flipping)
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }


        if (CurrentState != State.Sleeping)
        {
            UpdateRockPositions();
        }
    }

    private void CacheSplineArcLength()
    {
        if (rockSplinePath == null)
        {
            Debug.LogError("Rock spline path is not assigned.", this);
            return;
        }

        _splineParameters = new float[SplineSampleCount + 1];
        _splineDistances = new float[SplineSampleCount + 1];

        rockSplinePath.Evaluate(0f, out float3 previousPosition, out _, out _);
        for (int i = 1; i <= SplineSampleCount; i++)
        {
            float t = i / (float)SplineSampleCount;
            rockSplinePath.Evaluate(t, out float3 position, out _, out _);
            Vector3 previous = new Vector3(previousPosition.x, previousPosition.y, previousPosition.z);
            Vector3 current = new Vector3(position.x, position.y, position.z);
            _splineDistances[i] = _splineDistances[i - 1] + Vector3.Distance(previous, current);
            _splineParameters[i] = t;
            previousPosition = position;
        }

        _splineLength = _splineDistances[SplineSampleCount];
    }

    private void UpdateRockPositions()
    {
        if (!rockSplinePath || _splineLength <= 0f) return;

        for (int i = 0; i < _rockRigidBodies.Count; i++)
        {
            Rigidbody2D rb = _rockRigidBodies[i];
            SplineObject splineObj = _rockSplineObjects[i];
            if (rb == null || splineObj == null) continue;

            splineObj.DistanceAlongSpline += _orbitSpeed * Time.fixedDeltaTime;
            splineObj.DistanceAlongSpline = Mathf.Repeat(splineObj.DistanceAlongSpline, _splineLength);

            rb.MovePosition(EvaluateSplinePosition(splineObj.DistanceAlongSpline));
        }
    }

    private Vector2 EvaluateSplinePosition(float distance)
    {
        float targetDistance = Mathf.Repeat(distance, _splineLength);
        int low = 0;
        int high = _splineDistances.Length - 1;
        while (low < high)
        {
            int middle = (low + high) / 2;
            if (_splineDistances[middle] < targetDistance)
                low = middle + 1;
            else
                high = middle;
        }

        int upperIndex = Mathf.Clamp(low, 1, _splineDistances.Length - 1);
        float lowerDistance = _splineDistances[upperIndex - 1];
        float upperDistance = _splineDistances[upperIndex];
        float segmentLength = upperDistance - lowerDistance;
        float fraction = segmentLength > 0f
            ? (targetDistance - lowerDistance) / segmentLength
            : 0f;
        float t = Mathf.Lerp(_splineParameters[upperIndex - 1], _splineParameters[upperIndex], fraction);

        rockSplinePath.Evaluate(t, out float3 position, out _, out _);
        return new Vector3(position.x, position.y, position.z);
    }

    protected override void Attack()
    {
        // Golem default behavior (has rocks)
        if(_rocks.Count > 0)
        {
            // If the player gets too close, the golem stomps
            if (Vector2.Distance(_player.transform.position, _rb.transform.position) < slamTriggerRadius)
            {
                _target = _player.transform.position;
                StartCoroutine("Slam");
            }

            // If the player gets close enough, the golem stops, aims for a sec, then fires a rock towards the players POS
            else if (Vector2.Distance(_player.transform.position, _rb.transform.position) < 4)
            {
                _rb.linearVelocity = new Vector2(0, 0);
                StartCoroutine("AimAndLaunchRock");
            }
        }
        else
        {
            // If the golem is out of rocks, just check to see if it's close enough to slam
            // Desired behavior is for the golem to chase the player until they can hit them
            // If the player gets too close, the golem stomps
            if (Vector2.Distance(_player.transform.position, _rb.transform.position) < slamTriggerRadius)
            {
                _target = _player.transform.position;
                StartCoroutine("Slam");
            }
        }
        
    }

    protected override bool Detection()
    {
        // If the player's position overlaps with a point on the detection circle's collider or the enemy gets hit, attack
        if (_detectionCircle.GetComponent<CircleCollider2D>().OverlapPoint(_player.transform.position) || _initialHealth != _health)  
        {
            if (rockSplinePath == null || _splineLength <= 0f || rockPrefab == null)
                return false;

            // Once the player is detected, create the rocks
            if(!_initialRocksSpawned)
            {
                CreateRocks();
                _initialRocksSpawned = true;
            }
            return true;
        }
        return false;
    }

    private void CreateRocks()
    {
        for (int i = 0; i < initialRockAmount; i++)
        {
            float distance = (i / (float)initialRockAmount) * _splineLength;
            Vector2 position = EvaluateSplinePosition(distance);
            GameObject rock = Instantiate(rockPrefab, position, Quaternion.identity);
            Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
            SplineObject splineObject = rock.GetComponent<SplineObject>();
            if (!rb || !splineObject)
            {
                // If either the rigidbody or spline object is null
                Debug.LogError("Each rock prefab must have a Rigidbody2D and SplineObject component.", rock);
                continue;
            }

            _rocks.Add(rock);
            _rockRigidBodies.Add(rb);
            _rockSplineObjects.Add(splineObject);
            splineObject.DistanceAlongSpline = distance;
            rb.position = position;
        }
    }

    protected override void TakeDamage(Collider2D collision)
    { 
        if (!collision.gameObject.GetComponent<DamageObject>() || CurrentState == State.Sleeping)
        {
            // Not a damage object or we're sleeping, don't do anything
            return;
        }

        // If the golem has rocks still, destroy one of the rocks
        if (_rocks.Count > 0)
        {
            DestructibleObject destroyedRock = _rocks[_rocks.Count - 1].GetComponent<DestructibleObject>();
            if(destroyedRock)
            {
                ClearRockAtIndex(_rocks.Count - 1);
                destroyedRock.DestroyObject();
            }
        }
        else
        {
            // If the golem has no rocks, it takes damage
            _health -= collision.gameObject.GetComponent<DamageObject>().Damage;
        }

        if (_health <= 0)
        {
            CurrentState = State.Dead;
            Destroy(gameObject);
        }
    }

    protected override void Wander()
    {
        // Stops the golem from wandering
        _rb.linearVelocity = new Vector2(0,0);
    }

    #region RockLaunchingMethods
    IEnumerator AimAndLaunchRock()
    {
        CurrentState = State.Aiming;
        yield return new WaitForSeconds(aimTime);
        CurrentState = State.Shooting;

        LaunchRock();

        yield return new WaitForSeconds(reloadTime);
        CurrentState = State.Attacking;
        _target = new Vector2(1,1);
        //_attackCooldownTimer = 0;
    }

    private void LaunchRock()
    {
        // No rocks left to launch
        if(_rocks.Count <= 0)
        {
            return;
        }

        GameObject rockToLaunch = _rocks[_rocks.Count-1];
        _target = _player.transform.position - rockToLaunch.transform.position;
        Projectile rockProj = rockToLaunch.GetComponent<Projectile>();
        EarthBarrierObject rockEBO = rockToLaunch.GetComponent<EarthBarrierObject>();

        // Remove game object from rock list
        ClearRockAtIndex(_rocks.Count - 1);

        // Enable scripts on the rock projectile
        rockProj.enabled = true;
        rockEBO.enabled = true;

        // Launch rock projectile
        rockProj.LaunchProjectile(_target);
        CurrentState = State.Attacking;
    }

    private void ClearRockAtIndex(int index)
    {
        _rocks.RemoveAt(index);
        _rockRigidBodies.RemoveAt(index);
        _rockSplineObjects.RemoveAt(index);
    }
    #endregion

    private IEnumerator Slam()
    {
        CurrentState = State.Slamming;
        // Prevent slams while this coroutine is running
        //_canSlam = false;

        // TODO: We will want to have functionality to play the slam
        // animation. When the startup is done and the golem slams,
        // enable the collider and play the particle effects and all that stuff.
        // For now, we are going to spawn the particle effects and enable the collider
        _rb.linearVelocity = Vector2.zero;
        _slamCollider.enabled = true;
        ParticleSystem p = Instantiate(slamParticles, transform.position, transform.rotation);
        // Make sure the size of the particles scales dynamically to the collider size
        p.transform.localScale *= slamColliderRadius;

        // Wait until particle effect is finished
        while (p != null && p.IsAlive())
        {
            yield return null;
        }

        _slamCollider.enabled = false;

        yield return new WaitForSeconds(slamCooldown);
        CurrentState = State.Attacking;
        _target = new Vector2(1, 1);
    }
}
