using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Modification : TemporaryBuff
{
    [SerializeField, Tooltip("The unique identifier for this modification")]
    protected ModificationID id;
    [SerializeField, Tooltip("The type of weapon required for this modification")]
    protected WeaponType requiredWeapon = WeaponType.None;

    // Properties
    public ModificationID Id => id;
    public WeaponType RequiredWeapon => requiredWeapon;

    public override void Start()
    {
        base.Start();
        if (requiredWeapon == WeaponType.None) return; 
        WeaponManager.Instance.OnWeaponAquired.AddListener(OnRequirementMet);

        // Check initial weapon
        if (WeaponManager.Instance.InitialWeapon == requiredWeapon)
        {
            OnRequirementMet(requiredWeapon);
        }
    }

    public void OnRequirementMet(WeaponType type)
    {
        if (type == requiredWeapon)
        {
            SetAvailable(true);
            WeaponManager.Instance.OnWeaponAquired.RemoveListener(OnRequirementMet);
        }
    }

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

    public virtual void OnDestroy()
    {
        WeaponManager.Instance.OnWeaponAquired.RemoveListener(OnRequirementMet);
    }

    public virtual void Activate() { }

    public virtual void Deactivate() { }

    //public virtual void ApplyEffect() { }
}
