using System.Collections;
using UnityEngine;

public class LightMetal : Modification, IStatModifier
{
    [SerializeField,Tooltip("Indicates whether the modification is currently active.")]
    private bool isActive = false;
    [SerializeField, Tooltip("The percentage reduction in attack damage")]
    private float attackDamageModifier = -0.5f; //50% less damage
    [SerializeField, Tooltip("The percentage reduction in cooldown time")]
    private float cooldownModifier = -0.5f; //50% faster cooldown

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

        return type switch
        {
            StatType.AttackDamage => attackDamageModifier,
            StatType.Cooldown => cooldownModifier,
            _ => 0,
        };
    }
}