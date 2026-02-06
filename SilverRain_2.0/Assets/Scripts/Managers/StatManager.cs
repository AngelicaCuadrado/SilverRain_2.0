using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class StatManager : MonoBehaviour
{
    List<PermanentUpgradeEntry> allPermUpgradesList;
    List<TemporaryUpgradeEntry> allTempUpgradesList;
    List<TemporaryUpgradeEntry> currentTempUpgradesList;
    Dictionary<StatType, PermanentUpgrade> allPermUpgrades = new Dictionary<StatType, PermanentUpgrade>();
    Dictionary<StatType, TemporaryUpgrade> allTempUpgrades = new Dictionary<StatType, TemporaryUpgrade>();
    Dictionary<StatType, TemporaryUpgrade> currentTempUpgrades = new Dictionary<StatType, TemporaryUpgrade>();
    int maxTempUpgrades;
    UnityEvent<StatType, float> OnStatChanged;

    float AttackDamage;
    float Cooldown;
    float Duration;
    float ProjectSpeed;
    float Size;
    float MaxHealth;
    float MovementSpeed;
    float Armor;
    float XpMult;
    float HealthRegen;

    private static StatManager instance;
    public static StatManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StatManager();
            }
            return instance;
        }
    }
    public float GetStat(StatType type)
    {
        return type switch
        {
            StatType.AttackDamage => AttackDamage,
            StatType.Cooldown => Cooldown,
            StatType.Duration => Duration,
            StatType.ProjectileSpeed => ProjectSpeed,
            StatType.Size => Size,
            StatType.MaxHealth => MaxHealth,
            StatType.MovementSpeed => MovementSpeed,
            StatType.Armor => Armor,
            StatType.XpMult => XpMult,
            StatType.HealthRegen => HealthRegen,
            _ => 0f,
        };
    }
    
    public float CalculateStat(StatType type)
    {
        float finalStat = 0f;
        if (allPermUpgrades.ContainsKey(type))
        {
            finalStat += allPermUpgrades[type].Calculate();
        }

        if (currentTempUpgrades.ContainsKey(type))
        {
            finalStat += currentTempUpgrades[type].Calculate();
        }

        finalStat += ModificationManager.instance.GetStatModifications(type);
        return finalStat;
    }

    public void UpdatePermStats(StatType type)
    {
        if (allPermUpgrades.ContainsKey(type))
        {
            float newValue = CalculateStat(type);
            switch (type)
            {
                case StatType.AttackDamage:
                    AttackDamage = newValue;
                    break;
                case StatType.Cooldown:
                    Cooldown = newValue;
                    break;
                case StatType.Duration:
                    Duration = newValue;
                    break;
                case StatType.ProjectileSpeed:
                    ProjectSpeed = newValue;
                    break;
                case StatType.Size:
                    Size = newValue;
                    break;
                case StatType.MaxHealth:
                    MaxHealth = newValue;
                    break;
                case StatType.MovementSpeed:
                    MovementSpeed = newValue;
                    break;
                case StatType.Armor:
                    Armor = newValue;
                    break;
                case StatType.XpMult:
                    XpMult = newValue;
                    break;
                case StatType.HealthRegen:
                    HealthRegen = newValue;
                    break;
            }
            //OnStatChanged?.Invoke(type, newValue);
        }
    }

    public void ApplyPermanentStatsAtGameStart()
    {
        foreach (var type in allPermUpgrades.Keys)
        {
            UpdatePermStats(type);               
            OnStatChanged?.Invoke(type, GetStat(type));
        }
    }

    public void AddTempUpgrade(StatType type)
    {
        if (!allTempUpgrades.ContainsKey(type))
        {
            Debug.LogWarning($"Temporary upgrade of type {type} does not exist in allTempUpgrades.");
            return;
        }

        currentTempUpgrades.Add(type, allTempUpgrades[type]);

        if (currentTempUpgrades.Count >= maxTempUpgrades)
        {
            foreach(var statType in allTempUpgrades.Keys)
            {
                if(!currentTempUpgrades.ContainsKey(statType))
                {
                    allTempUpgrades[statType].SetAvailable(false);
                }
            }
        }
    }

    public void UpdateTempStats(StatType type)
    {
        if (currentTempUpgrades.ContainsKey(type))
        {
            float newValue = CalculateStat(type);
            switch (type)
            {
                case StatType.AttackDamage:
                    AttackDamage = newValue;
                    break;
                case StatType.Cooldown:
                    Cooldown = newValue;
                    break;
                case StatType.Duration:
                    Duration = newValue;
                    break;
                case StatType.ProjectileSpeed:
                    ProjectSpeed = newValue;
                    break;
                case StatType.Size:
                    Size = newValue;
                    break;
                case StatType.MaxHealth:
                    MaxHealth = newValue;
                    break;
                case StatType.MovementSpeed:
                    MovementSpeed = newValue;
                    break;
                case StatType.Armor:
                    Armor = newValue;
                    break;
                case StatType.XpMult:
                    XpMult = newValue;
                    break;
                case StatType.HealthRegen:
                    HealthRegen = newValue;
                    break;
            }
            OnStatChanged?.Invoke(type, newValue);
        }
    }

    public void ResetPermStats()
    {
        foreach (var permanentUpgrade in allPermUpgrades.Values)
        {
            permanentUpgrade.ResetLevels();
        }
    }

    public void ResetTempStats()
    {
        currentTempUpgrades.Clear();
        foreach (var temporaryUpgrade in allTempUpgrades.Values)
        {
            temporaryUpgrade.ResetLevels();
        }
    }
}
