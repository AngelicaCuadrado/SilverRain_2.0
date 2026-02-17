using UnityEngine;

public class InvincibilityShield : Pickup
{
    [Header("Invincibility Shield Settings")]
    [SerializeField, Tooltip("The duration for which the invincibility shield will be active.")]
    private float invincibilityDuration = 10f;

    public override void OnPickup()
    {
        var playerHealth = PlayerFinder.Instance.Player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ActivateInvincibility(invincibilityDuration);
        }
        base.OnPickup();
    }
}