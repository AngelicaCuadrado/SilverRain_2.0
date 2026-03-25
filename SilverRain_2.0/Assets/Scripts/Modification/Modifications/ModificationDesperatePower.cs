using UnityEngine;

public class ModificationDesperatePower : Modification, IStatModifier
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
                StatManager.Instance.UpdateTempStats(StatType.Cooldown);
                StatManager.Instance.UpdateTempStats(StatType.ProjectileSpeed);
            }
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();

        if (playerHealth != null)
        {
            playerHealth.onLowHealthStateChanged.RemoveListener(OnLowHealthStateChanged);
        }

        if (isActive)
        {
            isActive = false;
            StatManager.Instance.UpdateTempStats(StatType.Cooldown);
            StatManager.Instance.UpdateTempStats(StatType.ProjectileSpeed);
        }
    }

    private void OnLowHealthStateChanged(bool lowHealth)
    {
        if (isActive == lowHealth) return;

        isActive = lowHealth;
        StatManager.Instance.UpdateTempStats(StatType.Cooldown);
        StatManager.Instance.UpdateTempStats(StatType.ProjectileSpeed);
    }

    public float GetModifyValue(StatType type)
    {
        if (!isActive) return 0f;

        switch (type)
        {
            case StatType.Cooldown:
                return -0.3f;
            case StatType.ProjectileSpeed:
                return 0.3f;
            default:
                return 0f;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onLowHealthStateChanged.RemoveListener(OnLowHealthStateChanged);
        }
    }
}