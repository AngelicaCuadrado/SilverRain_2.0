using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class RangedEnemyController : EnemyController
{
    [Header("Projectile Settings")]
    [SerializeField, Tooltip("")]
    private Transform firePoint;
    [SerializeField, Tooltip("")]
    private GameObject projectilePrefab;

    [Header("Pooling")]
    [SerializeField, Tooltip("")]
    private string projectilePoolKey = "EnemyProjectilePool";

    public override void CheckPlayerInRange()
    {
        if (targetPlayer == null) return;

        // Raycast to the player
        Vector3 origin = transform.position;
        Vector3 direction = (targetPlayer.transform.position - origin).normalized;

        // Raycast toward the player
        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange))
        {
            // Only attack if the ray hits the player
            if (hit.collider.CompareTag("Player"))
            {
                animator.SetBool("isAttacking", true);
                Attack();
                return;
            }
        }

        // If raycast didn't hit the player, stop attacking
        attackTimer = 0f;
        animator.SetBool("isAttacking", false);
    }

    public override void Attack()
    {
        if (targetPlayer == null) return;

        attackTimer += Time.deltaTime;
        //Spawn projectile targetting playerTrans.
        if (attackTimer >= timeBetweenAttacks)
        {
            Vector3 dir = (targetPlayer.transform.position - firePoint.position).normalized;

            ObjectPooler pool = EnemyManager.Instance.EnemyProjectilePool;
            if (pool == null) return;

            GameObject go = pool.Spawn(projectilePoolKey, firePoint.position, Quaternion.LookRotation(dir));
            if (go == null) return;

            var projectile = go.GetComponent<EnemyProjectile>();
            if (projectile == null) return;

            projectile.Initialize(dir, enemy.Damage, playerHealth);
            projectile.PoolKey = projectilePoolKey;

            attackTimer = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}