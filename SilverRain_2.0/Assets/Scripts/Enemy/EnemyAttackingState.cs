using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    private bool hasDealtDamage = false; 
    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Entered Attacking State");
        hasDealtDamage = false;

        enemy.animator.SetFloat("speed", 0f);

        enemy.animator.SetBool("isAttacking", true);

    }
    public override void UpdateState(EnemyStateManager enemy)
    {
        
    }
    public override void ExitState(EnemyStateManager enemy)
    {
        
        enemy.animator.SetBool("isAttacking", false);
    }

    public void OnAttackHit(EnemyStateManager enemy)
    {
        if (hasDealtDamage) return;
        Debug.Log("Attack Hit");
    }

    public void AttackAnimationEnd(EnemyStateManager enemy)
    {
       if (enemy.chaseState != null)
       {
           enemy.SwitchState(enemy.chaseState);
       }
       else
       {
           enemy.SwitchState(enemy.idleState);
       }


    }

}
