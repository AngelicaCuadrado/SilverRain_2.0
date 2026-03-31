using UnityEngine;

public class ModificationPrecisionShot : Modification, IStatModifier
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
                StatManager.Instance.UpdateTempStats(StatType.ProjectileSpeed);
                StatManager.Instance.UpdateTempStats(StatType.Size);
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
            StatManager.Instance.UpdateTempStats(StatType.ProjectileSpeed);
            StatManager.Instance.UpdateTempStats(StatType.Size);
        }
    }

    private void OnHighHealthStateChanged(bool highHealth)
    {
        if (isActive == highHealth) return;

        isActive = highHealth;
        StatManager.Instance.UpdateTempStats(StatType.ProjectileSpeed);
        StatManager.Instance.UpdateTempStats(StatType.Size);
    }

    public float GetModifyValue(StatType type)
    {
        if (!isActive) return 0f;

        switch (type)
        {
            case StatType.ProjectileSpeed:
                return 0.3f;
            case StatType.Size:
                return 0.2f;
            default:
                return 0f;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onHighHealthStateChanged.RemoveListener(OnHighHealthStateChanged);
        }
    }
}