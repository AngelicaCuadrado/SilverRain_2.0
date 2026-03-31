using System.Collections;
using UnityEngine;

public class ModificationLightMetal : Modification, IStatModifier
{
    private bool isActive = false;

    public override void Activate()
    {
        base.Activate();
        isActive = true;
        StatManager.Instance.UpdateTempStats(StatType.AttackDamage);
        StatManager.Instance.UpdateTempStats(StatType.Cooldown);
    }

    public float GetModifyValue(StatType type)
    {
        if (!isActive) return 0;

        switch (type)
        {
            case StatType.AttackDamage: return -0.5f; //50% less damage
            case StatType.Cooldown: return -0.5f; //50% faster
            default: return 0;
        }
    }
}