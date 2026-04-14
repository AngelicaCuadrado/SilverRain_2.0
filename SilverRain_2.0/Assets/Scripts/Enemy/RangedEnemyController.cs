using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
//using static UnityEditor.PlayerSettings;
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
        LayerMask playerLayer = LayerMask.GetMask("Player");
        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange, playerLayer))
        {
            // Only attack if the ray hits the player
            if (hit.collider.CompareTag("Player"))
            {
                //animator.SetBool("isAttacking", true);
                Attack();
                return;
            }
        }

        // If raycast didn't hit the player, stop attacking
        attackTimer = 0f;
        //animator.SetBool("isAttacking", false);
    }

    public override void Attack()
    {
        if (targetPlayer == null) return;

        attackTimer += Time.deltaTime;
        //Spawn projectile targetting playerTrans.
        if (attackTimer >= timeBetweenAttacks)
        {
            // Trigger the attack animation
            animator.SetTrigger("attacking");

            attackTimer = 0;
        }
    }

    public void FireProjectile()
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
    }

    void OnDrawGizmos()
    {
        // Draw the detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Only draw the ray if we have a target to look at
        if (targetPlayer != null)
        {
            Vector3 origin = transform.position;
            Vector3 direction = (targetPlayer.transform.position - origin).normalized;

            // Perform a "Preview" raycast for the gizmo
            if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange))
            {
                // Green if it hits the player, Yellow if it hits an obstacle
                Gizmos.color = hit.collider.CompareTag("Player") ? Color.green : Color.yellow;
                Gizmos.DrawLine(origin, hit.point);
                Gizmos.DrawWireSphere(hit.point, 0.1f);
            }
            else
            {
                // Red if the player is out of range or ray hits nothing
                Gizmos.color = Color.red;
                Gizmos.DrawRay(origin, direction * attackRange);
            }
        }
    }
}