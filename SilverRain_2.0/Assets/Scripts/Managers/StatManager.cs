using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatManager : MonoBehaviour
{
    Dictionary<StatType, PermanentUpgrade> allPermUpgrades = new Dictionary<StatType, PermanentUpgrade>();
    Dictionary<StatType, TemporaryUpgrade> allTempUpgrades = new Dictionary<StatType, TemporaryUpgrade>();
    Dictionary<StatType, TemporaryUpgrade> currentTempUpgrades = new Dictionary<StatType, TemporaryUpgrade>();
    int maxTempUpgrades;
    UnityEvent<StatType, float> OnPermStatChanged;
    UnityEvent<StatType, float> OnTempStatChanged;

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
}
