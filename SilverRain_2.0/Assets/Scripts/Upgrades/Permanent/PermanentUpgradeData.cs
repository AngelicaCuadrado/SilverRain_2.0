using UnityEngine;

[CreateAssetMenu(fileName = "NewPermanentUpgradeData", menuName = "Data/Permanent Upgrade Data")]
public class PermanentUpgradeData : BaseUpgradeData
{
    [SerializeField, Tooltip("The base price of buying this upgrade")]
    private float cost;
    [SerializeField, Tooltip("The price increase per-level of buying this upgrade")]
    private float costIncreasePerLevel;

    public float Cost => cost;
    public float CostIncreasePerLevel => costIncreasePerLevel;
}
