using UnityEngine;

public class BouncingBullets : Modification
{
    [SerializeField, Tooltip("The number of times a bullet can bounce to another enemy")]
    private int bounceAmount = 2;
    [SerializeField, Tooltip("The maximum range at which a bullet can bounce to another enemy")]
    private float bounceRange = 15f;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(OnProjectileHit);
    }

    private void OnProjectileHit(WeaponType type, GameObject[] hitObjects, Vector3 hitPoint, Projectile proj)
    {
        if (type != WeaponType.Pistol) return;
        if (proj == null) return;

        // Ensure we're working with a pistol projectile
        PistolProjectile pistolProj = proj as PistolProjectile;
        if (pistolProj == null) return;

        // Use SpawnMetadata to avoid recursion and to allow a limited bounce chain.
        var originalMeta = proj.gameObject.GetComponent<SpawnMetadata>();
        // Prevent handling the same projectile multiple times by this mod
        if (originalMeta != null && originalMeta.HasProcessed(Id))
        {
            return;
        }

        int generation = originalMeta != null ? originalMeta.Generation : 0;
        // Stop if this projectile already reached the max bounce generation
        if (generation >= bounceAmount)
        {
            return;
        }

        // Find nearest enemy
        Transform target = FindNearestEnemy(hitPoint);
        if (target == null) return;

        // Spawn a new bullet that will head toward the found target
        SpawnBounceBullet(hitPoint, target, generation);

        // Mark the original projectile as processed by this modification so we don't spawn again from it
        if (originalMeta == null) originalMeta = proj.gameObject.AddComponent<SpawnMetadata>();
        originalMeta.MarkProcessed(Id);
    }

    private Transform FindNearestEnemy(Vector3 fromPos)
    {
        Collider[] hits = Physics.OverlapSphere(
            fromPos,
            bounceRange,
            LayerMask.GetMask("Enemy")
        );

        float bestDist = float.MaxValue;
        Transform best = null;

        foreach (var hit in hits)
        {
            float d = (hit.transform.position - fromPos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = hit.transform;
            }
        }

        return best;
    }

    private void SpawnBounceBullet(Vector3 hitPoint, Transform target, int parentGeneration)
    {
        // Direction toward the new target
        Vector3 dir = (target.position - hitPoint).normalized;

        // Spawn projectile from pool
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(
            WeaponManager.Instance.AllWeapons[WeaponType.Pistol].ProjectilePoolKey,
            hitPoint,
            Quaternion.LookRotation(dir)
        );

        // Initialize projectile like a normal pistol bullet
        var pistol = WeaponManager.Instance.CurrentWeapons[WeaponType.Pistol] as Pistol;
        var proj = projObj.GetComponent<PistolProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("BouncingBullets: spawned projectile missing PistolProjectile");
            Destroy(projObj);
            return;
        }

        // Keep using the bounced flag for any logic in the projectile itself, but bounce control is now via SpawnMetadata
        proj.Init(
            pistol,
            pistol.WeaponStats.Damage,
            dir,
            pistol.WeaponStats.ProjectileSpeed,
            true
        );

        // Ensure metadata exists on spawned projectile, set generation (parent + 1)
        var newMeta = projObj.GetComponent<SpawnMetadata>();
        if (newMeta == null) newMeta = projObj.AddComponent<SpawnMetadata>();
        newMeta.Generation = parentGeneration + 1;

        // Notify other mods / systems that a projectile was spawned (so exploding bullets, etc. will still apply)
        pistol?.HandleProjectileSpawn(projObj);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponHit.RemoveListener(OnProjectileHit);
    }
}