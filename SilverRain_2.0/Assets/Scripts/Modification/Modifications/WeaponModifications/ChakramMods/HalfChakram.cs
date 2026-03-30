using UnityEngine;

public class HalfChakram : Modification
{
    [SerializeField, Tooltip("The multiplier for the duration of the chakram projectile")]
    private float multiplier = 0.5f;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponProjectileSpawn.AddListener(OnProjectileSpawn);
    }

    private void OnProjectileSpawn(WeaponType type, Weapon weapon, GameObject originalProjectile)
    {
        if (type != WeaponType.Chakram) return;
        if (weapon == null || originalProjectile == null) return;

        // Don't re-process projectiles we've already handled
        var originalMeta = originalProjectile.GetComponent<SpawnMetadata>();
        if (originalMeta != null && originalMeta.HasProcessed(Id))
        {
            return;
        }

        var chakram = weapon as Chakram;
        if (chakram == null) return;

        Vector3 spawnPos = originalProjectile.transform.position;
        Quaternion spawnRot = originalProjectile.transform.rotation;
        Vector3 direction = originalProjectile.transform.forward;

        Transform player = PlayerFinder.Instance.Player.transform;
        if (player == null)
        {
            Debug.LogWarning("HalfChakram: could not find player transform.");
            return;
        }

        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(
            weapon.ProjectilePoolKey,
            spawnPos,
            spawnRot
        );

        var proj = projObj.GetComponent<ChakramProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("HalfChakram: spawned projectile missing ChakramProjectile");
            Destroy(projObj);
            return;
        }

        float dmg = weapon.WeaponStats.Damage;
        float halfSpeed = weapon.WeaponStats.ProjectileSpeed * multiplier;
        float halfDuration = weapon.WeaponStats.Duration * multiplier;
        float halfSize = weapon.WeaponStats.Size * multiplier;
       
        // Initialize the chakram projectile with half duration and half size
        proj.Init(
            chakram,
            player,
            direction,
            dmg,
            halfDuration,
            halfSize,
            halfSpeed
        );

        // Ensure metadata exists and set generation + mark this mod as processed on the spawned projectile
        var newMeta = projObj.GetComponent<SpawnMetadata>();
        if (newMeta == null) newMeta = projObj.AddComponent<SpawnMetadata>();
        newMeta.Generation = (originalMeta != null) ? originalMeta.Generation + 1 : 1;
        newMeta.MarkProcessed(Id);

        // Let other mods react to the half-chakram, but HalfChakram won't process it again
        chakram.HandleProjectileSpawn(projObj);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponProjectileSpawn.RemoveListener(OnProjectileSpawn);
    }
}