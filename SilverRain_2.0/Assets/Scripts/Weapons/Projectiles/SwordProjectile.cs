using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SwordProjectile : Projectile
{
    float angle = 90f;
    float rotationRadius = 0f;
    float rotationSpeed;
    Transform playerTrans;

    public void Init(Sword parent, Transform player, float dmg, float duration, float size, float speed)
    {
        parentWeapon = parent;
        playerTrans = player;
        damage = dmg;
        lifeTime = duration;
        rotationRadius = size;
        rotationSpeed = 180f * speed;

        //Scale the gameobject based on size
        if (size < 1f) size = 1f;
        transform.localScale = Vector3.one * size;

        //Start lifetime countdown
        if (lifeTime > 0f)
        {
            lifeCoroutine = StartCoroutine(LifeTimer());
        }

        //Apply modifications
        parentWeapon.HandleProjectileSpawn();
    }
    private void Update()
    {
        //Rotate around the player
        angle += rotationSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * rotationRadius;

        //Stay centered on the player
        transform.position = playerTrans.position + offset;

        //Ignore player's rotation
        Vector3 dir = (playerTrans.position - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = look * Quaternion.Euler(-90f, 0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("sword hit");
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
    }

    public override void OnCreatedPool()
    {
    }

    //Called whenever the pool spawns this instance
    public override void OnSpawnFromPool()
    {

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

        angle = 0f;
        parentWeapon = null;
    }
}
