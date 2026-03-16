using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("")]
    protected Enemy enemy;
    [SerializeField, Tooltip("")]
    protected NavMeshAgent agent;
    [SerializeField, Tooltip("")]
    protected Animator animator;
    [SerializeField, Tooltip("")]
    protected GameObject targetPlayer;
    [SerializeField, Tooltip("")]
    protected PlayerHealth playerHealth;
    protected bool tutorialFrozen; //Raz

    [Header("Movement")]
    [SerializeField, Tooltip("")]
    protected float moveSpeed = 3f;
    [SerializeField, Tooltip("")]
    protected Vector3 lastKnownPlayerPos;

    [Header("Attack")]
    [SerializeField, Tooltip("")]
    protected float attackRange = 2f;
    [SerializeField, Tooltip("")]
    protected float timeBetweenAttacks = 1f;
    [SerializeField, Tooltip("")]
    protected float attackTimer = 0f;

    public virtual void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        targetPlayer = PlayerFinder.Instance.Player;
        playerHealth = targetPlayer.GetComponent<PlayerHealth>();
        agent.speed = moveSpeed;

        if (enemy == null) { Debug.LogWarning($"Enemy {name} is missing a Enemy component"); }
        if (agent == null) { Debug.LogWarning($"Enemy {name} is missing a navMeshAgent"); }
        if (animator == null) { Debug.LogWarning($"Enemy {name} is missing an Animator"); }
        if (targetPlayer == null) { Debug.LogWarning($"Enemy {name} could not find target player"); }
        if (playerHealth == null) { Debug.LogWarning($"Enemy {name} could not find player health"); }
    }

    public virtual void Update()
    {
        Move();
        CheckPlayerInRange();
    }

    public virtual void Move()
    {
        // Guard clauses
        if (agent == null) { return; }
        if (targetPlayer == null) { return; }
        if (!agent.isOnNavMesh) { Debug.LogWarning($"Enemy {name} is not on a NavMeshSurface"); return; }

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

    public abstract void CheckPlayerInRange();
    public abstract void Attack();
     public virtual void SetTutorialFrozen(bool isFrozen)
    {
        tutorialFrozen = isFrozen;

        if (agent != null && agent.isOnNavMesh)
        {
            if (isFrozen)
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }

            agent.isStopped = isFrozen;
        }

        if (animator != null)
        {
            animator.SetFloat("speed", 0f);
            animator.SetBool("isAttacking", false);
        }
    }
}
