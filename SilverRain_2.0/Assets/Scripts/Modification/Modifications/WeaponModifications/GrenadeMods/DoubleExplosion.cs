using System.Collections;
using UnityEngine;
/// <summary>
/// Create a second, delayed, explosion at the same location as the first explosion
/// </summary>
public class DoubleExplosion : Modification
{
    [SerializeField,Tooltip("The delay time between the first and second explosion")]
    private float waitTime = 0.5f;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(HandleExplosion);
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

        // Mark this projectile as processed by DoubleExplosion so the delayed explosion's HandleWeaponHit
        // won't re-trigger a new delayed explosion.
        if (originalMeta == null) originalMeta = proj.gameObject.AddComponent<SpawnMetadata>();
        originalMeta.MarkProcessed(Id);

        GrenadeProjectile grenadeProj = proj as GrenadeProjectile;
        if (grenadeProj == null) return;

        StartCoroutine(DelayedExplosion(grenadeProj, position));
    }

    private IEnumerator DelayedExplosion(GrenadeProjectile proj, Vector3 position)
    {
        if (proj == null) yield break;

        // Delay before the second explosion
        yield return new WaitForSeconds(waitTime);

        // Play the explosion VFX
        var explosion = WeaponManager.Instance.EffectsPool.Spawn(proj.ExplosionVFXPoolKey, position, Quaternion.identity);
        explosion.GetComponent<GrenadeExplosionVFX>().Init(proj.ExplosionVFXPoolKey, proj.ExplosionRadius);

        // Get all colliders in the explosion radius
        Collider[] hits;
        if (proj.HitMask.value != 0)
        {
            // LayerMask ensures only specific layers are hit
            hits = Physics.OverlapSphere(position, proj.ExplosionRadius, proj.HitMask);
        }
        else
        {
            Debug.LogWarning("GrenadeProjectile has no LayerMask of enemies to overlap");
            yield break;
        }

        // Check if any enemies were hit
        if (hits.Length > 0)
        {
            // Try to get EnemyHealth component and apply damage
            foreach (var h in hits)
            {
                var enemyHealth = h.GetComponent<EnemyHealth>();
                if (enemyHealth == null)
                {
                    enemyHealth = h.GetComponentInParent<EnemyHealth>();
                }
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(Mathf.RoundToInt(proj.Damage));
                }
            }

            // Convert Collider array to GameObject array
            GameObject[] hitEnemies = new GameObject[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                hitEnemies[i] = hits[i].gameObject;
            }

            // Ensure projectile metadata is marked (defensive; should already be marked in HandleExplosion)
            var meta = proj.gameObject.GetComponent<SpawnMetadata>();
            if (meta == null) meta = proj.gameObject.AddComponent<SpawnMetadata>();
            meta.MarkProcessed(Id);

            // Apply modifications (this will notify OnWeaponHit for other mods)
            proj.ParentWeapon.HandleWeaponHit(hitEnemies, position, proj);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponHit.RemoveListener(HandleExplosion);
    }
}
