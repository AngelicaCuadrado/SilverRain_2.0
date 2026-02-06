using System;
using UnityEngine;
using UnityEngine.Events;

public class TemporaryUpgrade : ITemporary
{
    int level;
    int maxLevel;
    TemporaryUpgradeData data;
    bool isAvailable;
    public StatType StatType;

    UnityEvent<ITemporary, bool> OnAvailabilityChanged;
    UnityEvent<StatType> OnTemporaryUpgradeLevelChanged;

    public void LevelUp()
    {
        if (level >= maxLevel) return;
        level++;
        OnTemporaryUpgradeLevelChanged?.Invoke(StatType);

        if (level >= maxLevel)
        {
            SetAvailable(false);
            OnAvailabilityChanged?.Invoke(this, false);
        }
    }

    public void ResetLevels()
    {
        level = 0;
        SetAvailable(true);
    }

    public void SetAvailable(bool available)
    {
        isAvailable = available;
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
