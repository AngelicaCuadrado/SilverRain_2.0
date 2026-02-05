using UnityEngine;

[CreateAssetMenu(fileName = "BaseUpgradeData", menuName = "Scriptable Objects/BaseUpgradeData")]
public abstract class BaseUpgradeData : ScriptableObject
{
    StatType statType;
    float baseAmount;
    float amountPerLevel;
    string name;
    string description;
    int maxLevel;

    public StatType StatType => statType;
    public float BaseAmount => baseAmount;
    public float AmountPerLevel => amountPerLevel;
    public string Name => name;
    public string Description => description;
    public int MaxLevel => maxLevel;

}
