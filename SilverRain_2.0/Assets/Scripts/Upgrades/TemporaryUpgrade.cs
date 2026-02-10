using System;
using UnityEngine;
using UnityEngine.Events;

public class TemporaryUpgrade : ITemporary
{
    int currentLevel;
    //int maxLevel;
    string description;
    TemporaryUpgradeData data;
    bool isAvailable;
    public StatType StatType;
    [SerializeField, Tooltip("")]
    private UITemporary uiData;
    public UITemporary UIData { get { return uiData; } }

    UnityEvent<ITemporary, bool> OnAvailabilityChanged;
    UnityEvent<StatType> OnTemporaryUpgradeLevelChanged;

    public void LevelUp()
    {
        if (currentLevel >= data.MaxLevel) return;
        currentLevel++;
        OnTemporaryUpgradeLevelChanged?.Invoke(StatType);

        if (currentLevel >= data.MaxLevel)
        {
            SetAvailable(false);
            OnAvailabilityChanged?.Invoke(this, false);
        }
    }

    public void ResetLevels()
    {
        currentLevel = 0;
        SetAvailable(true);
    }

    public void SetAvailable(bool available)
    {
        isAvailable = available;
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
