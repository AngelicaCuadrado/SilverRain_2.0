using UnityEngine;

[CreateAssetMenu(fileName = "BaseUpgradeData", menuName = "Scriptable Objects/BaseUpgradeData")]
public abstract class BaseUpgradeData : ScriptableObject
{
    StatType statType;
    float baseAmount;
    float amountPerLevel;
    string upgradeName;
    string description;
    int maxLevel;

    public StatType StatType => statType;
    public float BaseAmount => baseAmount;
    public float AmountPerLevel => amountPerLevel;
    public string Name => upgradeName;
    public string Description => description;
    public int MaxLevel => maxLevel;

}
