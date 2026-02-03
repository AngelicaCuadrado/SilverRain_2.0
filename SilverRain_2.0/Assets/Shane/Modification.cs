using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class Modification : MonoBehaviour, ITemporary
{
    [SerializeField] string modificationName;
    [SerializeField] string description;
    bool isAvailable;
    UnityEvent<ITemporary> OnAvailabilityChanged;
    public void LevelUp()
    {
        try
        {
            ModificationManager.instance.AddModification(this);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void ResetLevels()
    {
        try
        {
            ModificationManager.instance.RemoveModification(this);
        }
        catch( Exception e )
        {
            Debug.LogException(e);
        }
    }

    public void SetAvailable(bool isAvailable)
    {
        this.isAvailable = isAvailable;
    }

    public void UpdateDescription()
    { 

    }

    public virtual void Activate() 
    {
        isAvailable = true;
    }

    public abstract void ApplyEffect();

}
