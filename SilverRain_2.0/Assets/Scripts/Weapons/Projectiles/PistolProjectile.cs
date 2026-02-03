using System.Collections;
using UnityEngine;

public class PistolProjectile : MonoBehaviour, IPoolable
{
    private Pistol parentGun;
    private float damage;
    //Movement
    private Vector3 direction;
    private float speed;
    //Lifetime before returning to pool
    [SerializeField] private float lifeTime = 5f;
    private Coroutine lifeCoroutine;

    public void Init(Pistol parent, float dmg, Vector3 dir, float spd)
    {
        parentGun = parent;
        damage = dmg;
        direction = dir.normalized;
        speed = spd;
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
            parentGun.HandleWeaponHit(transform.position);
        }
        //Return the projectile to the pool
        WeaponManager.Instance.ProjectilePool.ReturnToPool(gameObject);
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        if (gameObject.activeInHierarchy)
        {
            WeaponManager.Instance.ProjectilePool.ReturnToPool(gameObject);
        }
    }

    //Called once when the pool initially creates the instance
    public void OnCreatedPool()
    {
    }

    //Called whenever the pool spawns this instance
    public void OnSpawnFromPool()
    {
        //Start lifetime countdown
        if (lifeTime > 0f)
        {
            lifeCoroutine = StartCoroutine(LifeTimer());
        }
    }

    //Called before the pool deactivates this instance
    public void OnReturnToPool()
    {
        //Stop any running timers and reset state
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }

        direction = Vector3.zero;
        parentGun = null;
    }
}


//---------------------------- TEMPORARY TO AVOID ERRORS, DELETE ALL OF THIS ----------------------------
public class EnemyHealth : MonoBehaviour
{
    public void TakeDamage(int damageAmount) { }
}
