using System.Collections;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<DamageObject>())
        {
            Debug.Log("explode");
            Instantiate(explosion,  transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
