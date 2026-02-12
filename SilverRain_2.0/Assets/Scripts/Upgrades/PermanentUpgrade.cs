using System;
using UnityEngine;
using UnityEngine.Events;

public class PermanentUpgrade : MonoBehaviour, IUpgradeable
{
    [Header("Level")]
    [SerializeField, Tooltip("")]
    private int level;
    [SerializeField, Tooltip("")]
    private int maxLevel;
    [Space]

    [Header("References")]
    [SerializeField, Tooltip("")]
    private PermanentUpgradeData data;
    [SerializeField, Tooltip("")]
    private StatType statType;
    [Space]

    [Header("UI")]
    [SerializeField, Tooltip("")]
    private string description;

    // Events
    public UnityEvent<StatType> OnPermanentUpgradeLevelChanged;

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
        float currentValue = Calculate();

        if (level >= maxLevel)
        {
            description = $"+{currentValue} (MAX)";
        }
        else
        {
            float nextValue = data.BaseAmount + data.AmountPerLevel * (level + 1);
            description = $"+{currentValue} → +{nextValue}";
        }
    }

    public float Calculate()
    {
        return data.BaseAmount + (data.AmountPerLevel * level);
    }
}