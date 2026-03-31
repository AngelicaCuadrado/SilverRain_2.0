using UnityEngine;

public class FalloutZone : Modification, IWeaponModifier
{
    [SerializeField, Tooltip("The percentage increase in damage")]
    private float damageModifier = 0.5f;
    [SerializeField, Tooltip("The percentage decrease in cooldown")]
    private float cooldownModifier = -0.2f;
    [SerializeField, Tooltip("The percentage decrease in damage to players")]
    private float damageToPlayerModifier = 0.5f;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(HandleExplosion);
    }

    public float GetModifyValue(WeaponType weapon, StatType stat)
    {
        return stat switch
        {
            StatType.AttackDamage => damageModifier,
            StatType.Cooldown => cooldownModifier,
            _ => 0,
        };
    }

    private void HandleExplosion(WeaponType type, GameObject[] targets, Vector3 position, Projectile proj)
    {
        if (type != WeaponType.Grenade) return;
        if (proj == null) return;

        // Prevent recursion: skip if this projectile was already processed by this modification.
        var originalMeta = proj.gameObject.GetComponent<SpawnMetadata>();
        if (originalMeta != null && originalMeta.HasProcessed(Id))
        {
            return;
        }

        // Mark this projectile as processed by FalloutZone
        if (originalMeta == null) originalMeta = proj.gameObject.AddComponent<SpawnMetadata>();
        originalMeta.MarkProcessed(Id);

        // Only damage player(s) at the explosion position, no VFX, no OnHit notifications.
        GrenadeProjectile grenadeProj = proj as GrenadeProjectile;
        if (grenadeProj == null) return;

        LayerMask playerMask = LayerMask.GetMask("Player");
        if (playerMask.value == 0)
        {
            Debug.LogWarning("FalloutZone: 'Player' layer not found for overlap check.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(position, grenadeProj.ExplosionRadius, playerMask);
        if (hits.Length == 0) return;

        int damageToDeal = Mathf.RoundToInt(proj.Damage * damageToPlayerModifier);
        foreach (var h in hits)
        {
            var playerHealth = h.GetComponent<PlayerHealth>() ?? h.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToDeal);
                // Mark the player as processed to prevent multiple hits from the same explosion.
                return;
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponHit.RemoveListener(HandleExplosion);
    }
}