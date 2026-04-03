using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyHealth : MonoBehaviour
{
    public static event Action<EnemyHealth> OnEnemyKilled;

    [Header("Health")]
    [SerializeField, Tooltip("The base health of the enemy")]
    private int baseHealth = 100;
    [SerializeField, Tooltip("The current health of the enemy")]
    private int currentHealth;
    [SerializeField, Tooltip("The key used to identify the blood VFX in the object pool")]
    private string bloodVFXPoolKey;
    [SerializeField, Tooltip("The sound effect ID for when the enemy takes damage")]
    private string sfxID;

    [Header("Invisibility")]
    [SerializeField,Tooltip("The duration which the enemy reveals nearby enemies when taking damage")]
    private float revealDuration = 5f;
    [SerializeField,Tooltip("The radius within which the enemy reveals nearby enemies when taking damage")]
    private float revealRadius = 5f;

    [Header("References")]
    [SerializeField, Tooltip("The enemy component attached to this GameObject")]
    private Enemy enemy;
    [SerializeField, Tooltip("The Rigidbody component attached to this GameObject")]
    private Rigidbody rb;
    [SerializeField, Tooltip("The Animator component attached to this GameObject")]
    private Animator animator;
    [SerializeField, Tooltip("The EnemyController component attached to this GameObject")]
    private EnemyController controller;
    [SerializeField, Tooltip("The PlayerExperience component attached to the player")]
    private PlayerExperience playerExp;
    [SerializeField, Tooltip("The NavMeshAgent component attached to this GameObject")]
    private NavMeshAgent agent;

    void Start()
    {
        float multiplier = (StageManager.Instance != null) ? StageManager.Instance.GetHealthMultiplier() : 1f;
        currentHealth = Mathf.RoundToInt(baseHealth * multiplier);

        if (PlayerFinder.Instance.Player == null) { Debug.Log("EnemyHealth couldn't find player"); return; }
        playerExp = PlayerFinder.Instance.Player.GetComponent<PlayerExperience>();
    }

    public void TakeDamage(int damage)
    {
        // Take damage
        currentHealth -= damage;

        // Play sound effect
        AudioManager.Instance.PlaySFX(sfxID);

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }

        // Spawn blood VFX
        Vector3 spawnPos = transform.position + Vector3.up * 1f;
        Quaternion rot = Quaternion.identity;
        GlobalInvisibilityManager.Instance.BloodSplatterPool.Spawn(bloodVFXPoolKey, spawnPos, rot);

        if (!GlobalInvisibilityManager.Instance.IsActive)
        {
            // Reveal self
            enemy.RevealTimed(revealDuration);

            // Reveal nearby enemies
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");
            Collider[] colliders = Physics.OverlapSphere(transform.position, revealRadius, enemyLayer);
            foreach (var col in colliders)
            {
                Enemy nearbyEnemy = col.GetComponent<Enemy>();
                if (nearbyEnemy != null && nearbyEnemy != enemy)
                {
                    nearbyEnemy.RevealTimed(revealDuration);
                }
            }
        }

        animator.SetTrigger("hurt");
    }

    public void ResetHealth()
    {
        float multiplier = (StageManager.Instance != null) ? StageManager.Instance.GetHealthMultiplier() : 1f;
        currentHealth = Mathf.RoundToInt(baseHealth * multiplier);

        // Enable collision
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
        // Enable physics
        rb.isKinematic = false;
        rb.useGravity = true;
        // Enable components
        if (agent != null) agent.enabled = true;
        if (controller != null) controller.enabled = true;
    }

    private void Die()
    {
        // Disable collision
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        // Disable physics
        rb.isKinematic = true;
        rb.useGravity = false;
        // Disable components
        if (agent != null) agent.enabled = false;
        if (controller != null) controller.enabled = false;

        // Give player rewards
        playerExp.GainExp(enemy.XPValue);
        ScoreManager.Instance.AddScore(enemy.ScoreValue);

        // Start animation
        animator.SetBool("isDead", true);

        OnEnemyKilled?.Invoke(this);
    }

    // This will be called by "Death" animation event
    // Ensure the event has exactly the same name as this method
    public void DieAnimFinished()
    {
        animator.SetBool("isDead", false);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (enemy.PoolKey != null)
        {
            enemy.PoolOwner.ReturnToPool(gameObject, enemy.PoolKey);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
