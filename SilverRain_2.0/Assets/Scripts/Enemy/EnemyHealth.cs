using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyHealth : MonoBehaviour
{
    [FormerlySerializedAs("maxHealth")]
    [Header("Health")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private ParticleSystem bloodSplatterPrefab;
    [SerializeField] private string sfxID;

    [Header("Components")]
    public Animator animator;
    private Enemy enemy;
    private EnemyController controller;
    private PlayerExperience playerExp;
    //private AudioSource audioSource;

    void Start()
    {
        float multiplier = (StageManager.Instance != null) ? StageManager.Instance.GetHealthMultiplier() : 1f;
        currentHealth = Mathf.RoundToInt(baseHealth * multiplier);
        
        enemy = GetComponent<Enemy>();
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<EnemyController>();
        playerExp = PlayerFinder.Instance.Player.GetComponent<PlayerExperience>();
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
        
        playerExp.GainExp(enemy.XPValue);
        ScoreManager.Instance.AddScore(enemy.ScoreValue);
        
        yield return new WaitForSeconds(3);
        
        //Destroy(gameObject);
        ReturnToPool();
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