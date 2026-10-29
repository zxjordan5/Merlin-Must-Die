using System.Collections;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SanguineCommunionAbility : ExcaliburAbility
{
    private Player _player;
    private GameObject _sanguineCommunionObjectRef;
    
    private void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        StartCoroutine(LoadDamageObjects());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Activate()
    {
        // Instantiate SanguineCommunion object
        Quaternion objQuat = Quaternion.Euler(0, 0, _player.AimAngle);
        
        // TODO: Make projectile move along rotated angle
        GameObject fbObj = Instantiate(_sanguineCommunionObjectRef, _player.transform.position, objQuat);
        Projectile fbProjectile = fbObj.GetComponent<Projectile>();
        fbProjectile.LaunchProjectile(_player.AimDirNorm);
    }

    IEnumerator LoadDamageObjects()
    {
        AsyncOperationHandle<GameObject> handleSanguineCommunionObject = Addressables.LoadAssetAsync<GameObject>("SanguineCommunion_Object");
        yield return handleSanguineCommunionObject;
        if(handleSanguineCommunionObject.Status == AsyncOperationStatus.Succeeded)
        {
            _sanguineCommunionObjectRef = handleSanguineCommunionObject.Result.gameObject;
        }
    }
}
