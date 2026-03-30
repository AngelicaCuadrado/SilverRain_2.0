using UnityEngine;

public class DoubleSword : Modification
{
    [SerializeField, Tooltip("The starting angle for the second sword projectile")]
    private float startAngle = 270f;
    [SerializeField, Tooltip("The maximum number of swords that can be spawned")]
    private int maxSwords = 1;
    [SerializeField, Tooltip("The current number of swords spawned")]
    private int currentSwords = 0;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponProjectileSpawn.AddListener(OnProjectileSpawn);
    }

    public void OnProjectileSpawn(WeaponType type, Weapon weapon, GameObject originalProjectile)
    {
        if (type != WeaponType.Sword) return;
        if (weapon == null || originalProjectile == null) return;

        // Avoid re-processing projectiles this mod already handled
        var originalMeta = originalProjectile.GetComponent<SpawnMetadata>();
        if (originalMeta != null && originalMeta.HasProcessed(Id))
        {
            return;
        }

        if (currentSwords >= maxSwords)
        {
            currentSwords = 0;
            return;
        }

        // Get original rotation
        Quaternion originalRot = originalProjectile.transform.rotation;

        // Mirror it 180 degrees
        Quaternion mirroredRot = originalRot * Quaternion.Euler(0f, 180f, 0f);

        // Get player position
        Transform player = PlayerFinder.Instance.Player.transform;

        // Spawn second projectile
        Vector3 spawnPos = player.position - player.forward * 90f;

        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(
            weapon.ProjectilePoolKey,
            spawnPos,
            mirroredRot
        );

        var proj = projObj.GetComponent<SwordProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("DoubleSword: spawned projectile missing SwordProjectile");
            Destroy(projObj);
            return;
        }

        var sword = weapon as Sword;
        if (sword == null) return;

        // Initialize with same stats
        proj.Init(
            sword,
            player,
            weapon.WeaponStats.Damage,
            originalProjectile.GetComponent<SwordProjectile>().LifeTime,
            weapon.WeaponStats.Size,
            weapon.WeaponStats.ProjectileSpeed,
            startAngle
        );

        // Ensure metadata exists on spawned projectile, set generation and mark processed by this mod
        var newMeta = projObj.GetComponent<SpawnMetadata>();
        if (newMeta == null) newMeta = projObj.AddComponent<SpawnMetadata>();
        newMeta.Generation = (originalMeta != null) ? originalMeta.Generation + 1 : 1;
        newMeta.MarkProcessed(Id);

        currentSwords++;
        // Let other mods react to this spawned projectile (DoubleSword won't process it again because it's marked)
        sword.HandleProjectileSpawn(projObj);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponProjectileSpawn.RemoveListener(OnProjectileSpawn);
    }
}