using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Modification : TemporaryBuff
{
    [SerializeField, Tooltip("")]
    private ModificationID id; //For object identification
    [SerializeField,Tooltip("")]
    private WeaponType requiredWeapon = WeaponType.None;

    // Properties
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
