using System;
using UnityEngine;
using UnityEngine.Events;

public class TemporaryUpgrade : TemporaryBuff
{
    [SerializeField,Tooltip("The data for base and per-level stat values")]
    private TemporaryUpgradeData data;
    [SerializeField, Tooltip("Identifier for the stat this upgrade handles")]
    private StatType statType;

    // Properties
    public TemporaryUpgradeData Data => data;
    public StatType StatType => statType;

    public override void SetAvailable(bool availability)
    {
        isAvailable = availability;
        StatManager.Instance.HandleTempStatAvailabilityChange(this, availability);
    }

    public override void UpdateDescription()
    {
        uiData.UpdateDescription(level, maxLevel, Calculate(), CalculateNextLevel());
    }

    public float Calculate()
    {
        return data.BaseAmount + (data.AmountPerLevel * level);
    }

    private float CalculateNextLevel()
    {
        return data.BaseAmount + (data.AmountPerLevel * (level + 1));
    }
}
