using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Modification : MonoBehaviour, ITemporary
{
    [SerializeField] string modificationName; //For UI
    [SerializeField] string description;
    [SerializeField] ModificationID id; //For object identification
    bool isAvailable;
    public UnityEvent<ITemporary> OnAvailabilityChanged;
    UITemporary uiData;

    public UITemporary UIData => uiData;

    public void LevelUp()
    {
       ModificationManager.Instance.AddModification(this);
    }

    public void ResetLevels()
    {
        SetAvailable(true);
    }

    public void SetAvailable(bool isAvailable)
    {
        this.isAvailable = isAvailable;
        OnAvailabilityChanged?.Invoke(this);
    }

    public void UpdateDescription()
    { 

    }

    public virtual void Activate() 
    {

    }

    public abstract void ApplyEffect();

    public string GetName() 
    {
        return modificationName;
    }

    public string GetDescription()
    { 
        return description; 
    }

    public ModificationID GetId() 
    { 
        return id;
    }

}
