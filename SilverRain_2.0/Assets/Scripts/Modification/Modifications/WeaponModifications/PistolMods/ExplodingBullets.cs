using UnityEngine;

public class ExplodingBullets : Modification
{
    [SerializeField, Tooltip("The radius of the explosion")]
    private float explosionRadius = 10;
    [SerializeField, Tooltip("The damage dealt by the explosion")]
    private int explosionDamage = 10;

    [SerializeField, Tooltip("The layer mask for detecting hits")]
    private LayerMask hitMask;
    [SerializeField, Tooltip("The key used to access the pool containing the explosion VFX")]
    private string explosionVFXPoolKey;
    [SerializeField, Tooltip("The position of the explosion for gizmo visualization")]
    private Vector3 pos;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(OnWeaponHit);
    }

    public void OnWeaponHit(WeaponType type, GameObject[] objects, Vector3 position, Projectile proj)
    {
        pos = position;
        if (type != WeaponType.Pistol) return;
        Explode(position);
    }

    private void Explode(Vector3 position)
    {
        var explosion = ModificationManager.Instance.EffectsPool.Spawn(explosionVFXPoolKey, position, Quaternion.identity);
        explosion.GetComponent<ExplosionVFXController>().Init(explosionVFXPoolKey, explosionRadius);

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

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponHit.RemoveListener(OnWeaponHit);
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