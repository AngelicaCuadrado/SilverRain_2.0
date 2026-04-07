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
    public bool hasSurvivalInstinct = false;

    [Header("Events")]
    [HideInInspector] public UnityEvent onPlayerHealthChanged;
    [HideInInspector] public UnityEvent onTakeDamage;
    [HideInInspector] public UnityEvent OnDie;
    [HideInInspector] public UnityEvent<bool> onLowHealthStateChanged;
    [HideInInspector] public UnityEvent<bool> onHighHealthStateChanged;

    private bool isLowHealth = false;
    private bool isHighHealth = false;

    public bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private object _pauseToken;
    
    private void Awake()
    {
        //maxHealth = 100f * FindAnyObjectByType<PlayerStats>().maxHealth;
        currentHealth = maxHealth;
        //audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    public void ActivateInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityTimer = duration;
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        AudioManager.Instance.PlaySFX("sfx_player_hurt");

        onTakeDamage?.Invoke();
        onPlayerHealthChanged?.Invoke();
        CheckLowHealthState();
        CheckHighHealthState();

        if (currentHealth <= 0f)
        {
            if (hasSurvivalInstinct)
            {
                hasSurvivalInstinct = false; 

                currentHealth = maxHealth * 0.3f;

                onPlayerHealthChanged?.Invoke();
                CheckLowHealthState();
                CheckHighHealthState();

                return;
            }
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        onPlayerHealthChanged?.Invoke();
        CheckHighHealthState();
        CheckLowHealthState();
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
        OnDie?.Invoke();
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

    private void CheckLowHealthState()
    {
        bool newLowHealthState = GetHealthPercentage() <= 0.3f;

        if (newLowHealthState == isLowHealth) return;

        isLowHealth = newLowHealthState;
        onLowHealthStateChanged?.Invoke(isLowHealth);
    }

    private void CheckHighHealthState()
    {
        bool newHighHealthState = GetHealthPercentage() >= 0.8f;

        if (newHighHealthState == isHighHealth) return;

        isHighHealth = newHighHealthState;
        onHighHealthStateChanged?.Invoke(isHighHealth);
    }
}