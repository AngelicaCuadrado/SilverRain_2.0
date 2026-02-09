using System;
using UnityEngine;
using UnityEngine.Events;

public class PermanentUpgrade : IUpgradeable
{
    int currentLevel;
    //int maxcurrentLevel;
    PermanentUpgradeData data;
    string description;
    StatType statType;

    UnityEvent<StatType> OnPermanentUpgradeLevelChanged;
    public void LevelUp()
    {
        if (currentLevel >= data.MaxLevel) return;
        currentLevel++;
        OnPermanentUpgradeLevelChanged?.Invoke(statType);
    }

    public void ResetLevels()
    {
        currentLevel = 0;
    }

    public void UpdateDescription()
    {
        float currentValue = Calculate();

        if (currentLevel >= data.MaxLevel)
        {
            description = $"+{currentValue} (MAX)";
        }
        else
        {
            float nextValue = data.BaseAmount + data.AmountPerLevel * (currentLevel + 1);
            description = $"+{currentValue} → +{nextValue}";
        }
    }


    public float Calculate()
    {
        return data.BaseAmount + (data.AmountPerLevel * currentLevel);
    }
}
