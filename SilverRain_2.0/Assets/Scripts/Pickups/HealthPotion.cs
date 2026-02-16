using UnityEngine;

public class HealthPotion : Pickup
{
    [SerializeField,Tooltip("The amount of health the potion will restore.")]
    private float healAmount = 50f;

    public override void OnPickup()
    {
        var playerHealth = PlayerFinder.Instance.Player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }
        base.OnPickup();
    }
}
