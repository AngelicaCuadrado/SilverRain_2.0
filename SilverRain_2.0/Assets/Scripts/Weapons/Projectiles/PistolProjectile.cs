using System.Collections;
using UnityEngine;

public class PistolProjectile : Projectile
{
    //Movement
    private Vector3 direction;
    private float speed;

    public void Init(Pistol parent, float dmg, Vector3 dir, float spd)
    {
        parentWeapon = parent;
        damage = dmg;
        direction = dir.normalized;
        speed = spd;

        //Apply modifications
        parentWeapon.HandleProjectileSpawn();
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
            parentWeapon.HandleWeaponHit(hits, transform.position);   
        }
        //Return the projectile to the pool
        WeaponManager.Instance.ProjectilePool.ReturnToPool(gameObject);
    }

    //Called once when the pool initially creates the instance
    public override void OnCreatedPool()
    {
    }

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
        //Stop any running timers and reset state
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }

        direction = Vector3.zero;
        parentWeapon = null;
    }
}