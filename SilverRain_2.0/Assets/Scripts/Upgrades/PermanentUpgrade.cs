using System;
using UnityEngine;
using UnityEngine.Events;

public class PermanentUpgrade : IUpgradeable
{
    int level;
    int maxLevel;
    PermanentUpgradeData data;
    StatType statType;

    UnityEvent<StatType> OnPermanentUpgradeLevelChanged;
    public void LevelUp()
    {
        if (level >= maxLevel) return;
        level++;
        OnPermanentUpgradeLevelChanged?.Invoke(statType);
    }

    public void ResetLevels()
    {
        level = 0;
    }

    public void UpdateDescription()
    {
        throw new System.NotImplementedException();
    }

    public float Calculate()
    {
        return data.BaseAmount + (data.AmountPerLevel * level);
    }
}
