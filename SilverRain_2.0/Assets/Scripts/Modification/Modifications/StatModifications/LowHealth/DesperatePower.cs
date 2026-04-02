using UnityEngine;

public class DesperatePower : Modification, IStatModifier
{
    [SerializeField, Tooltip("Indicates whether the modification is currently active.")]
    private bool isActive = false;
    [SerializeField, Tooltip("The player's health component.")]
    private PlayerHealth playerHealth;

    public override void Activate()
    {
        base.Activate();

        // Attempt to find the player's health component if it hasn't been assigned in the inspector
        playerHealth = PlayerFinder.Instance.Player.GetComponent<PlayerHealth>();

        // Subscribe to the low health state change event
        if (playerHealth != null)
        {
            playerHealth.onLowHealthStateChanged.AddListener(OnLowHealthStateChanged);

            bool lowHealth = playerHealth.GetHealthPercentage() <= 0.3f;
            // Ensure the modification's active state is correctly set based on the player's current health status
            if (isActive != lowHealth)
            {
                isActive = lowHealth;
                // Update the stats to reflect the current state of the modification
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

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (playerHealth != null)
        {
            playerHealth.onLowHealthStateChanged.RemoveListener(OnLowHealthStateChanged);
        }
    }
}