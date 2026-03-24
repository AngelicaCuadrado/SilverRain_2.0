using UnityEngine;

public class ChakramEvolution : Chakram, IWeaponEvolution
{
    [Header("Evolution Requirements")]
    [SerializeField, Tooltip("The type of weapon that must reach max level to evolve this weapon.")]
    private WeaponType requiredWeapon = WeaponType.Chakram;
    [SerializeField, Tooltip("The type of upgrade that must reach max level to evolve this weapon.")]
    private StatType requiredStat = StatType.MovementSpeed;
    [SerializeField, Tooltip("Indicates whether the weapon requirement has been met.")]
    private bool weaponRequirementMet = false;
    [SerializeField, Tooltip("Indicates whether the upgrade requirement has been met.")]
    private bool upgradeRequirementMet = false;
    [SerializeField, Tooltip("The maximum degree of the arc in which projectiles will spawn")]
    private float coneDegrees = 45f;
    [SerializeField, Tooltip("How many projectiles will spawn in an arc")]
    private int numOfProjectiles = 3;

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
    public override void Attack()
    {
        Vector3 centerDir = cam.forward;

        // Compute angle step
        float halfCone = coneDegrees * 0.5f;
        float step = coneDegrees / (numOfProjectiles - 1);

        for (int i = 0; i < numOfProjectiles; i++)
        {
            // Angle offset for this projectile
            float angle = -halfCone + (step * i);

            // Rotate center direction by angle around the Y axis
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * centerDir;

            // Rotation so projectile faces the direction
            Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

            // Spawn projectile
            var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, playerTrans.position + firePointOffset, rot);

            var proj = projObj.GetComponent<ChakramProjectile>();
            if (proj == null)
            {
                Debug.LogWarning("ChakramEvolution: projectile missing ChakramProjectile component.");
                Destroy(projObj);
                return;
            }

            // Initialize projectile
            proj.Init(this, playerTrans, direction, weaponStats.Damage, weaponStats.Duration, weaponStats.Size, weaponStats.ProjectileSpeed);

            // Trigger modification for each projectile
            HandleProjectileSpawn(projObj);
        }

        // Trigger modification only once per attack
        //HandleProjectileSpawn();
    }
    #endregion
}