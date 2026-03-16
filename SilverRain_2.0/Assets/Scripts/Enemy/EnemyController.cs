using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public GameObject targetPlayer;
    public NavMeshAgent agent;
    public Animator animator;
    public Enemy enemy;
    protected bool tutorialFrozen;

    public abstract void Move();
    public abstract void Attack(PlayerHealth player);

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
