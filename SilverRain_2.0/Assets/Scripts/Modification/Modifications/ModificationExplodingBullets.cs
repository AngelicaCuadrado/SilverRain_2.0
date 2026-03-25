using UnityEngine;

public class ModificationExplodingBullets : Modification
{
    [SerializeField]
    GameObject explosionPrefab; //Testing purposes, change to vfx later
    [SerializeField]
    float explosionRadius = 10; //Testing value, needs refinement
    [SerializeField]
    int explosionDamage = 10; //Testing value, needs refinement
    [SerializeField]
    private LayerMask hitMask;
    [SerializeField, Tooltip("The key used to access the pool containing the explosion VFX")]
    private string explosionVFXPoolKey;

    private Vector3 pos;

    public override void Start()
    {
        base.Start();
        WeaponManager.Instance.OnWeaponAquired.AddListener(OnRequirementMet);
    }

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(OnWeaponHit);
        
    }

    public void OnWeaponHit(WeaponType type, GameObject[] objects, Vector3 position) 
    {
        pos = position;
        if (type != WeaponType.Pistol) return;
        Explode(position);
    }

    private void Explode(Vector3 position) 
    {
        var explosion = WeaponManager.Instance.EffectsPool.Spawn(explosionVFXPoolKey, position, Quaternion.identity);
        explosion.GetComponent<GrenadeExplosionVFX>().Init(explosionVFXPoolKey, explosionRadius);

        Collider[] hits = Physics.OverlapSphere(position, explosionRadius, hitMask);

        foreach (Collider hit in hits) 
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null) 
            {
                enemy.TakeDamage(explosionDamage);
            }
        }
    }

    public void OnRequirementMet(WeaponType type)
    {
        if (type == WeaponType.Pistol)
        {
            SetAvailable(true);
            return;
        }
        return;
    }

    private void OnDestroy()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnWeaponHit.RemoveListener(OnWeaponHit);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pos != null)
        {
            Gizmos.DrawWireSphere(pos, explosionRadius);
        }
    }

}
