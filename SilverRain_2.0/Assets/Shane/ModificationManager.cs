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

   private void ResetModifications()
    {
        currentModifications.Clear();
    }

    float GetStatModifications(StatType type) 
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

    float GetWeaponStatModification(WeaponType weapon, StatType stat) 
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
