using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        enemy.animator.SetFloat("speed", enemy.patrolSpeed * 1.5f);

        if (enemy.agent != null)
        {
            enemy.agent.isStopped = false;
            enemy.agent.speed = enemy.patrolSpeed * 1.5f;
            enemy.agent.stoppingDistance = enemy.attackRange;
        }

    }
    public override void UpdateState(EnemyStateManager enemy)
    {

        if (enemy.player == null || enemy.agent == null)
        {
            if (enemy.agent != null)
                enemy.agent.ResetPath();

            enemy.SwitchState(enemy.idleState);
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            enemy.transform.position,
            enemy.player.position
        );

        // Player too far back to patrol
        if (distanceToPlayer > enemy.detectionRange)
        {
            enemy.agent.ResetPath();
            enemy.SwitchState(enemy.patrolState);
            return;
        }

        // Close enough  attack
        if (distanceToPlayer <= enemy.attackRange)
        {
            enemy.agent.ResetPath();
            enemy.SwitchState(enemy.attackingState);
            return;
        }

        // Otherwise, chase via NavMesh
        enemy.agent.SetDestination(enemy.player.position);
       
    }
    public override void ExitState(EnemyStateManager enemy)
    {

        // Reset chase animation
        //enemy.GetComponent<Animator>().SetBool("isChasing", false);

        if (enemy.agent != null)
            enemy.agent.ResetPath();

    }

}
