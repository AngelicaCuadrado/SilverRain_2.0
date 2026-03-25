using UnityEngine;

public class BouncingBullets : Modification
{
    [SerializeField, Tooltip("")]
    private float bounceRange = 15f;

    public override void Start()
    {
        base.Start();
        WeaponManager.Instance.OnWeaponAquired.AddListener(OnRequirementMet);

        // Check initial weapon
        if (WeaponManager.Instance.InitialWeapon == WeaponType.Pistol)
        {
            OnRequirementMet(WeaponType.Pistol);
        }
    }

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(OnProjectileHit);
    }

    private void OnProjectileHit(WeaponType type, GameObject[] hitObjects, Vector3 hitPoint)
    {
        if (type != WeaponType.Pistol) return;

        // Find nearest enemy
        Transform target = FindNearestEnemy(hitPoint);
        if (target == null) return;

        // Spawn a new bullet
        SpawnBounceBullet(hitPoint, target);
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

    private void SpawnBounceBullet(Vector3 hitPoint, Transform target)
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

        proj.Init(
            pistol,
            pistol.WeaponStats.Damage,
            dir,
            pistol.WeaponStats.ProjectileSpeed
        );
    }

    public void OnRequirementMet(WeaponType type)
    {
        if (type == WeaponType.Pistol)
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
            WeaponManager.Instance.OnWeaponHit.RemoveListener(OnProjectileHit);
        }
    }
}
