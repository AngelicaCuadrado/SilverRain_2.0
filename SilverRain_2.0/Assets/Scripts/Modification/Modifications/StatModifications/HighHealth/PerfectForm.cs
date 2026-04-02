using UnityEngine;

public class PerfectForm : Modification, IStatModifier
{
    [SerializeField,Tooltip("Indicates whether the modification is currently active.")]
    private bool isActive = false;
    [SerializeField, Tooltip("The player's health component.")]
    private PlayerHealth playerHealth;

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
                StatManager.Instance.UpdateTempStats(StatType.AttackDamage);
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
            StatManager.Instance.UpdateTempStats(StatType.AttackDamage);
        }
    }

    private void OnHighHealthStateChanged(bool highHealth)
    {
        if (isActive == highHealth) return;

        isActive = highHealth;
        StatManager.Instance.UpdateTempStats(StatType.AttackDamage);
    }

    public float GetModifyValue(StatType type)
    {
        if (!isActive) return 0f;

        if (type == StatType.AttackDamage)
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
            playerHealth.onHighHealthStateChanged.RemoveListener(OnHighHealthStateChanged);
        }
    }
}