using UnityEngine;
/// <summary>
/// High-level UI Manager
///
/// This manager wraps:
/// - UIStack: modal windows/pages (Settings, Pause, MainMenu, etc.)
/// - UIOverlay: non-stacked UI (HUD, Toast, always-on widgets)
/// - Loading: a simple loading indicator panel
///
/// Rule of thumb:
/// - Use Push/Pop for "screens" that block interaction behind them.
/// - Use ShowOverlay for UI that should stay visible or be shown independently.
/// </summary>
public class UIManager : MonoBehaviour
{
    // Singleton mode for UI Manager
    public static UIManager Instance { get; private set; }

    [SerializeField] private UIRoot _root;
    
    public UIRoot Root => _root;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // Stack window (LIFO)
    public UIWindow Push(UIWindow windowPrefab) => _root.Stack.Push(windowPrefab);
    public void Pop() => _root.Stack.Pop();
    public void Clear() => _root.Stack.Clear();
    
    // Overlay window (by channel)
    public UIWindow ShowOverlay(UIWindow prefab) => _root.Overlay.Show("Default", prefab);
    public UIWindow ShowOverlay(string channel, UIWindow prefab) => _root.Overlay.Show(channel, prefab);
    public void ClearOverlay() => _root.Overlay.Clear("Default");
    public void ClearOverlay(string channel) => _root.Overlay.Clear(channel);
    public void ClearAllOverlay() => _root.Overlay.ClearAll();
    
    // Loading indicator
    public void ShowLoading(bool show)
    {
        if (_root.Loading == null) return;
        if (show) _root.Loading.Show();
        else _root.Loading.Hide();
    }
}