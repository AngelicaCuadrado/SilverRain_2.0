using System;
using UnityEngine;

[Serializable]
public class SettingsData
{
    public const int CurrentVersion = 1;

    public bool initialized = true;
    public int version = CurrentVersion;

    public AudioSettings audio = new();
    public VideoSettings video = new();

    public void Clamp()
    {
        audio.Clamp();
        video.Clamp();
    }
}

/// <summary>
/// Audio settings in normalized form (0..1) plus mute flags.
/// </summary>
[Serializable]
public class AudioSettings
{
    [Range(0f, 1f)] public float master = 1f;
    [Range(0f, 1f)] public float bgm = 1f;
    [Range(0f, 1f)] public float sfx = 1f;

    public bool masterMuted = false;
    public bool bgmMuted = false;
    public bool sfxMuted = false;

    public void Clamp()
    {
        master = Mathf.Clamp01(master);
        bgm = Mathf.Clamp01(bgm);
        sfx = Mathf.Clamp01(sfx);
    }
}

/// <summary>
/// Video/display settings.
/// </summary>
[Serializable]
public class VideoSettings
{
    public bool fullscreen = true;
    public int width = 1920;
    public int height = 1080;
    public int qualityIndex = 0;

    public void Clamp()
    {
        width = Mathf.Max(640, width);
        height = Mathf.Max(360, height);

        var maxQ = Mathf.Max(0, QualitySettings.names.Length - 1);
        qualityIndex = Mathf.Clamp(qualityIndex, 0, maxQ);
    }
}
