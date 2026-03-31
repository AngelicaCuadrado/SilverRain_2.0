using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeavyMetal : Modification, IWeaponModifier
{
    [SerializeField, Tooltip("Additional damage multiplier applied to the hammer (additive)")]
    private float damageModifier = 0.5f;
    [SerializeField, Tooltip("Additional cooldown multiplier applied to the hammer (additive)")]
    private float cooldownModifier = 0.5f;
    [SerializeField, Tooltip("Force applied to enemies away from the player on hit")]
    private float pushBackForce = 50f;
    [SerializeField, Tooltip("How long to disable the NavMeshAgent so physics impulse can move the enemy")]
    private float agentDisableDuration = 0.35f;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(PushbackEnemies);
    }

    public float GetModifyValue(WeaponType weapon, StatType stat)
    {
        return stat switch
        {
            StatType.AttackDamage => damageModifier,
            StatType.Cooldown => cooldownModifier,
            _ => 0,
        };
    }

    // Get the rigidbody from all enemies hit and push them away from the player
    private void PushbackEnemies(WeaponType type, GameObject[] hits, Vector3 pos, Projectile proj)
    {
        if (type != WeaponType.Hammer) return;
        if (hits == null || hits.Length == 0) return;

        var player = PlayerFinder.Instance.Player;
        if (player == null)
        {
            Debug.LogWarning("HeavyMetal: Player not found for pushback calculation.");
            return;
        }
        var playerPos = player.transform.position;

        // Avoid pushing the same rigidbody multiple times if multiple colliders hit the same enemy
        var processed = new HashSet<Rigidbody>();

        foreach (var go in hits)
        {
            if (go == null) continue;

            // Prefer Rigidbody on the hit object or one of its parents
            var rb = go.GetComponent<Rigidbody>() ?? go.GetComponentInParent<Rigidbody>();
            if (rb == null) continue;
            if (processed.Contains(rb)) continue;

            // Don't affect kinematic rigidbodies
            if (rb.isKinematic) continue;

            // Direction away from the player
            Vector3 dir = (rb.position - playerPos);
            if (dir.sqrMagnitude <= 0.0001f) dir = (rb.position - pos); // fallback to hit position if identical
            dir = dir.normalized;

            // Try to find a NavMeshAgent on the same object or parent
            var agent = rb.GetComponent<NavMeshAgent>() ?? rb.GetComponentInParent<NavMeshAgent>();

            // Apply knockback: if agent exists, temporarily disable it so physics can move the transform
            if (agent != null && agent.isOnNavMesh)
            {
                // Start coroutine to handle disabling/re-enabling the agent and apply physics impulse
                StartCoroutine(ApplyKnockbackWithAgent(rb, agent, dir * pushBackForce, agentDisableDuration));
            }
            else
            {
                // No agent: just apply impulse
                rb.AddForce(dir * pushBackForce, ForceMode.Impulse);
            }

            processed.Add(rb);
        }
    }

    private IEnumerator ApplyKnockbackWithAgent(Rigidbody rb, NavMeshAgent agent, Vector3 force, float disableDuration)
    {
        if (rb == null)
            yield break;

        // Disable agent so the physics engine controls movement
        bool wasEnabled = agent.enabled;
        if (wasEnabled)
        {
            agent.enabled = false;
        }

        // Clear velocity for consistent impulse, then apply
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(disableDuration);

        // Re-enable agent and warp to current Rigidbody position to avoid snapping
        if (rb != null && agent != null)
        {
            agent.Warp(rb.position);
            agent.enabled = true;
            agent.ResetPath();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponHit.RemoveListener(PushbackEnemies);
    }
}