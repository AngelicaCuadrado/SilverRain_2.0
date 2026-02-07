using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }
    private float currentGold;
    private float spentGold;
    public static UnityEvent OnGoldChanged;

    public void GainGold(float amount)
    {

    }

    public bool SpendGold(float amount)
    {
        return true;
    }

    public void ConvertToGold(float amount, float multiplier)
    {

    }
}
