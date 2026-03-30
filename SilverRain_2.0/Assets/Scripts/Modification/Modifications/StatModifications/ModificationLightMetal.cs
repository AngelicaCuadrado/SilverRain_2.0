using System.Collections;
using UnityEngine;

public class ModificationLightMetal : Modification, IWeaponModifier
{
    public override void Activate()
    {
        base.Activate();
        // Invoke the event to update the weapon stats
        ModificationManager.Instance.OnWeaponStatModificationChange.Invoke(WeaponType.Sword, StatType.AttackDamage);
        ModificationManager.Instance.OnWeaponStatModificationChange.Invoke(WeaponType.Sword, StatType.Cooldown);
    }

    public float GetModifyValue(WeaponType weapon, StatType stat)
    {
        if (weapon == WeaponType.Sword)
        {
            switch (stat)
            {
                case StatType.AttackDamage: return -0.5f; //50% less damage
                case StatType.Cooldown: return -0.5f; //50% faster
                default: return 0;
            }
        }
        return 0;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponAquired.RemoveListener(OnRequirementMet);
    }
}