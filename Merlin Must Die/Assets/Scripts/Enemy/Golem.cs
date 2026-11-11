using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.U2D;
using Unity.Mathematics;

public class Golem : Wander_Enemy
{
    [SerializeField] private float _aimTime = 1.0f;
    [SerializeField] private float _reloadTime = 2.0f;

    [SerializeField] private GameObject _rockPrefab;

    [SerializeField] private SplineContainer rockSplinePath;

    /// <summary>
    /// The orbit speed of the Golem's rocks around their rotation path.
    /// </summary>
    [SerializeField] private float _orbitSpeed = 0.1f;
    private float _splineLength = 0.0f;
    private const int SplineSampleCount = 512;
    private float[] _splineParameters;
    private float[] _splineDistances;


    private GameObject[] _rocks;
    private Rigidbody2D[] _rockRigidBodies;
    private SplineObject[] _rockSplineObjects;
    private Animator _animator;

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

        // Make the enemy's decection circle
        _detectionCircle = new GameObject("RockDetection");
        //_detectionCircle.transform.SetParent(transform);
        _detectionCircle.transform.position = _rb.transform.position;
        CircleCollider2D _collider = _detectionCircle.AddComponent<CircleCollider2D>();
        _collider.radius = 5f;
        _collider.isTrigger = true;

        _initialHealth = 4;
        _health = _initialHealth;

        // Set up variables for the golem's projectiles
        _rocks = new GameObject[5];
        _rockRigidBodies = new Rigidbody2D[5];
        _rockSplineObjects = new SplineObject[5];
        CacheSplineArcLength();
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
        //_animator.SetInteger("State", (int)MoveState);
        
        if (Detection() && CurrentState == State.Sleeping)
        {
            Debug.Log("Player Detected");
            CurrentState = State.Attacking;
            
        }

        _state = (int)CurrentState;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Once the player is detected, the enemy allways goes towards their location to attack
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

        for (int i = 0; i < _rockRigidBodies.Length; i++)
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
        // If the player gets too close, the golem stomps
        if (Vector2.Distance(_player.transform.position, _rb.transform.position) < 2)
        {
            CurrentState = State.Slamming;
            _target = _player.transform.position;
        }

        // If the player gets close enough, the golem stops, aims for a sec, then fires a rock towards the players POS
        else if (Vector2.Distance(_player.transform.position, _rb.transform.position) < 4)
        {
            _rb.linearVelocity = new Vector2(0,0);
            _target = _player.transform.position;
            StartCoroutine("AimTimer");
        }
    }

    protected override bool Detection()
    {
        // If the player's position overlaps with a point on the detection circle's collider or the enemy gets hit, attack
        if (_detectionCircle.GetComponent<CircleCollider2D>().OverlapPoint(_player.transform.position) || _initialHealth != _health)  
        {
            if (rockSplinePath == null || _splineLength <= 0f || _rockPrefab == null)
                return false;

            // Once the player is detected, create the rocks
            for (int i = 0; i < _rocks.Length; i++)
            {
                if (!_rocks[i]) // Should only be called once, but just in case
                {
                    float distance = (i / (float)_rocks.Length) * _splineLength;
                    Vector2 position = EvaluateSplinePosition(distance);
                    GameObject rock = Instantiate(_rockPrefab, position, Quaternion.identity);
                    Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
                    SplineObject splineObject = rock.GetComponent<SplineObject>();
                    if (!rb || !splineObject)
                    {
                        // If either the rigidbody or spline object is null
                        Debug.LogError("Each rock prefab must have a Rigidbody2D and SplineObject component.", rock);
                        continue;
                    }

                    _rocks[i] = rock;
                    _rockRigidBodies[i] = rb;
                    _rockSplineObjects[i] = splineObject;
                    splineObject.DistanceAlongSpline = distance;
                    rb.position = position;
                }
            }
            return true;
        }
        return false;
    }

    protected override void TakeDamage(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<DamageObject>() && CurrentState != State.Sleeping)
        {
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

    IEnumerator AimTimer()
    {
        CurrentState = State.Aiming;
        yield return new WaitForSeconds(_aimTime);
        _target = _player.transform.position + (_player.transform.position - _rb.transform.position)*2;
        CurrentState = State.Shooting;
        yield return new WaitForSeconds(_reloadTime);
        CurrentState = State.Attacking;
        _target = new Vector2(1,1);
        //_attackCooldownTimer = 0;
    }

}


//using UnityEngine;
//using UnityEngine.Splines; // Requires the Unity Splines package

//[RequireComponent(typeof(Rigidbody2D))]
//public class KinematicSplineMover2D : MonoBehaviour
//{
//    [SerializeField] private SplineContainer splineContainer;
//    [SerializeField] private float moveSpeed = 5f; // Units per second
//    [SerializeField] private bool loop = true;

//    private Rigidbody2D rb;
//    private float distanceAlongSpline = 0f;
//    private float splineLength = 0f;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();

//        if (splineContainer != null)
//        {
//            splineLength = splineContainer.Spline.GetLength();
//        }
//    }

//    void FixedUpdate()
//    {
//        if (splineContainer == null || splineLength <= 0f) return;

//        // Advance distance based on speed and fixed time
//        distanceAlongSpline += moveSpeed * Time.fixedDeltaTime;

//        if (loop)
//        {
//            distanceAlongSpline = Mathf.Repeat(distanceAlongSpline, splineLength);
//        }
//        else
//        {
//            distanceAlongSpline = Mathf.Clamp(distanceAlongSpline, 0f, splineLength);
//        }

//        // Convert linear distance (0 to splineLength) to a 0-1 parameter t
//        float t = distanceAlongSpline / splineLength;

//        // Evaluate position and tangent (forward direction) on the 2D spline (XY plane)
//        splineContainer.Evaluate(t, out Vector3 localPosition, out Vector3 localTangent, out Vector3 localUp);

//        // Convert local spline position/tangent to world space
//        Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);
//        Vector2 targetPosition2D = new Vector2(worldPosition.x, worldPosition.y);

//        Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent);
//        float angle = Mathf.Atan2(worldTangent.y, worldTangent.x) * Mathf.Rad2Deg;

//        // Move and rotate the kinematic Rigidbody2D safely for physics
//        rb.MovePosition(targetPosition2D);
//        rb.MoveRotation(angle);
//    }
//}
