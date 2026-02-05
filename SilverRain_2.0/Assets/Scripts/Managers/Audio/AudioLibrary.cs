using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Library", menuName = "Audio/AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    [Tooltip("List of audio entries. Ids should be unique.")]
    public ClipEntry[] clips;

    [Serializable]
    public struct ClipEntry
    {
        public string id;
        public AudioClip clip;
    }

    private Dictionary<string, AudioClip> _cache;

    private void OnEnable()
    {
        RebuildCache();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        RebuildCache();
    }
#endif

    /// <summary>
    /// Finds an AudioClip by id. Returns null if not found.
    /// </summary>
    public AudioClip Find(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (_cache == null) RebuildCache();

        return _cache.TryGetValue(id, out var clip) ? clip : null;
    }

    private void RebuildCache()
    {
        _cache = new Dictionary<string, AudioClip>(StringComparer.Ordinal);

        if (clips == null) return;

        for (int i = 0; i < clips.Length; i++)
        {
            var id = clips[i].id;
            var clip = clips[i].clip;

            if (string.IsNullOrEmpty(id)) continue;

            if (_cache.ContainsKey(id))
            {
                Debug.LogWarning($"[AudioLibrary] Duplicate id '{id}' in '{name}'. First entry will be used.");
                continue;
            }

            _cache[id] = clip;
        }
    }
}
