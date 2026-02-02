using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectWindow : UIWindow
{
    [SerializeField] private UpgradeWindow upgradeWindowPrefab;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button backButton;
    [SerializeField] private Button upgradeButton;
    
    public override void OnPushed()
    {
        BindUIEvents();
    }

    public override void OnPopped()
    {
        UnbindUIEvents();
    }
    
    private void BindUIEvents()
    {
        level1Button.onClick.AddListener(LevelSelect("Level1"));
        level2Button.onClick.AddListener(LevelSelect("Level2"));
        level3Button.onClick.AddListener(LevelSelect("Level3"));
        backButton.onClick.AddListener(Back);
        upgradeButton.onClick.AddListener(Upgrade);
    }

    private void UnbindUIEvents()
    {
        level1Button.onClick.RemoveListener(LevelSelect("Level1"));
        level2Button.onClick.RemoveListener(LevelSelect("Level2"));
        level3Button.onClick.RemoveListener(LevelSelect("Level3"));
        backButton.onClick.RemoveListener(Back);
        upgradeButton.onClick.RemoveListener(Upgrade);
    }

    private UnityAction LevelSelect(string levelName)
    {
        // Temporary solution, may later invoke GameManager or SceneManager
        
        return () =>
        {
            UIManager.Instance.ShowLoading(true);
            SceneManager.LoadSceneAsync(levelName);
            UIManager.Instance.ShowLoading(false);
            UIManager.Instance.Clear();
        };
    }

    private void Back()
    {
        UIManager.Instance.Pop();
    }

    private void Upgrade()
    {
        UIManager.Instance.Push(upgradeWindowPrefab);
    }
}