using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Modification : TemporaryBuff
{
    [SerializeField, Tooltip("")]
    private string modificationName; //For UI
    [SerializeField, Tooltip("")]
    private string description;
    [SerializeField, Tooltip("")]
    private ModificationID id; //For object identification

    // Properties
    public string ModificationName => modificationName;
    public string Description => description;
    public ModificationID Id => id;

    public override void LevelUp()
    {
        ModificationManager.Instance.AddModification(this);
    }

    public override void ResetLevels()
    {
        SetAvailable(isAvailableAtStart);
    }

    public override void SetAvailable(bool availability)
    {
        isAvailable = availability;
        ModificationManager.Instance.HandleAvailabilityChange(this, availability);
    }

    public override void UpdateDescription()
    {
        uiData.UpdateDescription();
    }

    public virtual void Activate() { }

    public virtual void Deactivate() { }

    public virtual void ApplyEffect() { }
}
