using UnityEngine;

public class PistolEvolution : Pistol, IWeaponEvolution
{
    [Header("Evolution Requirements")]
    [SerializeField, Tooltip("The type of weapon that must reach max level to evolve this weapon.")]
    private WeaponType requiredWeapon = WeaponType.Pistol;
    [SerializeField, Tooltip("The type of upgrade that must reach max level to evolve this weapon.")]
    private StatType requiredStat = StatType.ProjectileSpeed;
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

    #endregion
}
