using System;
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
    
    [Header("Stage")]
    [SerializeField] private Slider stageBar;
    [SerializeField] private TMP_Text stageText;
    
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

        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnTimerUpdated.AddListener(RefreshTimer);
            StageManager.Instance.OnStageChanged.AddListener(RefreshStage);
            RefreshTimer(StageManager.Instance.ElapsedTime);
            RefreshStage(StageManager.Instance.CurrentStage);
        }
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged.AddListener(RefreshScore);
            RefreshScore();
        }
    }

    public override void OnPopped()
    {
        if (_playerHealth != null) _playerHealth.onPlayerHealthChanged.RemoveListener(RefreshHealth);
        if (_playerExperience != null) _playerExperience.OnExpChanged.RemoveListener(RefreshExp);
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnTimerUpdated.RemoveListener(RefreshTimer);
            StageManager.Instance.OnStageChanged.RemoveListener(RefreshStage);
        }
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged.RemoveListener(RefreshScore);
    }

    private void Update()
    {
        if (StageManager.Instance != null)
        {
            TimeSpan elapsedTime = TimeSpan.FromSeconds(StageManager.Instance.ElapsedTime);
            timerText.text = elapsedTime.ToString(@"mm\:ss");
        }
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

    private void RefreshTimer(float elapsed)
    {
        if (stageBar == null) return;
        int stage = StageManager.Instance.CurrentStage;
        if (stage != 5)
        {
            float stagePercentage = (elapsed % 60f) / 60f;
            stageBar.value = stagePercentage;
        }
        else
        {
            stageBar.value = 1f;
        }
        
    }

    private void RefreshStage(int stage)
    {
        if (stageText != null) 
        {
            stageText.text = stage==5 ? "Stage. Max" : $"Stage. {stage}";
        }
    }
    
    private void RefreshScore()
    {
        int score = ScoreManager.Instance.GetScore();
        if (scoreText != null) 
            scoreText.text = $"Score: {score}";
    }
}
