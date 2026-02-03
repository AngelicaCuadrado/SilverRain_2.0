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

    public float CalculateStat(StatType type)
    {
        return
            //Player stats
            StatManager.Instance.GetStat(type) +
            //Modification stats
            ModificationManager.Instance.GetWeaponStatModification(weapon.WeaponType, type) +
            //Base weapon stats
            weaponData.GetBaseStat(type) +
            //Per-level weapon stats
            (weaponData.GetPerLevelStat(type) * weapon.WeaponLevel);
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





//---------------------------- TEMPORARY TO AVOID ERRORS, DELETE ALL OF THIS ----------------------------
public enum StatType
{
    Damage,
    Cooldown,
    Duration,
    ProjectileSpeed,
    Size
}

public class StatManager
{
    private static StatManager instance;
    public static StatManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StatManager();
            }
            return instance;
        }
    }
    public float GetStat(StatType type)
    {
        return 0f;
    }
}

public class ModificationManager
{
    private static ModificationManager instance;
    public static ModificationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ModificationManager();
            }
            return instance;
        }
    }
    public float GetWeaponStatModification(WeaponType weaponType, StatType statType)
    {
        return 0f;
    }
}