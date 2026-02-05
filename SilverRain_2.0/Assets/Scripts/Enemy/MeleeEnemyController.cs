using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyController : EnemyController
{
    private float meleeTimer = 0f;
    [SerializeField]
    private float timeBetweenAttacks = 1f;

    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private float attackRange = 2f;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        agent.speed = moveSpeed;
    }
    //private void OnTriggerStay(Collider collision)
    //{
        
    //    //Debug.Log("We have entered");
    //    //Check if player
    //    if (collision.CompareTag("Player")) 
    //    {
    //        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
    //        //Debug.Log("Found Health");
    //        //Add timer to deal damage
    //        meleeTimer += Time.deltaTime;
    //        animator.SetBool("isAttacking", true);

    //        //Check if time has passed;
    //        if (meleeTimer >= timeBetweenAttacks)
    //        {
    //            //Debug.Log("Timer passed");
    //            //Attack
    //            Attack(player);
                
    //        }

    //    }

    //}

    //private void OnTriggerExit(Collider collision)
    //{
    //    animator.SetBool("isAttacking", false);
    //    meleeTimer = 0f;
    //}

    private void Update()
    {
        Move();
        CheckPlayerInRange();
    }
    public override void Attack(PlayerHealth player)
    {
        //Debug.Log("Melee Enemy Attacking");
        meleeTimer += Time.deltaTime;
        if (meleeTimer >= timeBetweenAttacks)
        {
            if (targetPlayer != null)
            {
                player.TakeDamage(enemy.damage);
            }
            meleeTimer = 0;
        }
    }

    public override void Move()
    {
        //Move towards the player
        if (agent != null && agent.isOnNavMesh && targetPlayer != null)
        { agent.SetDestination(targetPlayer.transform.position);
            float speed = 0f;
            speed = agent.velocity.magnitude;
            animator.SetFloat("speed", speed);
        }
    }

    private void CheckPlayerInRange()
    {
        bool playerInRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (playerInRange && targetPlayer != null)
        {
            //Debug.Log("Melee Enemy Found Player");
            PlayerHealth player = targetPlayer.GetComponent<PlayerHealth>();
            if (player != null)
            {
                animator.SetBool("isAttacking", true);
                Attack(player);
            }
        }
        else
        {
            meleeTimer = 0f;
            animator.SetBool("isAttacking", false);
        }
    }

}