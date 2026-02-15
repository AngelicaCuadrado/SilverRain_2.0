using System.Collections;
using UnityEngine;

public class Chakram : Weapon
{
    [Header("References")]
    [SerializeField, Tooltip("The player's location that the chakram returns to")]
    private Transform playerTrans;
    [SerializeField, Tooltip("")]
    private Vector3 firePointOffset;
    [SerializeField, Tooltip("The rotation which the projectile will spawn in relative to the camera")]
    private float spawnAngleOffset = 90f;

    private void Update()
    {
        if (cam == null) return;
        //Make the chakram visual follow the camera's position and rotation
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
        //Cache the player transform
        playerTrans = PlayerFinder.Instance.Player.transform;
        if (playerTrans == null)
        {
            Debug.LogError("Chakram: Could not find player transform.");
            return;
        }
        base.OnActivate();
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
        Quaternion spawnRot = Quaternion.Euler(playerTrans.eulerAngles.x + spawnAngleOffset, 0f, 0f);
        //Instantiate projectile
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, playerTrans.position + firePointOffset, spawnRot);
        //Initialize projectile
        var proj = projObj.GetComponent<ChakramProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("Chakram: The instantiated projectile does not have a ChakramProjectile component.");
            Destroy(projObj);
            return;
        }
        proj.Init(this, playerTrans, cam.forward, weaponStats.Damage, weaponStats.Duration, weaponStats.Size, weaponStats.ProjectileSpeed);
    }
    #endregion
}
