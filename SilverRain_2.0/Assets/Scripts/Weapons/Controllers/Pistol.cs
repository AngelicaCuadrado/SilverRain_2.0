using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Pistol : Weapon
{
    [Header("References")]
    [SerializeField, Tooltip("The position from which bullets will spawn")]
    private Transform firePoint;

    private void Update()
    {
        if (cam == null) return;
        //Make the pistol follow the camera's position and rotation
        Vector3 desiredPos = cam.position + (cam.forward * spawnOffsetForward) + (cam.up * spawnOffsetUp);
        transform.position = desiredPos;
        transform.rotation = Quaternion.LookRotation(-cam.forward, Vector3.up);
    }

    #region TemporaryBuff Implementation
    public override void LevelUp()
    {
        base.LevelUp();
        // Recalculate stats
        weaponStats.CalculateStat(StatType.AttackDamage);
        weaponStats.CalculateStat(StatType.Cooldown);
        weaponStats.CalculateStat(StatType.ProjectileSpeed);
    }

    public override void UpdateDescription()
    {
        uiData.UpdateDescription(level, maxLevel,
            "Damage", weaponStats.GetCurrentStatsForUI(StatType.AttackDamage), weaponStats.GetNextLevelStatsForUI(StatType.AttackDamage),
            "Cooldown", weaponStats.GetCurrentStatsForUI(StatType.Cooldown), weaponStats.GetNextLevelStatsForUI(StatType.Cooldown),
            "Speed", weaponStats.GetCurrentStatsForUI(StatType.ProjectileSpeed), weaponStats.GetNextLevelStatsForUI(StatType.ProjectileSpeed));
    }
    #endregion

    #region Weapon Implementation
    public override IEnumerator OnDuration()
    {
        Attack();
        StartCoroutine(OnCooldown());
        yield break;
    }

    public override void Attack()
    {
        //Calculate spawn rotation so the projectile faces forward
        Quaternion spawnRot = Quaternion.LookRotation(firePoint.forward, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
        //Instantiate projectile
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, firePoint.position, spawnRot);
        //Initialize projectile
        var proj = projObj.GetComponent<PistolProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("Pistol: The instantiated projectile does not have a PistolProjectile component.");
            Destroy(projObj);
            return;
        }
        proj.Init(this, weaponStats.Damage, firePoint.forward, weaponStats.ProjectileSpeed);
    }
    #endregion
}