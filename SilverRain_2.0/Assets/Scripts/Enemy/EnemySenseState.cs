using UnityEngine;

public class EnemySenseState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Entering Sense State");
    }
    public override void UpdateState(EnemyStateManager enemy)
    {
        Debug.Log("Updating Sense State");
    }
    public override void ExitState(EnemyStateManager enemy)
    {
        Debug.Log("Exiting Sense State");
    }

}
