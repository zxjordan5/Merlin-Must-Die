using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BackInTheStone : ExcaliburAbility
{
    private GameObject _initialAoeObjRef;
    private GameObject _damageFieldObjRef;

    private GameObject _initialAoeInstance;
    private GameObject _damageFieldInstance;

    private Player _player;

    /// <summary>
    /// The duration of the initial AOE damage effect (should be a very small number)
    /// </summary>
    [SerializeField] private float initialAoeDuration = 0.1f;
    
    /// <summary>
    /// The duration of the damage field effect
    /// </summary>
    [SerializeField] private float damageFieldDuration = 4.0f;

    /// <summary>
    /// How long the damage field has been active in the scene
    /// </summary>
    private float _damageFieldTimer = 0.0f;

    private Animator _initialAoeAnimator;

    private void Start()
    {
        _player = FindAnyObjectByType<Player>();
        StartCoroutine(LoadDamageObjects());
    }

    private void Update()
    {
        // TODO: If the GO instances aren't null, delete them after a time
        if (_initialAoeInstance != null)
        {
            AnimatorStateInfo stateInfo = _initialAoeAnimator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("BackInTheStone_InitialAoe"))
            {
                // Lightning bolt animation is not playing, play it
                _initialAoeAnimator.Play("BackInTheStone_InitialAoe");
            }
            else if (stateInfo.normalizedTime >= 1.0f)
            {
                // Animation is done playing, time to spawn damage over time field
                _damageFieldInstance = Instantiate(_damageFieldObjRef, _initialAoeInstance.transform.position, Quaternion.identity);
                Destroy(_initialAoeInstance.gameObject);
                _initialAoeInstance = null;
            }
        }

        if (_damageFieldInstance != null)
        {
            _damageFieldTimer += Time.deltaTime;
            if (_damageFieldTimer >= damageFieldDuration)
            {
                _damageFieldTimer = 0.0f;
                Destroy(_damageFieldInstance.gameObject);
                _damageFieldInstance = null;
            }
        }
    }
    
    public override void Activate()
    {
         Debug.Log("Ability activated!");
        // Spawn initial AOE then delete, then spawn damage field in the same location as the AOE
        // (Additional spawning logic handled in Update())
        _initialAoeInstance = Instantiate(_initialAoeObjRef, _player.transform.position, Quaternion.identity);
        _initialAoeAnimator = _initialAoeInstance.GetComponent<Animator>();
    }

    IEnumerator LoadDamageObjects()
    {
        AsyncOperationHandle<GameObject> handleDamageField = Addressables.LoadAssetAsync<GameObject>("BackInTheStone_DamageField");
        yield return handleDamageField;
        if(handleDamageField.Status == AsyncOperationStatus.Succeeded)
        {
            // FOR TESTING
            //Instantiate(handleDamageField.Result.gameObject, _player.transform.position, Quaternion.identity);
            
            _damageFieldObjRef = handleDamageField.Result.gameObject;
        }
        
        AsyncOperationHandle<GameObject> handleInitialAoe = Addressables.LoadAssetAsync<GameObject>("BackInTheStone_InitialAOE");
        yield return handleInitialAoe;
        if (handleInitialAoe.Status == AsyncOperationStatus.Succeeded)
        {
            _initialAoeObjRef = handleInitialAoe.Result.gameObject;
        }
    }
}
