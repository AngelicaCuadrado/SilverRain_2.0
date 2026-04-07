using UnityEngine;

public class CombatRecovery : Modification
{
    private PlayerHealth playerHealth;

    public override void Activate()
    {
        base.Activate();

        playerHealth = FindAnyObjectByType<PlayerHealth>();

        EnemyHealth.OnEnemyKilled += OnEnemyKilled;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        EnemyHealth.OnEnemyKilled -= OnEnemyKilled;
    }

    private void OnEnemyKilled(EnemyHealth enemy)
    {
        if (playerHealth == null) return;

        float healAmount = playerHealth.maxHealth * 0.03f;
        playerHealth.Heal(healAmount);
    }

    private void OnDestroy()
    {
        EnemyHealth.OnEnemyKilled -= OnEnemyKilled;
    }
}