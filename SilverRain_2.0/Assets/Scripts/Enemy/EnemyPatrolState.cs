using Unity.VisualScripting;
using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private Vector3 currentTarget;
    private bool hasTarget = false;
    private float elapsedTime = 0f;
    private float patrolEndTime = 2f; // Time to patrol before picking a new target

    private const float REACH_THRESHOLD = 0.2f;
    public override void EnterState(EnemyStateManager enemy)
    {
       
        enemy.animator.SetFloat("speed", 1.1f); 
        hasTarget = false;
        elapsedTime = 0f;

    }
    public override void UpdateState(EnemyStateManager enemy)
    {

        if (enemy.player != null)
        {
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);
            if (distanceToPlayer <= enemy.detectionRange)
            {
                enemy.SwitchState(enemy.chaseState);
                return;
            }

            
        }

        // Patrol timer stuff 
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= patrolEndTime)
        {

            elapsedTime = 0f;
            enemy.SwitchState(enemy.idleState);
            return;
        }

        if (!hasTarget || ReachedTarget(enemy.transform.position))
        {
            PickNewRandomTarget(enemy);
        }
        MoveTowardsTarget(enemy);



    }

    public override void ExitState(EnemyStateManager enemy)
    {
        
        
    }

    private void PickNewRandomTarget(EnemyStateManager enemy)
    {
        Vector2 randomCircle = Random.insideUnitCircle * enemy.patrolRadius;
        currentTarget = enemy.patrolPoint + new Vector3(randomCircle.x, 0, randomCircle.y);
        hasTarget = true;
    }

    private void MoveTowardsTarget(EnemyStateManager enemy)
    {
        Transform t = enemy.transform;

        // Move
        t.position = Vector3.MoveTowards(
            t.position,
            currentTarget,
            enemy.patrolSpeed * Time.deltaTime
        );

        // Rotate smoothly to face target
        Vector3 dir = currentTarget - t.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            t.rotation = Quaternion.Slerp(t.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    public bool ReachedTarget(Vector3 currentPosition) { 
        return Vector3.Distance(currentPosition, currentTarget) <= REACH_THRESHOLD;
    }

}
