using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyController : EnemyController
{
     public override void Update()
    {
        if (tutorialFrozen) return;

        base.Update();
    }

    public override void CheckPlayerInRange()
    {
        if (targetPlayer == null) return;
        if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= attackRange)
        {
            if (playerHealth != null)
            {
                animator.SetBool("isAttacking", true);
                Attack();
            }
        }
        else
        {
            attackTimer = 0f;
            animator.SetBool("isAttacking", false);
        }
    }

    public override void Attack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= timeBetweenAttacks)
        {
            playerHealth.TakeDamage(enemy.Damage);
            attackTimer = 0;
        }
    }
}
