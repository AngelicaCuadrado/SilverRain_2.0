using UnityEngine;

public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyStateManager enemy);
    
    public abstract void UpdateState(EnemyStateManager enemy);

    public abstract void ExitState(EnemyStateManager enemy);
    public virtual void OnColliderEnter(EnemyStateManager enemy, Collider other)
    {
        Debug.Log("Base OnColliderEnter called with object: " + other.name);

    }
}

