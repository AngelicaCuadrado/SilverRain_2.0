using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class StatManager : MonoBehaviour
{
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
}
