using UnityEngine;
using UnityEngine.AI;


public class EnemyStateManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    
    public Transform player;

    // declare states
    EnemyBaseState currentState;
    public EnemyIdleState idleState = new EnemyIdleState();
    public EnemyPatrolState patrolState = new EnemyPatrolState();
    public EnemyAttackingState attackingState = new EnemyAttackingState();
    public EnemyChaseState chaseState = new EnemyChaseState();
    public EnemyDeadState deadState = new EnemyDeadState();
    public EnemySenseState senseState = new EnemySenseState();

    // animator reference
    [HideInInspector]
    public Animator animator;

    [Header("Idle State Settings")]
    [Range(.5f, 10f)]
    [SerializeField] public float idleDuration = 2f; // center value for idle duration
    [Range(0f, 1f)]
    [SerializeField] public float idleDurationVariance = 0.2f; // variance percentage for idle duration


    [Header("Patrol State Settings")]
    [SerializeField] public float patrolRadius = 5f;
    [SerializeField] public float patrolSpeed = 2f;
    [HideInInspector] public Vector3 patrolPoint;


    [Header("Combat Settings")]
    public float detectionRange = 200f;
    public float attackRange = 2.5f;
    public float attackDamage = 1;

    [Header("Health")]
    [SerializeField] public float maxHealth = 10f;
    [HideInInspector] public float currentHealth;
    [SerializeField] public float deathShrinkSpeed = 2f;


    [HideInInspector]
    public NavMeshAgent agent;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        patrolPoint = transform.position;

        agent.speed = patrolSpeed;
        agent.stoppingDistance = attackRange;
        currentHealth = maxHealth;

        //Autoassign playerTrans if not set in Inspector
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
            else
            {
                Debug.LogWarning($"[{name}] No object with tag 'Player' found. Enemy will not chase.");
            }
        }

    }
    void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
    public void SwitchState(EnemyBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    // Animation Event Bridges
    public void OnAttackHit()
    {
        if (currentState is EnemyAttackingState attacking)
        {
            attacking.OnAttackHit(this);
        }
    }
    public void AttackAnimationEnd()
    {
        if (currentState is EnemyAttackingState attacking)
        {
            attacking.AttackAnimationEnd(this);
        }
    }

    public void TakeDamage(int amount)
    {
        if(currentHealth <= 0)
        {
            return; // Already dead
        }

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        Debug.Log($"Enemy took {amount} damage, current health: {currentHealth}");
    }

    public void Die()
    {
        if (currentState == deadState)
        {
            return; // Prevent multiple death triggers
        }


        SwitchState(deadState);
    }

}
