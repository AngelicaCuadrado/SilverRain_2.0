using UnityEngine;

public class GrenadeEvolution : Grenade, IWeaponEvolution
{
    [Header("Evolution Requirements")]
    [SerializeField, Tooltip("The type of weapon that must reach max level to evolve this weapon.")]
    private WeaponType requiredWeapon = WeaponType.Grenade;
    [SerializeField, Tooltip("The type of upgrade that must reach max level to evolve this weapon.")]
    private StatType requiredStat = StatType.AttackDamage;
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
    public override void LevelUp()
    {
        base.LevelUp();
        weaponStats.CalculateStat(StatType.ProjectileSpeed);
    }
    public override void Attack()
    {
        //Calculate spawn rotation so the projectile faces forward
        Quaternion rot = Quaternion.LookRotation(firePoint.forward, Vector3.up);

        //Instantiate projectile
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, firePoint.position, rot);

        //Initialize projectile
        var proj = projObj.GetComponent<GrenadeProjectile>();
        if (proj != null)
        {
            proj.Init(this, weaponStats.Damage, weaponStats.Size);
        }

        // Set projectile velocity
        Rigidbody rb = projObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 speed = firePoint.forward * throwForce * weaponStats.ProjectileSpeed;
            rb.linearVelocity = speed;
            print(firePoint.forward);
            print(throwForce);
            print(weaponStats.ProjectileSpeed);
            print(speed);
            rb.angularVelocity = Vector3.zero;
        }

        // Notify the WeaponManager about the projectile spawn
        HandleProjectileSpawn();
    }
    #endregion
}
