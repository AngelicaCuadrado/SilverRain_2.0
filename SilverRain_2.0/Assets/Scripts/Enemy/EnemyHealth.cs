using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyHealth : MonoBehaviour
{
    public static event Action<EnemyHealth> OnEnemyKilled;

    [Header("Health")]
    [SerializeField, Tooltip("")]
    private int baseHealth = 100;
    [SerializeField, Tooltip("")]
    private int currentHealth;
    [SerializeField, Tooltip("")]
    private ParticleSystem bloodSplatterPrefab;
    [SerializeField, Tooltip("")]
    private string sfxID;

    [Header("References")]
    [SerializeField, Tooltip("")]
    private Enemy enemy;
    [SerializeField, Tooltip("")]
    private Rigidbody rb;
    [SerializeField, Tooltip("")]
    private Animator animator;
    [SerializeField, Tooltip("")]
    private EnemyController controller;
    [SerializeField, Tooltip("")]
    private PlayerExperience playerExp;
    [SerializeField, Tooltip("")]
    private NavMeshAgent agent;
    //private AudioSource audioSource;

    void Start()
    {
        float multiplier = (StageManager.Instance != null) ? StageManager.Instance.GetHealthMultiplier() : 1f;
        currentHealth = Mathf.RoundToInt(baseHealth * multiplier);

        if (PlayerFinder.Instance.Player == null) { Debug.Log("EnemyHealth couldn't find player"); return; }
        playerExp = PlayerFinder.Instance.Player.GetComponent<PlayerExperience>();

        //enemy = GetComponent<Enemy>();
        //animator = GetComponentInChildren<Animator>();
        //controller = GetComponent<EnemyController>();
        //audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        AudioManager.Instance.PlaySFX(sfxID);

        if (currentHealth <= 0)
        {
            Die();
        }

        Vector3 bloodSplatterSpawn = transform.position;
        bloodSplatterSpawn.y += 1f;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        var bloodSplatter = Instantiate(bloodSplatterPrefab, bloodSplatterSpawn, rotation);

        bloodSplatter.Play();

        if (!GlobalInvisibilityManager.Instance.IsActive)
        {
            enemy.RevealTimed(5f);
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
    }

    // This will be called by "Death" animation event
    // Ensure the event has exactly the same name as this method
    public void DieAnimFinished()
    {
        animator.SetBool("isDead", false);
        ReturnToPool();
    }

    IEnumerator DeathCoroutine()
    {
        animator.SetBool("isDead", true);
        if (controller != null) controller.enabled = false;

        playerExp.GainExp(enemy.XPValue);
        ScoreManager.Instance.AddScore(enemy.ScoreValue);

        yield return new WaitForSeconds(3);

        //ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (enemy.PoolKey != null)
        {
            EnemyManager.Instance.EnemyPool.ReturnToPool(gameObject, enemy.PoolKey);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
