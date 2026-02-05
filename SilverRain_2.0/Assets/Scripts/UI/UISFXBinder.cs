using UnityEngine;
using UnityEngine.UI;

public static class UISFXBinder
{
    public static void BindButtons(UIWindow pageRoot, string sfxKey = "sfx_click", bool includeInactive = true)
    {
        if (!pageRoot) return;

        var buttons = pageRoot.GetComponentsInChildren<Button>(includeInactive);
        foreach (var btn in buttons)
        {
            if (!btn) continue;

            // add this component if the botton does not play click sfx
            if (btn.GetComponent<NoUISFX>() != null) continue;

            // avoid repeat binding
            if (btn.GetComponent<UISFXBoundTag>() != null) continue;

            btn.onClick.AddListener(() => AudioManager.Instance.PlaySFX(sfxKey));
            btn.gameObject.AddComponent<UISFXBoundTag>();
        }
    }
    
    private class UISFXBoundTag : MonoBehaviour { }
}

public class NoUISFX : MonoBehaviour { }