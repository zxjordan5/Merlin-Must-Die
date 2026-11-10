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
    [SerializeField] private float _orbitSpeed = 1.0f;
    private float _splineLength = 0.0f;


    private GameObject[] _rocks;
    private Vector3[] _rockPositions;
    private Rigidbody2D[] _rockRigidBodies;
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
        _rockPositions = new Vector3[5];
        _rockPositions[0] = new Vector3(-0.35f, 0.29f, 0.05f);
        _rockPositions[1] = new Vector3(0.35f, 0.29f, 0.05f);
        _rockPositions[2] = new Vector3(0, -0.53f, -0.05f);
        _rockPositions[3] = new Vector3(-0.61f, -0.2f, -0.05f);
        _rockPositions[4] = new Vector3(0.61f, -0.2f, -0.05f);

        _splineLength = rockSplinePath.Spline.GetLength();
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

        // TODO: Surround this with state check
        //foreach(Rigidbody2D rb in _rockRB)
        //{
        //    rb.transform.RotateAround(transform.position, Vector3.up, _orbitSpeed * Time.deltaTime);
        //}
        
        // Top Y bound = 0.48
        // Bottom Y bound = -0.54
        // Right X bound = 0.9
        // Left X bound = -0.9
        // Rotate the rocks
        /*
        if (CurrentState != State.Sleeping)
        {
            for (int i = 0; i < _rocks.Length; i++)
            {
                if (_rocks[i].transform.localPosition.x >= 0 && _rocks[i].transform.localPosition.y < 0.053)
                {
                    Debug.Log("movin");
                    _rockRB[i].MovePosition(new Vector2(0.015f + _rockRB[i].transform.position.x, 0.0093f + _rockRB[i].transform.position.y));
                }
                else if (_rocks[i].transform.localPosition.x >= 0 && _rocks[i].transform.localPosition.y > 0.053)
                {
                    _rockRB[i].MovePosition(new Vector2(-0.015f + _rockRB[i].transform.position.x, 0.0075f + _rockRB[i].transform.position.y));
                }
                else if (_rocks[i].transform.localPosition.x <= 0 && _rocks[i].transform.localPosition.y > 0.053)
                {
                    _rockRB[i].MovePosition(new Vector2(-0.015f + _rockRB[i].transform.position.x, -0.0075f + _rockRB[i].transform.position.y));
                }
                else if (_rocks[i].transform.localPosition.x <= 0 && _rocks[i].transform.localPosition.y < 0.053)
                {
                    _rockRB[i].MovePosition(new Vector2(0.015f + _rockRB[i].transform.position.x, -0.0093f + _rockRB[i].transform.position.y));
                }
            }
        }
        */
    }

    private void InitializeRockPositions()
    {
        if (rockSplinePath == null) return;
        for (int i = 0; i < _rockRigidBodies.Length; i++)
        {
            Rigidbody2D rb = _rockRigidBodies[i];
            if (rb == null) continue;
            SplineObject splineObj = rb.GetComponentInParent<SplineObject>();
            if (splineObj == null)
            {
                Debug.LogError("SplineObject component not found on parent of Rigidbody2D");
                continue;
            }
            // Set initial distance along the spline based on the rock's index
            splineObj.DistanceAlongSpline = (i / (float)_rockRigidBodies.Length) * _splineLength;

            // Convert linear distance (0 to splineLength) to a 0-1 parameter t
            float t = splineObj.DistanceAlongSpline / _splineLength;

            // Evaluate position and tangent (forward direction) on the 2D spline (XY plane)
            rockSplinePath.Evaluate(t, out float3 localPosition, out float3 localTangent, out float3 localUp);

            // Convert evaluated values to local space
            Vector2 targetPosition2D = new Vector2(localPosition.x, localPosition.y);

            // Move the kinematic Rigidbody2D safely for physics
            rb.MovePosition(targetPosition2D);
            //_rockPositions[i] = targetPosition2D;
        }
    }

    private void UpdateRockPositions()
    {
        if (rockSplinePath == null) return;

        foreach (Rigidbody2D rb in _rockRigidBodies)
        {
            SplineObject splineObj = rb.GetComponentInParent<SplineObject>();
            if (splineObj == null)
            {
                Debug.LogError("SplineObject component not found on parent of Rigidbody2D");
                continue;
            }

            // Advance the distance along the spline
            splineObj.DistanceAlongSpline += _orbitSpeed * Time.fixedDeltaTime;
            splineObj.DistanceAlongSpline = Mathf.Repeat(splineObj.DistanceAlongSpline, _splineLength);

            // Convert linear distance (0 to splineLength) to a 0-1 parameter t
            float t = splineObj.DistanceAlongSpline / _splineLength;

            // Evaluate position and tangent (forward direction) on the 2D spline (XY plane)
            // Using float 3s here because splines need them I guess? They're basically the same as Vector
            rockSplinePath.Evaluate(t, out float3 localPosition, out float3 localTangent, out float3 localUp);

            // Convert evaluated values to local space
            Vector2 targetPosition2D = new Vector2(localPosition.x, localPosition.y);
            Vector3 targetPosition3D = new Vector3(localPosition.x, localPosition.y, localPosition.z);

            //Vector3 worldTangent = rockSplinePath.transform.TransformDirection(localTangent);
            //float angle = Mathf.Atan2(worldTangent.y, worldTangent.x) * Mathf.Rad2Deg;

            // Move and rotate the kinematic Rigidbody2D safely for physics
            rb.MovePosition(targetPosition3D);
            //rb.MoveRotation(angle);
        }
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
            // Once the player is detected, create the rocks
            for (int i = 0; i < _rocks.Length; i++)
            {
                if (_rocks[i] == null) // Should only be called once, but just in case
                {
                    Debug.Log("Instantiated");
                    _rocks[i] = Instantiate(_rockPrefab, transform, false);
                    _rockRigidBodies[i] = _rocks[i].GetComponent<Rigidbody2D>();     
                }
            }
            InitializeRockPositions();
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

