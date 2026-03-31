using System.Collections;
using UnityEngine;

public class PistolProjectile : Projectile
{
    [Header("Movement Settings")]
    [Tooltip("The direction that the bullet will fly in")]
    private Vector3 direction;
    [Tooltip("The base speed for the projectile")]
    private float speed;
    [Tooltip("Indicates whether the projectile has bounced")]
    private bool hasBounced = false;

    // Properties
    public bool HasBounced => hasBounced;

    public void Init(Pistol parent, float dmg, Vector3 dir, float spd, bool bounced = false)
    {
        parentWeapon = parent;
        damage = dmg;
        direction = dir.normalized;
        speed = spd;
        hasBounced = bounced;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Try to find EnemyHealth on the object or its parent
        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            enemyHealth = other.GetComponentInParent<EnemyHealth>();
        }
        //Apply damage
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(Mathf.RoundToInt(damage));
            //Apply modifications
            GameObject[] hits = new[] { other.gameObject };
            parentWeapon.HandleWeaponHit(hits, transform.position, this);
        }
        //Return the projectile to the pool
        PoolOwner.ReturnToPool(gameObject, PoolKey);
    }

    //Called once when the pool initially creates the instance
    public override void OnCreatedPool() { }

    //Called whenever the pool spawns this instance
    public override void OnSpawnFromPool()
    {
        //Start lifetime countdown
        if (lifeTime > 0f)
        {
            lifeCoroutine = StartCoroutine(LifeTimer());
        }
    }

    //Called before the pool deactivates this instance
    public override void OnReturnToPool()
    {
        // Let base handle coroutine stop and SpawnMetadata reset
        base.OnReturnToPool();

        direction = Vector3.zero;
    }

    // Draw gizmos to visualize the projectile's path in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + direction * 5f);
    }
}