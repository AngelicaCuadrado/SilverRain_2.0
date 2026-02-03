using System.Data;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [SerializeField, Tooltip("The weapon controller that is attached to the same weapon object")]
    private Weapon weapon;
    [SerializeField, Tooltip("The scriptable object holding the default, and per-level properties")]
    private WeaponData weaponData;

    //TBD if we want to cached properties for performance
    private float damage;
    private float cooldown;
    private float duration;
    private float projectileSpeed;
    private float size;

    public float CalculateStat(StatType type)
    {
        switch (type)
        {
            case StatType.AttackDamage:
                damage = weaponData.baseDamage +
                    (weaponData.perLevelDamage * weapon.WeaponLevel) +
                    GetModificationStats(weapon.WeaponType, type);
                return damage;
            case StatType.Cooldown:
                cooldown = weaponData.baseCooldown +
                    (weaponData.perLevelCooldown * weapon.WeaponLevel) +
                    GetModificationStats(weapon.WeaponType, type);
                return cooldown;
            case StatType.Duration:
                duration = weaponData.baseDuration +
                    (weaponData.perLevelDuration * weapon.WeaponLevel) +
                    GetModificationStats(weapon.WeaponType, type);
                return duration;
            case StatType.ProjectileSpeed:
                projectileSpeed = weaponData.baseProjectileSpeed +
                    (weaponData.perLevelProjectileSpeed * weapon.WeaponLevel) +
                    GetModificationStats(weapon.WeaponType, type);
                return projectileSpeed;
            case StatType.Size:
                size = weaponData.baseSize +
                    (weaponData.perLevelSize * weapon.WeaponLevel) +
                    GetModificationStats(weapon.WeaponType, type);
                return size;
            default:
                Debug.LogError("Invalid StatType provided to CalculateStat");
                return 0f;
        }
    }

    public float GetModificationStats(WeaponType weaponType, StatType statType)
    {
        return 0f;

        //NOT IMPLEMENTED YET
        //Sudo code example:
        //Go to modification manager instance
        //call GetWeaponStatModification(weaponType, statType)
        //return that value to be added to the base + per level calculation
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