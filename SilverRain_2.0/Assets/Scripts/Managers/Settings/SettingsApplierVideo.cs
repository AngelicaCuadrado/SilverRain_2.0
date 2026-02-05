using UnityEngine;

public static class SettingsApplierVideo
{
    public static void Apply(VideoSettings v)
    {
        v.Clamp();

        // Quality level
        if (QualitySettings.names.Length > 0)
            QualitySettings.SetQualityLevel(v.qualityIndex, applyExpensiveChanges: true);

        // Fullscreen mode
        var mode = v.fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        // Resolution
        Screen.SetResolution(v.width, v.height, mode);
    }
}