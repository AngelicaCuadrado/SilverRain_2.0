using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : UIWindow
{
    [Header("Buttons")] 
    [SerializeField] private Button startButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Prefabs")] 
    [SerializeField] private TutorialWindow tutorialWindowPrefab;
    [SerializeField] private SettingsWindow settingsWindowPrefab;
    [SerializeField] private CreditsWindow creditsWindowPrefab;
    [SerializeField] private LevelSelectWindow levelSelectWindowPrefab;
    //[SerializeField] private UpgradeWindow upgradeWindowPrefab;

    public override void OnPushed()
    {
        startButton.onClick.AddListener(OnStartClicked);
        tutorialButton.onClick.AddListener(OpenTutorial);
        settingsButton.onClick.AddListener(OpenSettings);
        creditsButton.onClick.AddListener(OpenCredits);
        quitButton.onClick.AddListener(Quit);
        
        AudioManager.Instance.PlayBGM("bgm_main");
    }

    public override void OnPopped()
    {
        startButton.onClick.RemoveListener(OnStartClicked);
        tutorialButton.onClick.RemoveListener(OpenTutorial);
        settingsButton.onClick.RemoveListener(OpenSettings);
        creditsButton.onClick.RemoveListener(OpenCredits);
        quitButton.onClick.RemoveListener(Quit);
        
        //AudioManager.Instance.StopBGM();
    }

    private void OnEnable()
    {
        AudioManager.Instance.PlayBGM("bgm_main");
    }

    /// <summary>
    /// Loads gameplay scene asynchronously.
    /// SceneManager is responsible for clearing UI and showing loading screen.
    /// </summary>
    private void OnStartClicked()
    {
        UIManager.Instance.Push(levelSelectWindowPrefab);
    }

    /// <summary>
    /// Opens the Tutorial window.
    /// 
    /// Note:
    /// - In main menu we usually do NOT pause the game (no gameplay running),
    ///   so using a SettingsWindow prefab with pauseGameOnOpen=false is simplest.
    /// </summary>

    private void OpenTutorial()
    {
        UIManager.Instance.Push(tutorialWindowPrefab);
    }

    /// <summary>
    /// Opens the Settings window.
    /// </summary>
    private void OpenSettings()
    {
        UIManager.Instance.Push(settingsWindowPrefab);
    }

    /// <summary>
    /// Opens the Credits window.
    /// </summary>

    private void OpenCredits()
    {
        UIManager.Instance.Push(creditsWindowPrefab);
    }

    /// <summary>
    /// Quits the application (standalone build) or stops Play Mode (Unity Editor).
    /// </summary>
    private void Quit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
