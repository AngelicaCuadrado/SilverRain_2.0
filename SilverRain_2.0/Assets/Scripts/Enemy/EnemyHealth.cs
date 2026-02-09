using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private ParticleSystem bloodSplatterPrefab;
    [SerializeField] private string sfxID;

    [Header("Components")]
    public Animator animator;
    private Enemy enemy;
    private EnemyController controller;
    private PlayerExperience player;
    //private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        enemy = GetComponent<Enemy>();
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<EnemyController>();
        player = FindFirstObjectByType<PlayerExperience>();
        //audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void TakeDamage(int damage)
    {
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

        if (!GlobalInvisibilityManager.Instance.isActive)
        {
            enemy.RevealTimed(5f);
        }

        animator.SetTrigger("hurt");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
    }
    
    private void Die()
    {
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