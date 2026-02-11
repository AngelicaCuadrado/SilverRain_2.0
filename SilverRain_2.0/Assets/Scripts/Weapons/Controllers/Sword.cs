using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Projectile Settings")]
    [SerializeField, Tooltip("The center of the sword's rotation")]
    private Transform playerTrans;
    [SerializeField, Tooltip("The rotation which the projectile will spawn in relative to the camera")]
    private float spawnAngleOffset = 90f;

    private void Update()
    {
        if (cam == null) return;
        //Make the pistol follow the camera's position and rotation
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
        weaponStats.CalculateStat(StatType.Duration);
        weaponStats.CalculateStat(StatType.ProjectileSpeed);
        weaponStats.CalculateStat(StatType.Size);
    }

    public override void UpdateDescription()
    {
        uiData.UpdateDescription(level, maxLevel,
            "Damage", weaponStats.GetCurrentStatsForUI(StatType.AttackDamage), weaponStats.GetNextLevelStatsForUI(StatType.AttackDamage),
            "Cooldown", weaponStats.GetCurrentStatsForUI(StatType.Cooldown), weaponStats.GetNextLevelStatsForUI(StatType.Cooldown),
            "Duration", weaponStats.GetCurrentStatsForUI(StatType.Duration), weaponStats.GetNextLevelStatsForUI(StatType.Duration),
            "Speed", weaponStats.GetCurrentStatsForUI(StatType.ProjectileSpeed), weaponStats.GetNextLevelStatsForUI(StatType.ProjectileSpeed),
            "Size", weaponStats.GetCurrentStatsForUI(StatType.Size), weaponStats.GetNextLevelStatsForUI(StatType.Size));
    }
    #endregion

    #region Weapon Implementation
    public override void OnActivate()
    {
        base.OnActivate();
        //Cache the player transform
        playerTrans = PlayerFinder.Instance.Player.transform;
        if (playerTrans == null)
        {
            Debug.LogError("Sword: Could not find player transform.");
            return;
        }
    }

    public override IEnumerator OnDuration()
    {
        Attack();
        yield return new WaitForSeconds(weaponStats.Duration);
        StartCoroutine(OnCooldown());
    }

    public override void Attack()
    {
        //Calculate spawn rotation so the projectile faces forward
        Quaternion spawnRot = Quaternion.Euler(0f, playerTrans.eulerAngles.y + spawnAngleOffset, 0f);
        //Instantiate projectile
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, playerTrans.position, spawnRot);
        //Initialize projectile
        var proj = projObj.GetComponent<SwordProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("Sword: The instantiated projectile does not have a SwordProjectile component.");
            Destroy(projObj);
            return;
        }
        proj.Init(this, playerTrans, weaponStats.Damage, weaponStats.Duration, weaponStats.Size, weaponStats.ProjectileSpeed);
    }
    #endregion
}