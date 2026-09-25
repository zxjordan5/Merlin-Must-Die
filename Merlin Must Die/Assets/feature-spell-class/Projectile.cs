using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 Velocity
    {
        get; set;
    }

    void Update()
    {
        gameObject.transform.Translate(Velocity * Time.deltaTime);
    }
}
