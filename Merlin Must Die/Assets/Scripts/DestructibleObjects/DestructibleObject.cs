using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Script for destructible objects within the game
/// </summary>
public class DestructibleObject : MonoBehaviour
{
    /// <summary>
    /// Health of the object (varies depending on object)
    /// </summary>
    [SerializeField] protected int health;
    

    /// <summary>
    /// Reference to the prefab of the destroyed object
    /// </summary>
    [SerializeField] protected UnityEngine.Object destructiblePrefabRef;

    /// <summary>
    /// Reference to the particle effect that will play when the object is destroyed
    /// </summary>
    [SerializeField] protected UnityEngine.Object particleExplosionRef;

    /// <summary>
    /// Sprite to use when object is full or mostly full health
    /// </summary>
    [SerializeField] private Sprite fullHealthSprite;

    // Sprite to use when object passes a certain damage threshold
    [SerializeField] private Sprite damagedSprite;

    /// <summary>
    /// SpriteRenderer for the object. We need to reference this to change sprites
    /// </summary>
    private SpriteRenderer _spriteRenderer;
    
    /// <summary>
    /// Initial health of the object
    /// </summary>
    private int _initialHealth;

    void Awake()
    {
        _initialHealth = health;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("DamageSpell"))
        {
            DamageObject dmgObj = other.GetComponent<DamageObject>();
            TakeDamage(dmgObj.Damage);

            if (_spriteRenderer.sprite != damagedSprite && health <= _initialHealth / 2)
            {
                SwitchToDamagedSprite();
            }
            
            if (health <= 0)
            {
                DestroyObject();
            }
        }
    }

    /// <summary>
    /// Subtracts a given damage amount from the current health of the object
    /// </summary>
    /// <param name="damage">The damage being taken</param>
    void TakeDamage(int damage)
    {
        health -= damage;
    }

    /// <summary>
    /// Deletes this object from the scene and instantiates it's destroyed pieces
    /// </summary>
    void DestroyObject()
    {
        GameObject destroyedObject = (GameObject)Instantiate(destructiblePrefabRef, transform.position, transform.rotation);
        GameObject particles = (GameObject)Instantiate(particleExplosionRef, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    /// <summary>
    /// Switches the sprite of the object to its "damaged" variant
    /// </summary>
    void SwitchToDamagedSprite()
    {
        _spriteRenderer.sprite = damagedSprite;
    }
}