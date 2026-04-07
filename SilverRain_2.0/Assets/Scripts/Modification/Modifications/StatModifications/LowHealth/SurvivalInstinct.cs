using UnityEngine;

public class SurvivalInstinct : Modification
{
    private PlayerHealth playerHealth;

    public override void Activate()
    {
        base.Activate();

        playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.hasSurvivalInstinct = true;
        }
    }
}