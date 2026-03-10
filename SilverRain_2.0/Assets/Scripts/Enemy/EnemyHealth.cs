using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyHealth : MonoBehaviour
{
    public static event Action<EnemyHealth> OnEnemyKilled;

    [Header("Health")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private ParticleSystem bloodSplatterPrefab;
    [SerializeField] private string sfxID;

    [Header("Components")]
    public Animator animator;
    private Enemy enemy;
    private EnemyController controller;
    private PlayerExperience player;
    private bool isDead;
    //private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        enemy = GetComponent<Enemy>();
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<EnemyController>();
        player = FindFirstObjectByType<PlayerExperience>();
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
        currentHealth = maxHealth;
        isDead = false;
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnEnemyKilled?.Invoke(this);

        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        
        // disable NaveMeshAgent, not Destroy
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;
        
        StartCoroutine(DeathCoroutine());
    }

    // replace Destroy with ReturnToPool
    IEnumerator DeathCoroutine()
    {
        animator.SetBool("isDead", true);
        //Destroy(controller);
        if (controller != null) controller.enabled = false;
        
        player.GainExp(enemy.RewardXP());
        GameManager.Instance.AddScore(enemy.RewardScore());
        
        yield return new WaitForSeconds(3);
        
        //Destroy(gameObject);
        enemy.ReturnToPool();
    }
}
