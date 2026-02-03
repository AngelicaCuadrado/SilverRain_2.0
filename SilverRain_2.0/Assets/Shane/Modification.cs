using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class Modification : MonoBehaviour, ITemporary
{
    bool isAvailable;
    UnityEvent<ITemporary> OnAvailabilityChanged;
    public void LevelUp()
    {

    }

    public void ResetLevels()
    {

    }

    public void SetAvailable(bool isAvailable)
    {

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
