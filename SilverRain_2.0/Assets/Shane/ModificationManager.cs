using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ModificationManager : MonoBehaviour
{
    List<Modification> allModifications;
    List<Modification> currentModifications;

    public static ModificationManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

   public void AddModification(Modification modification) 
    {
        if (!currentModifications.Contains(modification)) 
        {
            currentModifications.Add(modification);
        }
    }

    public void RemoveModification(Modification modification) 
    {
        currentModifications.Remove(modification);
    }

   public void ResetModifications()
    {
        currentModifications.Clear();
    }

    public float GetStatModifications(StatType type) 
    {
        float value = 0f;
        foreach (Modification modification in currentModifications)
        {
            if(modification is StatModification statModification)
            {
                value += statModification.GetModifyValue(type);
            }
        }
        return value;
    }

    public float GetWeaponStatModification(WeaponType weapon, StatType stat) 
    {
        float value = 0f;
        foreach (Modification modification in currentModifications)
        {
            if(modification is WeaponModification weaponModification)
            {
                value += weaponModification.GetModifyValue(weapon, stat);
            }
        }
        return value;
    }
}
