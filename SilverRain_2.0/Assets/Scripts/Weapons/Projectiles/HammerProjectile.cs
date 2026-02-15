using UnityEngine;

public class HammerProjectile : Projectile
{
    [Header("Position Settings")]
    [Tooltip("The starting and ending rotation for the hammer projectile, set from the hammer object")]
    private float startRotation;
    [SerializeField, Tooltip("The lowest rotation point, where the hammer hits, then returns to the start rotation")]
    private float endRotation = 45f;
    [SerializeField, Tooltip("How far forward the hammer projectile will spawn relative to the camera")]
    protected float spawnOffsetForward;
    [SerializeField, Tooltip("How far above the hammer projectile will spawn relative to the camera")]
    protected float spawnOffsetUp;
    [SerializeField, Tooltip("The camera position that the hammer projectile follows")]
    private Transform cam;
    [Space]

    [Header("Hit Settings")]
    [SerializeField, Tooltip("The position from which the hammer sphere casts to check for hits")]
    private Transform hitPosition;
    [SerializeField, Tooltip("The radius of the sphere cast that check for hits")]
    private float hitRadius;
    [SerializeField, Tooltip("What layer will be hit by the hammer")]
    private LayerMask hitMask;
    [Space]

    [Header("Swing Settings")]
    [Tooltip("The current state of the swing, true = down")]
    private bool swingingDown = true;
    [Tooltip("The current rotation of the swing")]
    private float currentRotation;
    [SerializeField, Tooltip("The speed that the hammer swings up and down")]
    private float swingSpeed = 180f;
    //[Space]

    //[SerializeField, Tooltip("The key used to access the pool containing the hit VFX")]
    //private string hitVFXPoolKey;



    public void Init(Hammer parent, Transform camPos, float dmg, float size, float startRot)
    {
        parentWeapon = parent;
        cam = camPos;
        damage = dmg;
        hitRadius = size;
        startRotation = startRot;

        //Scale the gameobject based on size
        if (size < 1f) size = 1f;
        transform.localScale = Vector3.one * size;

        //Apply modifications
        parentWeapon.HandleProjectileSpawn();
    }

    private void Update()
    {
        if (cam == null) return;

        // Follow camera
        Vector3 desiredPos = cam.position + (cam.forward * spawnOffsetForward) + (cam.up * spawnOffsetUp);
        transform.position = desiredPos;
        transform.rotation = cam.rotation;

        // Rotate relative to the camera rotation
        float targetRot = swingingDown ? endRotation : startRotation;
        currentRotation = Mathf.MoveTowards(currentRotation, targetRot, swingSpeed * Time.deltaTime);
        transform.localRotation *= Quaternion.Euler(currentRotation, 0f, 0f);

        // At endRotation, perform Hit() and start swing upward
        if (swingingDown && Mathf.Approximately(currentRotation, endRotation))
        {
            Hit();
            swingingDown = false;
        }
        // After swinging up, return to pool
        else if (!swingingDown && Mathf.Approximately(currentRotation, startRotation))
        {
            WeaponManager.Instance.ProjectilePool.ReturnToPool(gameObject, PoolKey);
        }
    }

    private void Hit()
    {
        //Play the hit VFX
        //var hitEffect = WeaponManager.Instance.EffectsPool.Spawn(hitVFXPoolKey, hitPosition.position, Quaternion.identity);
        //hitEffect.GetComponent<HammerHitVFX>().Init(hitVFXPoolKey, hitRadius);

        //Get all colliders in the hit radius
        Collider[] hits;
        if (hitMask.value != 0)
        {
            //LayerMask ensures only specific layers are hit
            hits = Physics.OverlapSphere(hitPosition.position, hitRadius, hitMask);
        }
        else
        {
            Debug.LogWarning("HammerProjectile has no LayerMask of enemies to overlap");
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
    }

    public override void OnCreatedPool()
    {
    }

    public override void OnSpawnFromPool()
    {
        swingingDown = true;
        currentRotation = startRotation;

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
        Gizmos.DrawWireSphere(hitPosition.position, hitRadius);
    }
}