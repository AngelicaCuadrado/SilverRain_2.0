using UnityEngine;

public class PanicRush : Modification, IStatModifier
{
    [SerializeField, Tooltip("Indicates whether the modification is currently active.")]
    private bool isActive = false;
    [SerializeField, Tooltip("The player's health component.")]
    private PlayerHealth playerHealth;

    public override void Activate()
    {
        base.Activate();

        playerHealth = PlayerFinder.Instance.Player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.onLowHealthStateChanged.AddListener(OnLowHealthStateChanged);

            bool lowHealth = playerHealth.GetHealthPercentage() <= 0.3f;
            if (isActive != lowHealth)
            {
                isActive = lowHealth;
                StatManager.Instance.UpdateTempStats(StatType.MovementSpeed);
            }
        }
    }

    private void OnLowHealthStateChanged(bool lowHealth)
    {
        if (isActive == lowHealth) return;

        isActive = lowHealth;
        StatManager.Instance.UpdateTempStats(StatType.MovementSpeed);
    }

    public float GetModifyValue(StatType type)
    {
        if (!isActive) return 0f;

        if (type == StatType.MovementSpeed)
        {
            return 0.5f;
        }

        return 0f;
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