using System.Collections;
using UnityEngine;

public class ModificationLightMetal : Modification, IWeaponModifier
{
    public override void ApplyEffect()
    {
        return;
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
}
