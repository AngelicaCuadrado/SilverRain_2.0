using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseWindow : UIWindow
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private SettingsWindow settingsWindowPrefab;

    private object _pauseToken;
    private object _inputToken;

    public override void OnPushed()
    {
        _pauseToken = PauseManager.Instance.Acquire("PauseMenu");
        _inputToken = InputManager.Instance.Acquire(InputMode.UI,"PauseMenu");
        
        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(Quit);
    }
    
    public override void OnPopped()
    {
        resumeButton.onClick.RemoveListener(Resume);
        settingsButton.onClick.RemoveListener(OpenSettings);
        quitButton.onClick.RemoveListener(Quit);

        // Release tokens (safe even if other systems still hold tokens).
        if (_pauseToken != null)
        {
            PauseManager.Instance.Release(_pauseToken);
            _pauseToken = null;
        }

        if (_inputToken != null)
        {
            InputManager.Instance.Release(_inputToken);
            _inputToken = null;
        }
    }

    private void Resume()
    {
        UIManager.Instance.Pop();
    }

    private void OpenSettings()
    {
        UIManager.Instance.Push(settingsWindowPrefab);
    }

    private void Quit()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
