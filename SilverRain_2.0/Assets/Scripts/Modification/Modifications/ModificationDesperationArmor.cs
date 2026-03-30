using UnityEngine;

public class ModificationDesperationArmor : Modification, IStatModifier
{
    private bool isActive = false;
    private PlayerHealth playerHealth;

    public override void Activate()
    {
        base.Activate();

        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.onLowHealthStateChanged.AddListener(OnLowHealthStateChanged);

            bool lowHealth = playerHealth.GetHealthPercentage() <= 0.3f;
            if (isActive != lowHealth)
            {
                isActive = lowHealth;
                StatManager.Instance.UpdateTempStats(StatType.Armor);
            }
        }
    }

    private void OnLowHealthStateChanged(bool lowHealth)
    {
        if (isActive == lowHealth) return;

        isActive = lowHealth;
        StatManager.Instance.UpdateTempStats(StatType.Armor);
    }

    public float GetModifyValue(StatType type)
    {
        if (!isActive) return 0f;

        if (type == StatType.Armor)
        {
            return 0.5f;
        }

        return 0f;
    }

    public override void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onLowHealthStateChanged.RemoveListener(OnLowHealthStateChanged);
        }
    }
}