using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SwordEvolution : Sword, IWeaponEvolution
{
    [Header("Evolution Requirements")]
    [SerializeField, Tooltip("The type of weapon that must reach max level to evolve this weapon.")]
    private WeaponType requiredWeapon = WeaponType.Sword;
    [SerializeField, Tooltip("The type of upgrade that must reach max level to evolve this weapon.")]
    private StatType requiredStat = StatType.Duration;
    [SerializeField, Tooltip("Indicates whether the weapon requirement has been met.")]
    private bool weaponRequirementMet = false;
    [SerializeField, Tooltip("Indicates whether the upgrade requirement has been met.")]
    private bool upgradeRequirementMet = false;

    public override void Start()
    {
        base.Start();
        // Subscribe to events
        WeaponManager.Instance.OnWeaponMaxLevelReached.AddListener(OnRequirementMet);
        StatManager.Instance.OnStatMaxLevelReached.AddListener(OnRequirementMet);
    }
    private void OnDestroy()
    {
        // Unsubscribe from events
        WeaponManager.Instance.OnWeaponMaxLevelReached.RemoveListener(OnRequirementMet);
        StatManager.Instance.OnStatMaxLevelReached.RemoveListener(OnRequirementMet);
    }

    #region IWeaponEvolution implementation
    public void OnRequirementMet(WeaponType type)
    {
        if (type == requiredWeapon)
        {
            weaponRequirementMet = true;
            CheckRequirements();
        }
    }

    public void OnRequirementMet(StatType type)
    {
        if (type == requiredStat)
        {
            upgradeRequirementMet = true;
            CheckRequirements();
        }
    }

    public void CheckRequirements()
    {
        if (weaponRequirementMet && upgradeRequirementMet)
        {
            SetAvailable(true);
        }
    }
    #endregion

    #region Weapon overrides
    public override IEnumerator OnDuration()
    {
        Attack();
        yield break;
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
        // Send -1 for duration will cause it to stay active
        proj.Init(this, playerTrans, weaponStats.Damage, -1, weaponStats.Size, weaponStats.ProjectileSpeed);
        // Notify the WeaponManager about the projectile spawn
        HandleProjectileSpawn();
    }
    #endregion
}