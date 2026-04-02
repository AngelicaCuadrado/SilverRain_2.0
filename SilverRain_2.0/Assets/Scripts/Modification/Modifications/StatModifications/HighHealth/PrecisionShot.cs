using UnityEngine;

public class PrecisionShot : Modification, IStatModifier
{
    [SerializeField,Tooltip("Indicates whether the modification is currently active.")]
    private bool isActive = false;
    [SerializeField, Tooltip("The player's health component.")]
    private PlayerHealth playerHealth;
    [SerializeField, Tooltip("The percentage increase in projectile speed when the modification is active.")]
    private float projectileSpeedModifier = 0.3f; // 30% increase in projectile speed
    [SerializeField, Tooltip("The percentage increase in projectile size when the modification is active.")]
    private float sizeModifier = 0.2f; // 20% increase in projectile size

    public override void Activate()
    {
        base.Activate();

        playerHealth = PlayerFinder.Instance.Player.GetComponent<PlayerHealth>();
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

        return type switch
        {
            StatType.ProjectileSpeed => projectileSpeedModifier,
            StatType.Size => sizeModifier,
            _ => 0f,
        };
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (playerHealth != null)
        {
            playerHealth.onHighHealthStateChanged.RemoveListener(OnHighHealthStateChanged);
        }
    }
}