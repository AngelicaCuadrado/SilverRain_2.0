
using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerExperience : MonoBehaviour
{
    [Header("Player Experience Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExp = 0f;
    [SerializeField] private float requriedExp = 100f;
    [SerializeField] private float expGrowthRate = 1.5f;
    
    public UnityEvent OnExpChanged;
    public UnityEvent OnLevelUp;

    private float expMult = 1f;
    
    public int CurrentLevel => currentLevel;
    public float CurrentExp => currentExp;
    public float RequriedExp => requriedExp;

    private void Start()
    {
        CalculateRequriedExp();
    }

    private void OnEnable()
    {
        //StatsManager.Instance.OnStatChanged.AddListener(HandleExpMult);
        //expMult = StatsManager.Instance.GetStat(StatType.XpMult);
    }

    private void OnDisable()
    {
        //StatsManager.Instance.OnStatChanged.RemoveListener(HandleExpMult);
    }

    private void HandleExpMult(StatType statType, float value)
    {
        if (statType == StatType.XpMult) expMult = value;
    }

    public void GainExp(float amount)
    {
        currentExp += amount * expMult;
        OnExpChanged.Invoke();
        
        if (currentExp >= requriedExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= requriedExp;
        CalculateRequriedExp();
        
        OnExpChanged.Invoke();
        OnLevelUp.Invoke();
    }

    private void CalculateRequriedExp()
    {
        requriedExp = 100f * Mathf.Pow(expGrowthRate, CurrentLevel - 1);
    }
    
    public float GetExpPersentage()
    {
        return currentExp /  requriedExp;
    }
}
