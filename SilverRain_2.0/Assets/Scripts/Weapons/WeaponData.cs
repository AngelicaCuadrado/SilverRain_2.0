using UnityEngine;
/// <summary>
/// Represents configuration data for a weapon, including its base attributes and per-level scaling values.
/// </summary>
/// <remarks>This class is intended to be used as a Unity ScriptableObject asset for defining weapon
/// characteristics in a game. It provides fields for both base values and incremental changes per level, allowing for
/// flexible weapon progression and balancing. WeaponData assets can be created via the Unity Editor using the
/// 'Data/Weapon Data' menu option.</remarks>
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    //Base weapon attributes
    public float baseDamage;
    public float baseCooldown;
    public float baseDuration;
    public float baseProjectileSpeed;
    public float baseSize;

    //Per level scaling attributes
    public float perLevelDamage;
    public float perLevelCooldown;
    public float perLevelDuration;
    public float perLevelProjectileSpeed;
    public float perLevelSize;
}