using System.Collections;
using UnityEngine;

public class GrenadeProjectile : Projectile
{
    [Header("Explosion Settings")]
    [SerializeField, Tooltip("How large the explosion is")]
    private float explosionRadius;
    [SerializeField, Tooltip("What layer will be hit by the explosion")]
    private LayerMask hitMask;

    public void Init(Grenade parent, float dmg, float size)
    {
        parentWeapon = parent;
        damage = dmg;
        explosionRadius = size;

        //Apply modifications
        parentWeapon.HandleProjectileSpawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    private void Explode()
    {
        //Get all colliders in the explosion radius
        Collider[] hits;
        if (hitMask.value != 0)
        {
            //LayerMask ensures only specific layers are hit
            hits = Physics.OverlapSphere(transform.position, explosionRadius, hitMask);
        }
        else
        {
            Debug.LogWarning("GrenadeProjectile has no LayerMask of enemies to overlap");
            return;
        }
        //Check if any enemies were hit
        if (hits.Length > 0)
        {
            //Try to get EnemyHealth component and apply damage
            foreach (var h in hits)
            {
                var enemyHealth = h.GetComponent<EnemyHealth>();
                if (enemyHealth == null)
                {
                    enemyHealth = h.GetComponentInParent<EnemyHealth>();
                }
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(Mathf.RoundToInt(damage));
                }
            }
            //Convert Collider array to GameObject array
            GameObject[] hitEnemies = new GameObject[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                hitEnemies[i] = hits[i].gameObject;
            }
            //Apply modifications
            parentWeapon.HandleWeaponHit(hitEnemies, transform.position);
        }
        //Return to pool
        WeaponManager.Instance.ProjectilePool.ReturnToPool(gameObject, PoolKey);
    }

    public override void OnCreatedPool()
    {
    }

    public override void OnSpawnFromPool()
    {
        //Start lifetime countdown
        if (lifeTime > 0f)
        {
            lifeCoroutine = StartCoroutine(LifeTimer());
        }
    }

    public override void OnReturnToPool()
    {
        //Stop any running timers and reset state
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }
        parentWeapon = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
