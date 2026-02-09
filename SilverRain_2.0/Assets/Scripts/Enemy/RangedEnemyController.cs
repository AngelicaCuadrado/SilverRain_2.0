using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyController : EnemyController
{
    private float shootTimer = 0f;
    [SerializeField] private float timeBetweenShots = 2f;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject projectilePrefab;
    private Vector3 lastKnownPlayerPos;


    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private float attackRange = 10f;
    
    private ObjectPooler pooler;
    private string poolKey = "EnemyProjectilePool";
    

    public override void Attack(PlayerHealth player)
    {
        shootTimer += Time.deltaTime;
        //Spawn projectile targetting playerTrans.
        if (shootTimer >= timeBetweenShots)
        {
            if (targetPlayer != null)
            {
                animator.SetTrigger("attacking");
                Vector3 dir = (targetPlayer.transform.position - firePoint.position).normalized;
                
                GameObject go = //Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                    pooler.Spawn(poolKey, firePoint.position, Quaternion.LookRotation(dir));
                
                go.GetComponent<EnemyProjectile>().Initialize(dir, enemy.damage, player);
                if (go == null) return;
                
                var projectile = go.GetComponent<EnemyProjectile>();
                if (projectile != null)
                {
                    projectile.pooler = EnemyManager.Instance.enemyProjectilePool;
                    projectile.PoolKey = poolKey;
                }
            }
            shootTimer = 0;
        }

    }

    public override void Move()
    {
        //Check that the player in on the navesh
        if (agent == null || !agent.isOnNavMesh) { return; }
        
        if (targetPlayer != null)
        {
            //Check if the player's position is on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPlayer.transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                //Move towards the player
                agent.SetDestination(hit.position);
                //Cache last known position
                lastKnownPlayerPos = hit.position;
            }
            else
            {
                //Player is off the NavMesh, move to last known position
                agent.SetDestination(lastKnownPlayerPos);
            }
            //Set animator speed
            float speed = agent.velocity.magnitude;
            animator.SetFloat("speed", speed);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        agent.speed = moveSpeed;

        pooler = EnemyManager.Instance.enemyProjectilePool;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckPlayerInRange();
    }

    private void CheckPlayerInRange() 
    {
        if (Physics.CheckSphere(transform.position, attackRange, playerLayer))
        {
            PlayerHealth player = targetPlayer.GetComponent<PlayerHealth>();
            if (player != null)
            {
                Attack(player);
            }
        }
        else 
        {
            shootTimer = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.GetComponent<PlayerHealth>()) 
    //    {
    //        PlayerHealth target = other.GetComponent<PlayerHealth>();
    //        Attack(target);
    //    }
    //}

    //private void OnTriggerExit(Collider collision)
    //{
    //    shootTimer = 0f;
    //}
}
