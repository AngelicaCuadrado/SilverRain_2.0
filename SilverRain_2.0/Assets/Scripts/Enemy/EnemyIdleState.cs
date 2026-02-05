using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float idleDuration;
    private float elapsedTime;
    public override void EnterState(EnemyStateManager enemy)
    {
        
        enemy.animator.SetFloat("speed", 0f);

        elapsedTime = 0f;

        float variation = enemy.idleDurationVariance * enemy.idleDuration;
        float min = enemy.idleDuration - variation;
        float max = enemy.idleDuration + variation;
        
        idleDuration = Random.Range(min, max);
        
    }
    public override void UpdateState(EnemyStateManager enemy)
    {

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= idleDuration)
        {
            enemy.SwitchState(enemy.patrolState);
            

        }

    }
    public override void ExitState(EnemyStateManager enemy)
    {
        
        //enemy.animator.SetFloat("speed", 1f);
        Debug.Log("Exiting Idle State");
    }

}
