using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// After the initial explosion, the hammer creates increasingly smaller explosions on each enemy hit up to
/// a configurable depth. Each chain explosion has reduced radius and damage. Explosions spawned by this mod
/// will not re-hit the same enemy for the duration of the chain (per source projectile).
/// </summary>
public class ChainReaction : Modification
{
    [SerializeField, Tooltip("Maximum number of explosions in the chain reaction")]
    private int maxExplosions = 2;
    [SerializeField, Tooltip("Damage reduction factor applied per subsequent explosion")]
    private float damageReductionPerExplosion = 0.2f;
    [SerializeField, Tooltip("Radius multiplier applied per subsequent explosion")]
    private float radiusReductionPerExplosion = 0.5f;
    [SerializeField, Tooltip("Fallback radius used if projectile does not expose a radius")]
    private float fallbackRadius = 2f;
    [SerializeField, Tooltip("Pool key for chain explosion VFX")]
    private string poolKey = "ChainReaction";

    // Tracks active chain sets per source projectile instance ID (so concurrent hammers don't interfere).
    private readonly Dictionary<int, HashSet<int>> activeChains = new();

    // When this flag is true we ignore OnWeaponHit events produced by our own chain explosions.
    private bool suppressSelf = false;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(HandleExplosion);
    }

    private void HandleExplosion(WeaponType type, GameObject[] targets, Vector3 position, Projectile proj)
    {
        if (type != WeaponType.Hammer) return;
        if (proj == null) return;
        if (suppressSelf) return;

        // Get radius from the projectile if it is a HammerProjectile, otherwise use fallback
        float initialRadius = fallbackRadius;
        if (proj is HammerProjectile hammerProj)
        {
            initialRadius = hammerProj.HitRadius;
        }

        // Key the chain by the source projectile instance id so multiple simultaneous chains are independent.
        int sourceId = proj.gameObject.GetInstanceID();

        // Prepare set of enemies already hit by this chain to avoid re-hitting them.
        if (!activeChains.ContainsKey(sourceId))
        {
            activeChains[sourceId] = new HashSet<int>();
        }

        var alreadyHit = activeChains[sourceId];

        // Add the initial targets (those hit by the original explosion) to the already-hit set,
        // and start a chain explosion centered on each of them.
        if (targets != null)
        {
            foreach (var targetGo in targets)
            {
                if (targetGo == null) continue;

                // Resolve EnemyHealth to identify the enemy root
                var eh = targetGo.GetComponent<EnemyHealth>() ?? targetGo.GetComponentInParent<EnemyHealth>();
                if (eh == null) continue;

                int enemyId = eh.gameObject.GetInstanceID();
                if (alreadyHit.Contains(enemyId)) continue;
                alreadyHit.Add(enemyId);

                // Start chain from this enemy (first chained explosion is count = 1)
                CreateExplosion(eh.transform.position, proj, sourceId, 1, proj.Damage, initialRadius);
            }
        }

        // Chain tracking can be removed here because CreateExplosion populates the set as it recurses.
        activeChains.Remove(sourceId);
    }

    /// <summary>
    /// Creates a chain explosion at position. explosionCount indicates how many chained explosions deep we are
    /// (1 = first chained explosion after the initial hit).
    /// </summary>
    private void CreateExplosion(Vector3 position, Projectile sourceProj, int sourceId, int explosionCount, float baseDamage, float baseRadiusLocal)
    {
        if (explosionCount > maxExplosions) return;

        // Compute the damage and radius for this explosion step
        float damageMultiplier = Mathf.Pow(1f - damageReductionPerExplosion, explosionCount);
        int damageToDeal = Mathf.RoundToInt(baseDamage * damageMultiplier);
        float radius = baseRadiusLocal * Mathf.Pow(radiusReductionPerExplosion, explosionCount - 1);

        // Spawn chain explosion VFX from the ModificationManager's effects pool using the "ChainReaction" key.
        if (ModificationManager.Instance != null && ModificationManager.Instance.EffectsPool != null)
        {
            var vfxObj = ModificationManager.Instance.EffectsPool.Spawn(poolKey, position, Quaternion.identity);
            var vfx = vfxObj?.GetComponent<GrenadeExplosionVFX>();
            if (vfx != null)
            {
                vfx.Init(poolKey, radius);
            }
        }

        // Find enemies in the explosion radius
        Collider[] hits = Physics.OverlapSphere(position, radius, LayerMask.GetMask("Enemy"));
        if (hits == null || hits.Length == 0) return;

        // Ensure the active chain set still exists (it can, but defensive check)
        if (!activeChains.TryGetValue(sourceId, out var alreadyHit))
        {
            // If the source tracking no longer exists, create it so we don't re-hit enemies
            alreadyHit = new HashSet<int>();
            activeChains[sourceId] = alreadyHit;
        }

        // Collect GameObjects hit for notifying other mods
        List<GameObject> hitGameObjects = new List<GameObject>();

        foreach (var c in hits)
        {
            if (c == null) continue;

            var eh = c.GetComponent<EnemyHealth>() ?? c.GetComponentInParent<EnemyHealth>();
            if (eh == null) continue;

            int enemyId = eh.gameObject.GetInstanceID();

            // Do not hit the same enemy more than once in this chain
            if (alreadyHit.Contains(enemyId)) continue;

            alreadyHit.Add(enemyId);

            // Apply damage
            eh.TakeDamage(damageToDeal);

            hitGameObjects.Add(eh.gameObject);

            // Schedule next chained explosion centered on this enemy (depth-first)
            CreateExplosion(eh.transform.position, sourceProj, sourceId, explosionCount + 1, baseDamage, baseRadiusLocal);
        }

        // Notify other mods/systems about these hits (but suppress our own listener to avoid re-entering and creating duplicate chains)
        if (hitGameObjects.Count > 0)
        {
            try
            {
                suppressSelf = true;
                // Use the original source projectile when notifying, so other mods see the same weapon context.
                sourceProj?.ParentWeapon.HandleWeaponHit(hitGameObjects.ToArray(), position, sourceProj);
            }
            finally
            {
                suppressSelf = false;
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponHit.RemoveListener(HandleExplosion);
    }

    // Optional: visualize the chain explosion radius in the editor for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, fallbackRadius);
    }
}