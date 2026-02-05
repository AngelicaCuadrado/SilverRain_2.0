

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    /// <summary>Current settings data in memory.</summary>
    public SettingsData Data { get; private set; }

    /// <summary>Raised when settings are changed or restored.</summary>
    public UnityEvent OnChanged;

    private bool _dirty;
    private string _savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, "settings.json");

        LoadOrCreate();
        ApplyAll();
        _dirty = false;
    }

    private void LoadOrCreate()
    {
        if (File.Exists(_savePath))
        {
            try
            {
                var json = File.ReadAllText(_savePath);
                var data = JsonUtility.FromJson<SettingsData>(json);

                if (data != null && data.initialized)
                {
                    // Migration hook
                    if (data.version != SettingsData.CurrentVersion)
                    {
                        data.version = SettingsData.CurrentVersion;
                    }

                    data.Clamp();
                    Data = data;
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SettingsManager] Failed to load settings: {e.Message}");
            }
        }

        // First run or corrupted data: create defaults
        Data = new SettingsData();
        SaveToFile();
    }

    /// <summary>
    /// Marks settings as modified. Call this when user changes a value.
    /// </summary>
    public void MarkDirty()
    {
        _dirty = true;
        OnChanged?.Invoke();
    }

    /// <summary>
    /// Applies current settings to runtime (Audio/Video).
    /// Does NOT save to disk. Use for preview.
    /// </summary>
    public void ApplyAll()
    {
        Data.Clamp();

        SettingsApplierAudio.Apply(Data.audio);
        SettingsApplierVideo.Apply(Data.video);
    }

    /// <summary>
    /// Saves current settings to disk (only if dirty).
    /// </summary>
    public void Save()
    {
        if (!_dirty) return;

        Data.Clamp();
        SaveToFile();
        _dirty = false;
    }

    /// <summary>
    /// Forces save regardless of dirty state.
    /// </summary>
    public void ForceSave()
    {
        Data.Clamp();
        SaveToFile();
        _dirty = false;
    }

    private void SaveToFile()
    {
        try
        {
            var json = JsonUtility.ToJson(Data, prettyPrint: true);
            File.WriteAllText(_savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SettingsManager] Failed to save settings: {e.Message}");
        }
    }

    /// <summary>
    /// Resets settings to defaults and saves immediately.
    /// </summary>
    public void ResetToDefault()
    {
        Data = new SettingsData();
        SaveToFile();
        ApplyAll();
        OnChanged?.Invoke();
    }

    /// <summary>
    /// Exports a snapshot as JSON for Apply/Cancel workflow.
    /// </summary>
    public string ExportJsonSnapshot()
    {
        return JsonUtility.ToJson(Data);
    }

    /// <summary>
    /// Imports a snapshot and applies immediately.
    /// markDirty=false for Cancel (revert without saving).
    /// </summary>
    public void ImportJsonSnapshot(string json, bool markDirty)
    {
        if (string.IsNullOrEmpty(json)) return;

        var loaded = JsonUtility.FromJson<SettingsData>(json);
        if (loaded == null) return;

        loaded.Clamp();
        Data = loaded;
        _dirty = markDirty;

        ApplyAll();
        OnChanged?.Invoke();
    }
}