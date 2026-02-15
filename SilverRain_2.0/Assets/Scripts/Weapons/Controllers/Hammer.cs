using System.Collections;
using UnityEngine;

public class Hammer : Weapon
{
    [Header("Projectile Settings")]
    [SerializeField, Tooltip("")]
    private float startRotation = -35f;

    [Header("References")]
    [SerializeField, Tooltip("The position from which the hammer projectile will spawn")]
    private Transform firePoint;

    private void Update()
    {
        if (cam == null) return;
        //Make the hammer visual follow the camera's position and rotation
        Vector3 desiredPos = cam.position + (cam.forward * spawnOffsetForward) + (cam.up * spawnOffsetUp) + (cam.right * spawnOffsetSide);
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
        weaponStats.CalculateStat(StatType.Size);
    }

    public override void UpdateDescription()
    {
        uiData.UpdateDescription(level, maxLevel,
            "Damage", weaponStats.GetCurrentStatsForUI(StatType.AttackDamage), weaponStats.GetNextLevelStatsForUI(StatType.AttackDamage),
            "Cooldown", weaponStats.GetCurrentStatsForUI(StatType.Cooldown), weaponStats.GetNextLevelStatsForUI(StatType.Cooldown),
            "Size", weaponStats.GetCurrentStatsForUI(StatType.Size), weaponStats.GetNextLevelStatsForUI(StatType.Size));
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
        Quaternion spawnRot = Quaternion.LookRotation(firePoint.forward, Vector3.up) * Quaternion.Euler(startRotation, 0f, 0f);
        //Instantiate projectile
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, firePoint.position, spawnRot);
        //Initialize projectile
        var proj = projObj.GetComponent<HammerProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("Hammer: The instantiated projectile does not have a HammerProjectile component.");
            Destroy(projObj);
            return;
        }
        proj.Init(this, cam.transform, weaponStats.Damage, weaponStats.Size, startRotation);
    }
    #endregion
}
