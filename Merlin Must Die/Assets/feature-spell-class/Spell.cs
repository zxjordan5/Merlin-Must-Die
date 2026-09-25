using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    [SerializeField] public float cooldown;
    [SerializeField] public SpellCode spellCode;
    [SerializeField] public GameObject spellObj;

    //Cast method to be overridden by every spell
    public virtual void Cast(Player player)
    {
        
    }

}
