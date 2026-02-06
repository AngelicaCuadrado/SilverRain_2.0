using System.Data;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [SerializeField, Tooltip("The weapon controller that is attached to the same weapon object")]
    private Weapon weapon;
    [SerializeField, Tooltip("The scriptable object holding the default, and per-level properties")]
    private WeaponData weaponData;

    //Cached stats
    [SerializeField, Tooltip("How much damage the weapon deals")]
    private float damage;
    [SerializeField, Tooltip("How long the weapon waits before attacking again")]
    private float cooldown;
    [SerializeField, Tooltip("How long the weapon stays active during attacks")]
    private float duration;
    [SerializeField, Tooltip("How fast the weapon's projectile moves")]
    private float projectileSpeed;
    [SerializeField, Tooltip("How large the weapon's hitbox is")]
    private float size;

    //Properties
    public float Damage => damage;
    public float Cooldown => cooldown;
    public float Duration => duration;
    public float ProjectileSpeed => projectileSpeed;
    public float Size => size;

    public void CalculateStat(StatType type)
    {
        //-----------------------------DEBUGGING CODE--------------------------------------
        Debug.Log("Calculating stat: " + type.ToString());
        SetStat(type, 1f);
        return;
        //---------------------------------------------------------------------------------

        float statValue =
            //Player stats
            StatManager.Instance.GetStat(type) +
            //Modification stats
            ModificationManager.instance.GetWeaponStatModification(weapon.WeaponType, type) +
            //Base weapon stats
            weaponData.GetBaseStat(type) +
            //Per-level weapon stats
            (weaponData.GetPerLevelStat(type) * weapon.WeaponLevel);
        SetStat(type, statValue);
    }

    private void SetStat(StatType type, float value)
    {
        switch (type)
        {
            case StatType.AttackDamage:
                damage = value;
                break;
            case StatType.Cooldown:
                cooldown = value;
                break;
            case StatType.Duration:
                duration = value;
                break;
            case StatType.ProjectileSpeed:
                projectileSpeed = value;
                break;
            case StatType.Size:
                size = value;
                break;
            default:
                Debug.LogWarning("WeaponStats: Attempted to set unknown stat type: " + type.ToString());
                break;
        }
    }

    public void ResetWeaponStats()
    {
        damage = 0f;
        cooldown = 0f;
        duration = 0f;
        projectileSpeed = 0f;
        size = 0f;
    }
}