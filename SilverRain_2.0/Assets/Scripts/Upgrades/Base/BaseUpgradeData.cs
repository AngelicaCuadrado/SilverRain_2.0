using UnityEngine;

//[System.Serializable]
public abstract class BaseUpgradeData : ScriptableObject
{
    [SerializeField, Tooltip("")]
    protected float baseAmount;
    [SerializeField, Tooltip("")]
    protected float amountPerLevel;

    public float BaseAmount => baseAmount;
    public float AmountPerLevel => amountPerLevel;
}