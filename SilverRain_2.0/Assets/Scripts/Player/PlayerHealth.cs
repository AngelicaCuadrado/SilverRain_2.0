using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Tooltip("")]
    private float maxHealth;
    [SerializeField, Tooltip("")]
    private float currentHealth;

    [Header("Events")]
    [HideInInspector] public UnityEvent onPlayerHealthChanged;
    [HideInInspector] public UnityEvent onTakeDamage;
    [HideInInspector] public UnityEvent OnDie;
    [HideInInspector] public UnityEvent<bool> onLowHealthStateChanged;
    [HideInInspector] public UnityEvent<bool> onHighHealthStateChanged;

    [Header("Health State")]
    [SerializeField, Tooltip("Indicates if the player is in low health state")]
    private bool isLowHealth = false;
    [SerializeField, Tooltip("Indicates if the player is in high health state")]
    private bool isHighHealth = false;

    [Header("Invincibility Settings")]
    [SerializeField, Tooltip("Indicates if the player is currently invincible")]
    private bool isInvincible = false;
    [SerializeField, Tooltip("Duration of invincibility in seconds")]
    private float invincibilityTimer = 0f;
    [SerializeField, Tooltip("GameObject for the invincibility glow effect")]
    private GameObject invincibilityGlowEffect;

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

            // Toggle the glow effect based on invincibility state
            if (invincibilityGlowEffect != null) { invincibilityGlowEffect.SetActive(true); }
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
        // Disable the glow effect when invincibility ends
        else
        {
            if (invincibilityGlowEffect != null) { invincibilityGlowEffect.SetActive(false); }
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