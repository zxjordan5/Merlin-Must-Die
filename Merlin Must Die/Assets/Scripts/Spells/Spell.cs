using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    //Cooldown field and timer
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;


    [SerializeField] public List<Grimoire.Direction> spellCode;
    [SerializeField] public GameObject spellObj;

    [SerializeField] public AudioClip castSound;


    //Returns false if spellTimer is above cooldown
    public bool OnCooldown{
        get
        {
            return cooldownTimer <= cooldown;
        }
    }

    //Individual timer for the spells cooldown
    protected virtual void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    //Cast method to be implemented by every spell
    public abstract void Cast(Player player);

    public void ResetCooldown()
    {
        cooldownTimer = 0;
    }

}
