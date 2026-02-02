using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : UIWindow
{
    [SerializeField] private Button backButton;
    
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
        backButton.onClick.AddListener(Back);
    }

    private void UnbindUIEvents()
    {
        backButton.onClick.RemoveListener(Back);
    }

    private void Back()
    {
        UIManager.Instance.Pop();
    }
}
