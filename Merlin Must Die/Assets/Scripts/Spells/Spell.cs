using System;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class Spell : MonoBehaviour
{
    //Cooldown field and timer
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    [CreateProperty]

    public float CooldownPercent
    {
        get
        {
            if(cooldownTimer > cooldown)
            {
                return 100;
            }
            return 100 * (cooldownTimer / cooldown);
        }
    }


    [SerializeField] public List<Grimoire.Direction> spellCode;

    [SerializeField] public GameObject spellObj;

    [SerializeField] public AudioClip castSound;

    [SerializeField]
    protected String spellName = "";

    public string SpellName
    {
        get => spellName;
    }

    public void Awake()
    {
        // Off cooldown on startup
        cooldownTimer = cooldown;
    }
    
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
    public virtual void Cast(Player player)
    {
        ResetCooldown();

        AudioSource.PlayClipAtPoint(castSound, player.transform.position);
    }

    public void ReduceCooldownPercent(float percentage)
    {
        cooldownTimer += percentage * (cooldown / 100);
    }
    public float GetCooldown()
    {
        return cooldownTimer;
    }
    public void ResetCooldown()
    {
        cooldownTimer = 0;
    }

}
