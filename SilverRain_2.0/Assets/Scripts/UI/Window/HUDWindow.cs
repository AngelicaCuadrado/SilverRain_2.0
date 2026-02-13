using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDWindow : UIWindow
{
    [Header("Health")]
    [SerializeField] private Slider healthBar;
    
    [Header("Experience")]
    [SerializeField] private Slider expBar;
    [SerializeField] private TMP_Text levelText;
    
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;
    
    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;
    
    // Cached references
    private PlayerHealth _playerHealth;
    private PlayerExperience _playerExperience;

    public override void OnPushed()
    {
        // Find Player components
        GameObject player = PlayerFinder.Instance.Player;
        _playerHealth = player.GetComponent<PlayerHealth>();
        _playerExperience = player.GetComponent<PlayerExperience>();
        
        // Subscribe to events
        if (_playerHealth != null)
        {
            RefreshHealth();
            _playerHealth.onPlayerHealthChanged.AddListener(RefreshHealth);
        }

        if (_playerExperience != null)
        {
            _playerExperience.OnExpChanged.AddListener(RefreshExp);
            RefreshExp();
        }
    }

    public override void OnPopped()
    {
        if (_playerHealth != null) _playerHealth.onPlayerHealthChanged.RemoveListener(RefreshHealth);
        if (_playerExperience != null) _playerExperience.OnExpChanged.RemoveListener(RefreshExp);
    }

    private void RefreshHealth()
    {
        if (healthBar != null) healthBar.value = _playerHealth.GetHealthPercentage();
    }

    private void RefreshExp()
    {
        if (expBar != null) expBar.value = _playerExperience.GetExpPersentage();
        if (levelText != null) levelText.text = $"Lv. {_playerExperience.CurrentLevel}";
    }
}
