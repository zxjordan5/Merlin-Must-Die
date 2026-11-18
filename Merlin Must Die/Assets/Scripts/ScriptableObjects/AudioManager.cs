using UnityEngine;

[CreateAssetMenu(fileName = "LevelScenePaths", menuName = "Scriptable Objects/AudioPaths")]
public class AudioManager : ScriptableObject
{
    #region Spell Audio
    public const string dashCast = "Assets/Audio/Spells/Dash/Dash";
    public const string witchboltCast = "Assets/Audio/Spells/Witchbolt/Fireball";
    public const string earthBarrierCast = "Assets/Audio/Spells/Earth_Barrier/Earth_Cast";
    public const string earthBarrierLand = "Assets/Audio/Spells/Earth_Barrier/Earth_Land";
    public const string tidalWaveCast = "Assets/Audio/Spells/Tidal_Wave/Uncut-Tidal-wave";
    public const string lightningBoltCast = "Assets/Audio/Spells/Lightning_Bolt/LightningBoltCast";
    #endregion
    #region Player Audio
    public const string spellInput = "Assets/Audio/Spells/Player/Spell_input";
    public const string spellClear = "Assets/Audio/Spells/Player/Spell_Clear_2";
    #endregion
}
