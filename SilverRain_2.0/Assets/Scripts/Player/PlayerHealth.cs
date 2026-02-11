using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth;
    public float currentHealth;

    [Header("Events")]
    private UnityEvent onPlayerHealthChanged;
    public UnityEvent onTakeDamage;
    public static event Action<bool> onDie;

    public bool isInvincible = false;

    private object _pauseToken;
    
    private void Start()
    {
        //maxHealth = 100f * FindAnyObjectByType<PlayerStats>().maxHealth;
        currentHealth = maxHealth;
        //audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        AudioManager.Instance.PlaySFX("sfx_player_hurt");

        onTakeDamage?.Invoke();
        onPlayerHealthChanged?.Invoke();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        onPlayerHealthChanged?.Invoke();
    }

    private IEnumerator RegenHealth()
    {
        yield return new WaitForSeconds(1f);
    }

    public void SetHealth(float amount)
    {
        if (amount <= 0f)
        {
            currentHealth = 0f;
            Die();
            return;
        }
        if (amount > maxHealth)
        {
            currentHealth = maxHealth;
            return;
        }
        currentHealth = Mathf.Clamp(amount, 0f, maxHealth);
    }

    private void Die()
    {
        // Death logic here
        onDie?.Invoke(false);
        //GameManager.Instance.PauseGame();
        
        // FOR TESTING HERE ONLY
        _pauseToken = PauseManager.Instance.Acquire("Die");
        Debug.Log("Player Died");
        
        // when player die, push GameOverWindow, when UIWindow is pushed,
        // acquired pause token, when leave this window, release the token.
        
        //GameManager.Instance.ChangeLevel("LevelSelector");
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}