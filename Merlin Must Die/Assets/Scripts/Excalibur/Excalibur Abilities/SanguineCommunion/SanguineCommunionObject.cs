using UnityEngine;

public class SanguineCommunionObject : DamageObject
{
    /// <summary>
    /// Reference to the player in the scene
    /// </summary>
    private Player _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = FindAnyObjectByType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
           // TODO: Heal player and explode object
           _player.Heal();
        }
        // Will explode on collision objects in the environment, enemies, and destructible objects. Will not explode on collision with the player.
        if (other.gameObject.CompareTag("EnvironmentObject") || other.gameObject.CompareTag("Destructible"))
        {
            //Explode();
        }
    }
}
