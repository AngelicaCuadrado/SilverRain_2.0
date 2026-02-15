using UnityEngine;

public class ChakramProjectile : Projectile, IPoolable
{
    [Header("References")]
    [SerializeField, Tooltip("")]
    Transform playerTrans;
    [Space]

    [Header("Flight Settings")]
    [SerializeField, Tooltip("")]
    private float flightSpeed;
    [SerializeField, Tooltip("")]
    private Vector3 flightDirection;
    [SerializeField, Tooltip("")]
    private Vector3 firePointOffset;
    [Space]

    [Header("Spin Settings")]
    [SerializeField]
    private float spinSpeed = 120f;

    private bool returning = false;

    public void Init(Chakram parent, Transform player, Vector3 direction, float dmg, float duration, float size, float speed)
    {
        parentWeapon = parent;
        playerTrans = player;
        damage = dmg;
        flightSpeed = speed;

        //Scale the gameobject based on size
        if (size < 1f) size = 1f;
        transform.localScale = Vector3.one * size;

        // Find max distance based on duration
        flightDirection = transform.position + direction.normalized * duration;
        
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
        if (playerTrans == null) return;

        // Rotate around itself
        transform.Rotate(Vector3.forward, spinSpeed * flightSpeed * Time.deltaTime);

        if (!returning)
        {
            // Forward flight
            transform.position = Vector3.MoveTowards(transform.position, flightDirection, flightSpeed * Time.deltaTime);
            
            // Return when max distance is reached
            if (Vector3.Distance(transform.position, flightDirection) < 0.1f)
            {
                returning = true;
            }
        }
        else
        {
            // Backward flight
            transform.position = Vector3.MoveTowards(transform.position, playerTrans.position + firePointOffset, flightSpeed * Time.deltaTime);

            // Retun to pool when player position reached
            if (Vector3.Distance(transform.position, playerTrans.position + firePointOffset) < 0.5f)
            { WeaponManager.Instance.ProjectilePool.ReturnToPool(gameObject, PoolKey); }
        }

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
            return;
        }

        if (!returning)
        {
            returning = true;
        }
    }

    public override void OnCreatedPool() { }

    public override void OnSpawnFromPool() { }

    public override void OnReturnToPool()
    {
        //Stop any running timers and reset state
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }

        parentWeapon = null;
        returning = false;
    }
}
