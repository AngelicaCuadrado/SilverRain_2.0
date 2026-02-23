using UnityEngine;

public class PistolEvolution : Pistol, IWeaponEvolution
{
    private StatType requiredStat = StatType.AttackDamage;
    private bool weaponRequirementMet = false;
    private bool upgradeRequirementMet = false;

    public override void Start()
    {
        base.Start();
        WeaponManager.Instance.OnWeaponMaxLevelReached.AddListener(OnRequirementMet);
        StatManager.Instance.OnStatMaxLevelReached.AddListener(OnRequirementMet);
    }

    public void OnRequirementMet(WeaponType type)
    {
        if (type == weaponType)
        {
            weaponRequirementMet = true;
            // Check if both requirements are met to evolve the weapon
            if (upgradeRequirementMet)
            {
                EvolveWeapon();
            }
        }
    }

    public void OnRequirementMet(StatType type)
    {
        if (type == requiredStat)
        {
            upgradeRequirementMet = true;
            // Check if both requirements are met to evolve the weapon
            if (weaponRequirementMet)
            {
                EvolveWeapon();
            }
        }
    }

    public void EvolveWeapon()
    {
        // Implement the logic to evolve the weapon, such as changing its appearance, stats, etc.
    }
    private void OnDestroy()
    {
        WeaponManager.Instance.OnWeaponMaxLevelReached.RemoveListener(OnRequirementMet);
        StatManager.Instance.OnStatMaxLevelReached.RemoveListener(OnRequirementMet);
    }
}
