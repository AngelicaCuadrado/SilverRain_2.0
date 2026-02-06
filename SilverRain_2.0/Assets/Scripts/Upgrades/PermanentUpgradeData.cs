using UnityEngine;

public class PermanentUpgradeData : BaseUpgradeData
{
    float cost;
    float costIncreasePerLevel;

    public float Cost => cost;
    public float CostIncreasePerLevel => costIncreasePerLevel;
}
