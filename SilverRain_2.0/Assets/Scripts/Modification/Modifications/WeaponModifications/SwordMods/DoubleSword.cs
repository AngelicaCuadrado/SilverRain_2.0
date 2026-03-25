using Unity.VisualScripting;
using UnityEngine;

public class DoubleSword : Modification
{
    [SerializeField, Tooltip("")]
    private float startAngle = 270f;
    [SerializeField, Tooltip("")]
    private int maxSwords = 1;
    [SerializeField, Tooltip("")]
    private int currentSwords = 0;

    public override void Start()
    {
        base.Start();
        WeaponManager.Instance.OnWeaponAquired.AddListener(OnRequirementMet);
        
        // Check initial weapon
        if(WeaponManager.Instance.InitialWeapon == WeaponType.Sword)
        {
            OnRequirementMet(WeaponType.Sword);
        }
    }

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponProjectileSpawn.AddListener(OnProjectileSpawn);

    }
    public void OnProjectileSpawn(WeaponType type, Weapon weapon, GameObject originalProjectile)
    {
        if (currentSwords >= maxSwords)
        {
            currentSwords = 0;
            return;
        }
        
        if (type != WeaponType.Sword) return;

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

        currentSwords++;
        sword.HandleProjectileSpawn(projObj);
    }

    public void OnRequirementMet(WeaponType type)
    {
        if (type == WeaponType.Sword)
        {
            SetAvailable(true);
            WeaponManager.Instance.OnWeaponAquired.RemoveListener(OnRequirementMet);
        }
    }

    private void OnDestroy()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnWeaponAquired.RemoveListener(OnRequirementMet);
            WeaponManager.Instance.OnWeaponProjectileSpawn.RemoveListener(OnProjectileSpawn);
        }
    }
}