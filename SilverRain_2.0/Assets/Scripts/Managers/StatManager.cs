using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance { get; private set; }

    [Header("Upgrade Lists")]
    [SerializeField, Tooltip("")]
    private List<PermanentUpgradeEntry> allPermUpgradesList;
    [SerializeField, Tooltip("")]
    private List<TemporaryUpgradeEntry> allTempUpgradesList;
    private Dictionary<StatType, PermanentUpgrade> allPermUpgrades = new();
    private Dictionary<StatType, TemporaryUpgrade> allTempUpgrades = new();
    private Dictionary<StatType, TemporaryUpgrade> currentTempUpgrades = new();
    [Space]

    [Header("Upgrade Amount")]
    [SerializeField, Tooltip("")]
    private int maxTempUpgrades;
    [Space]

    [Header("Events")]
    public UnityEvent<StatType, float> OnStatChanged;
    public UnityEvent<TemporaryBuff, bool> OnTempUpgradeAvailabilityChange;
    [Space]

    [Header("Stats")]
    [SerializeField, Tooltip("")]
    private float attackDamage;
    [SerializeField, Tooltip("")]
    private float cooldown;
    [SerializeField, Tooltip("")]
    private float duration;
    [SerializeField, Tooltip("")]
    private float projectSpeed;
    [SerializeField, Tooltip("")]
    private float size;
    [SerializeField, Tooltip("")]
    private float maxHealth;
    [SerializeField, Tooltip("")]
    private float movementSpeed;
    [SerializeField, Tooltip("")]
    private float armor;
    [SerializeField, Tooltip("")]
    private float xpMult;
    [SerializeField, Tooltip("")]
    private float healthRegen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        foreach (var entry in allPermUpgradesList)
        {
            if (!allPermUpgrades.ContainsKey(entry.statType))
            {
                allPermUpgrades.Add(entry.statType, entry.permanentUpgrade);
            }
            else
            {
                Debug.LogWarning($"Duplicate permanent upgrade type {entry.statType} found in allPermUpgradesList.");
            }
        }
        foreach (var entry in allTempUpgradesList)
        {
            if (!allTempUpgrades.ContainsKey(entry.statType))
            {
                allTempUpgrades.Add(entry.statType, entry.temporaryUpgrade);
            }
            else
            {
                Debug.LogWarning($"Duplicate temporary upgrade type {entry.statType} found in allTempUpgradesList.");
            }
        }
        maxTempUpgrades = 3;
    }

    public float GetStat(StatType type)
    {
        return type switch
        {
            StatType.AttackDamage => attackDamage,
            StatType.Cooldown => cooldown,
            StatType.Duration => duration,
            StatType.ProjectileSpeed => projectSpeed,
            StatType.Size => size,
            StatType.MaxHealth => maxHealth,
            StatType.MovementSpeed => movementSpeed,
            StatType.Armor => armor,
            StatType.XpMult => xpMult,
            StatType.HealthRegen => healthRegen,
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

        if (ModificationManager.Instance != null)
        {
            finalStat += ModificationManager.Instance.GetStatModifications(type);
        }
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
                    attackDamage = newValue;
                    break;
                case StatType.Cooldown:
                    cooldown = newValue;
                    break;
                case StatType.Duration:
                    duration = newValue;
                    break;
                case StatType.ProjectileSpeed:
                    projectSpeed = newValue;
                    break;
                case StatType.Size:
                    size = newValue;
                    break;
                case StatType.MaxHealth:
                    maxHealth = newValue;
                    break;
                case StatType.MovementSpeed:
                    movementSpeed = newValue;
                    break;
                case StatType.Armor:
                    armor = newValue;
                    break;
                case StatType.XpMult:
                    xpMult = newValue;
                    break;
                case StatType.HealthRegen:
                    healthRegen = newValue;
                    break;
            }
            //OnStatChanged?.Invoke(type, newValue);
        }
    }

    public void ApplyPermanentStatsAtGameStart()
    {
        foreach (var type in allPermUpgrades.Keys)
        {
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

        if (!currentTempUpgrades.ContainsKey(type))
        {
            currentTempUpgrades.Add(type, allTempUpgrades[type]);
            Debug.Log($"Added temporary upgrade of type {type} to currentTempUpgrades.");
            Debug.Log($"Current temporary upgrades count: {currentTempUpgrades.Count}");
        }

        currentTempUpgrades[type].LevelUp();
        UpdateTempStats(type);

        if (currentTempUpgrades.Count >= maxTempUpgrades)
        {
            foreach (var statType in allTempUpgrades.Keys)
            {
                if (!currentTempUpgrades.ContainsKey(statType))
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
                    attackDamage = newValue;
                    break;
                case StatType.Cooldown:
                    cooldown = newValue;
                    break;
                case StatType.Duration:
                    duration = newValue;
                    break;
                case StatType.ProjectileSpeed:
                    projectSpeed = newValue;
                    break;
                case StatType.Size:
                    size = newValue;
                    break;
                case StatType.MaxHealth:
                    maxHealth = newValue;
                    break;
                case StatType.MovementSpeed:
                    movementSpeed = newValue;
                    break;
                case StatType.Armor:
                    armor = newValue;
                    break;
                case StatType.XpMult:
                    xpMult = newValue;
                    break;
                case StatType.HealthRegen:
                    healthRegen = newValue;
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

    public void HandleTempStatAvailabilityChange(TemporaryBuff tempUpgrade, bool isAvailable)
    {
        OnTempUpgradeAvailabilityChange?.Invoke(tempUpgrade, isAvailable);
    }
}