using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField, Tooltip("")]
    private MainMenuWindow mainMenuWindowPrefab;
    [SerializeField, Tooltip("")]
    private HUDWindow hudWindowPrefab;

    [Header("Level Time")]
    [SerializeField, Tooltip("")]
    private float levelTimer;
    [SerializeField, Tooltip("")]
    private float levelDuration;

    [Header("Events")]
    [HideInInspector] public static UnityEvent OnLevelStart;
    [HideInInspector] public static UnityEvent OnLevelWon;
    [HideInInspector] public static UnityEvent OnLevelLost;
    [HideInInspector] public static UnityEvent OnGamePaused;
    [HideInInspector] public static UnityEvent OnGameUnpaused;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize state events
        OnLevelStart ??= new UnityEvent();
        OnLevelWon ??= new UnityEvent();
        OnLevelLost ??= new UnityEvent();
        OnGamePaused ??= new UnityEvent();
        OnGameUnpaused ??= new UnityEvent();

        //Subscribe to events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    public void ChangeLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] OnSceneLoaded: {scene.name}, IsPlayable: {IsPlayableLevel(scene)}");
        // Clean up all UI from previous scene
        UIManager.Instance.Clear();
        UIManager.Instance.ClearAllOverlay();

        if (IsPlayableLevel(scene))
        {
            InputManager.Instance.Apply(InputMode.Gameplay);
            UIManager.Instance.ShowOverlay("HUD", hudWindowPrefab);
            OnLevelStart?.Invoke();
        }
        else
        {
            InputManager.Instance.Apply(InputMode.UI);
            UIManager.Instance.Push(mainMenuWindowPrefab);
        }
    }

    private bool IsPlayableLevel(Scene scene)
    {
        return scene.name != "MainMenu";
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}