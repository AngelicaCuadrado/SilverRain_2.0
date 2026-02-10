using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int pauseCounter = 0;

    [Header("Events")]
    public static UnityEvent OnLevelStart;
    public static UnityEvent OnLevelWon;
    public static UnityEvent OnLevelLost;
    public static UnityEvent OnGamePaused;
    public static UnityEvent OnGameUnpaused;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Subscribe to events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #region Pause Management
    // public void PauseGame()
    // {
    //     // Increment pause counter
    //     pauseCounter++;
    //     // Pause game
    //     Time.timeScale = 0f;
    //     // Disable player inputs
    //     //playerInput.enabled = false;
    //
    //     //Make cursor unlocked and visible
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    //
    //     // Call event
    //     OnGamePaused?.Invoke();
    // }
    //
    // public void UnpauseGame()
    // {
    //     // Decrement pause counter
    //     pauseCounter = Mathf.Max(0, pauseCounter - 1);
    //     // Unpause game if all instances of pause are done
    //     if (pauseCounter == 0)
    //     {
    //         Time.timeScale = 1f;
    //         // Enable player inputs
    //         //playerInput.enabled = true;
    //
    //         //Make cursor locked and invisible
    //         Cursor.lockState = CursorLockMode.Locked;
    //         Cursor.visible = false;
    //
    //         // Call event
    //         OnGameUnpaused?.Invoke();
    //     }
    // }
    #endregion

    public void ChangeLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //--TODO--
        //Find out if the scene is a playable level or a menu
        //Call the appropriate "Start of Level" methods on managers
    }

    //------------------------------------Move this to score manager-------------------------------
    public void AddScore(int score) { }
}