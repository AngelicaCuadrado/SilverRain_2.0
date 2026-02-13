using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Events")]
    public static UnityEvent OnLevelStart;
    public static UnityEvent OnLevelWon;
    public static UnityEvent OnLevelLost;
    public static UnityEvent OnGamePaused;
    public static UnityEvent OnGameUnpaused;
    
    [Header("UI")]
    [SerializeField] private MainMenuWindow mainMenuWindowPrefab;
    [SerializeField] private HUDWindow hudWindowPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

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

    //------------------------------------Move this to score manager-------------------------------
    public void AddScore(int score) { }
}