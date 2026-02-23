using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [Header("Gold Data")]
    [SerializeField] private float currentGold;
    private float spentGold;

    public float CurrentGold => currentGold;
    public float SpentGold => spentGold;

    public static UnityEvent OnGoldChanged = new UnityEvent();

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
    }

    public void GainGold(float amount)
    {
        if (amount <= 0) return;
        currentGold += amount;
        OnGoldChanged?.Invoke();
    }

    public bool SpendGold(float amount)
    {
        if (amount <= 0) return true;

        if (currentGold >= amount)
        {
            currentGold -= amount;
            spentGold += amount; 
            OnGoldChanged?.Invoke();
            return true;
        }

        return false; 
    }

    public void ConvertToGold(float amount, float multiplier)
    {
        GainGold(amount * multiplier);
    }

    public void RefundAllSpentGold()
    {
        if (spentGold > 0)
        {
            currentGold += spentGold;
            spentGold = 0; 
            OnGoldChanged?.Invoke();
        }
    }
}