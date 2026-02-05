using UnityEngine;
using UnityEngine.AI;

public class EnemyDeadState : EnemyBaseState
{
    private bool initialized;
    private Vector3 initialScale;


    //needs a timer  because is not a monobehaviour script 
    private float deathWaitTimer = 0f;
    private const float DEATH_WAIT_DURATION = 1f;

    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Entering Dead State");
        initialized = false;

        // Stop NavMesh movement if present
        var agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        if (enemy.animator != null)
        {


            bool alreadyDead = enemy.animator.GetBool("isDead");

            
            if (!alreadyDead)
            {
                Debug.Log("Playing death animation");
                enemy.animator.SetBool("isDead", true);
                enemy.animator.SetFloat("speed", 0f);
            }

        }





        // Disable collider so it stops getting hit / blocking
        var col = enemy.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Cache scale so we can shrink smoothly
        initialScale = enemy.transform.localScale;
        initialized = true;



    }
    public override void UpdateState(EnemyStateManager enemy)
    {
        
        if (!initialized) return;

        if (deathWaitTimer < DEATH_WAIT_DURATION)
        {
            deathWaitTimer += Time.deltaTime;
            return; // just wait
        }



        // Shrink towards zero over time
        float shrinkSpeed = enemy.deathShrinkSpeed;

        enemy.transform.localScale = Vector3.Lerp(
            enemy.transform.localScale,
            Vector3.zero,
            shrinkSpeed * Time.deltaTime
        );

        // When small enough, destroy the GameObject
        if (enemy.transform.localScale.magnitude <= 0.05f)
        {
            Object.Destroy(enemy.gameObject);
        }
    }
    public override void ExitState(EnemyStateManager enemy)
    {
        
    }

}
