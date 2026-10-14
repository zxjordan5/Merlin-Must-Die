using UnityEngine;

/// <summary>
/// Class to represent any object within the game that can deal damage to other entities. 
/// This script MUST be attached to any object that meets that criteria.
/// </summary>
public class DamageObject : MonoBehaviour
{
    /// <summary>
    /// The amount of damage this object can deal
    /// </summary>
    [SerializeField] private int damage;

    public int Damage { get => damage; private set => damage = value; }
}
