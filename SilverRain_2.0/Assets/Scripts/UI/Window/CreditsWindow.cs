using UnityEngine;
using UnityEngine.UI;

public class CreditsWindow : UIWindow
{
    [SerializeField] private Button backButton;
    [SerializeField] private CreditsManager creditsManager;

    private void Awake()
    {
        if (creditsManager == null)
            creditsManager = GetComponentInChildren<CreditsManager>(true);
    }
    
    public override void OnPushed()
    {
        BindUIEvents();
        creditsManager?.PlayTimeline();
    }

    public override void OnPopped()
    {
        UnbindUIEvents();
        creditsManager?.StopTimeline();
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
