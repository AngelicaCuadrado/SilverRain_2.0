using UnityEngine;

public class ModificationStableEnergy : Modification, IStatModifier
{
    private bool isActive = false;
    private PlayerHealth playerHealth;

    public override void Activate()
    {
        base.Activate();

        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.onHighHealthStateChanged.AddListener(OnHighHealthStateChanged);

            bool highHealth = playerHealth.GetHealthPercentage() >= 0.8f;
            if (isActive != highHealth)
            {
                isActive = highHealth;
                StatManager.Instance.UpdateTempStats(StatType.Duration);
            }
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();

        if (playerHealth != null)
        {
            playerHealth.onHighHealthStateChanged.RemoveListener(OnHighHealthStateChanged);
        }

        if (isActive)
        {
            isActive = false;
            StatManager.Instance.UpdateTempStats(StatType.Duration);
        }
    }

    private void OnHighHealthStateChanged(bool highHealth)
    {
        if (isActive == highHealth) return;

        isActive = highHealth;
        StatManager.Instance.UpdateTempStats(StatType.Duration);
    }

    public float GetModifyValue(StatType type)
    {
        if (!isActive) return 0f;

        if (type == StatType.Duration)
        {
            return 0.5f;
        }

        return 0f;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onHighHealthStateChanged.RemoveListener(OnHighHealthStateChanged);
        }
    }
}