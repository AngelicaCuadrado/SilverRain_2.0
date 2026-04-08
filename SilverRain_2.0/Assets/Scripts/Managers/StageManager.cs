using System;
using UnityEngine;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Stage Settings")]
    [SerializeField] private int maxStage = 5;
    [SerializeField] private float stageDuration = 60f;

    [SerializeField] private Color[] stageEmissionColors =
    {
        Color.black,
        new Color(0.3f, 0.7f, 1.0f),    // blue
        new Color(0.7f, 0.3f, 1.0f),    // purple
        new Color(1.0f, 0.5f, 0.1f),    // orange
        new Color(1.0f, 0.0f, 0.0f),    // red
    };
    
    [Header("Enemy Settings")]
    [SerializeField] private float[] healthMultipliers = { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f};
    
    [HideInInspector] public UnityEvent<int> OnStageChanged;
    [HideInInspector] public UnityEvent<float> OnTimerUpdated;
    
    [SerializeField] private float _elapsedTime = 0f;
    [SerializeField] private int _currentStage = 0;
    private bool _isRunning = false;
    
    public int CurrentStage => _currentStage;
    public float ElapsedTime => _elapsedTime;

    public float GetHealthMultiplier()
    {
        int index = Mathf.Clamp(_currentStage - 1, 0, healthMultipliers.Length - 1);
        return healthMultipliers[index];
    }

    public Color GetStageEmissionColor()
    {
        int index = Mathf.Clamp(_currentStage - 1, 0, stageEmissionColors.Length - 1);
        return stageEmissionColors[index];
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
        
        OnStageChanged ??= new UnityEvent<int>();
        OnTimerUpdated ??= new UnityEvent<float>();
    }

    private void OnEnable()
    {
        GameManager.OnLevelStart?.AddListener(ResetStage);
        ResetStage();
    }

    private void OnDisable()
    {
        GameManager.OnLevelStart?.RemoveListener(ResetStage);
    }

    private void Update()
    {
        if (!_isRunning) { return; }
        
        _elapsedTime += Time.deltaTime;
        OnTimerUpdated?.Invoke(_elapsedTime);
        
        int newStage = Mathf.Clamp(
            Mathf.FloorToInt(_elapsedTime / stageDuration ) + 1, 
            1, 
            maxStage
        );

        if (newStage != _currentStage)
        {
            _currentStage = newStage;
            OnStageChanged?.Invoke(_currentStage);
        }
    }

    private void ResetStage()
    {
        _elapsedTime = 0f;
        _currentStage = 1;
        _isRunning = true;
        OnStageChanged?.Invoke(_currentStage);
        OnTimerUpdated?.Invoke(0f);
    }
}