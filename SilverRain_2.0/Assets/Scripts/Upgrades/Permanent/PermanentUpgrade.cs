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

    [Header("References")]
    [SerializeField, Tooltip("")]
    private PermanentUpgradeData data;
    [SerializeField, Tooltip("")]
    private StatType statType;
    [SerializeField, Tooltip("")]
    private UIPermanent uiData;

    // Events
    public UnityEvent<StatType> OnPermanentUpgradeLevelChanged;

    // Properties 
    public int Level => level;
    public int MaxLevel => maxLevel;
    public StatType StatType => statType;
    public UIPermanent UIData => uiData;

    public void LevelUp()
    {
        if (level >= maxLevel) return;
        level++;
        OnPermanentUpgradeLevelChanged?.Invoke(statType);
        UpdateDescription();
    }

    public void ResetLevels()
    {
        level = 0;
        OnPermanentUpgradeLevelChanged?.Invoke(statType);
        UpdateDescription();
    }

    public float Calculate()
    {
        return data.BaseAmount + (data.AmountPerLevel * level);
    }

    public float CalculateNextLevel()
    {
        return data.BaseAmount + (data.AmountPerLevel * (level + 1));
    }

    public float GetNextLevelCost()
    {
        if (level >= maxLevel) return 0f;
        return data.Cost + (data.CostIncreasePerLevel * level);
    }

    public void UpdateDescription()
    {
        uiData.UpdateDescription(level ,Calculate(), CalculateNextLevel());
    }
}