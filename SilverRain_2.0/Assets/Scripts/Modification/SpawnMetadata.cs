using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Small metadata attached to each projectile instance to avoid recursive mod processing.
/// </summary>
public class SpawnMetadata : MonoBehaviour
{
    // 0 for originally spawned by weapon, incremented by mods that spawn derived projectiles
    public int Generation = 0;

    // IDs of modifications that have already processed this projectile
    public List<ModificationID> ProcessedModIds = new();

    public bool HasProcessed(ModificationID modId)
    {
        return ProcessedModIds != null && ProcessedModIds.Contains(modId);
    }

    public void MarkProcessed(ModificationID modId)
    {
        ProcessedModIds ??= new();
        if (!ProcessedModIds.Contains(modId)) ProcessedModIds.Add(modId);
    }
}