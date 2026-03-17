using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : UIWindow
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Button backButton;
    
    private object _pauseToken;
    private object _inputToken;
    
    public override void OnPushed()
    {
        _pauseToken = PauseManager.Instance.Acquire("WinWindow");
        _inputToken = InputManager.Instance.Acquire(InputMode.UI,"WinWindow");

        backButton.onClick.AddListener(Back);
        scoreText.text = $"Score: {ScoreManager.Instance.GetScore()}";
        goldText.text = $"Gold: {GoldManager.Instance.CurrentGold} (+{ScoreManager.Instance.GetScore() * 1.5f})";
    }

    public override void OnPopped()
    {
        backButton.onClick.RemoveListener(Back);
        
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

    private void Back()
    {
        UIManager.Instance.Pop();
        GameManager.Instance.ChangeLevel("MainMenu");
    }
}
