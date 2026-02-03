using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModificationManager : MonoBehaviour
{
    public List<Modification> allModifications;
    List<Modification> currentModifications;

    public static ModificationManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        currentModifications = new List<Modification>();

        DontDestroyOnLoad(gameObject);
    }

   public void AddModification(Modification modification)
   {
        if (!currentModifications.Any(m => m.GetId() == modification.GetId()))
        {
            currentModifications.Add(modification);

            var catalogItem = allModifications.FirstOrDefault(m => m.GetId() == modification.GetId());
            if (catalogItem != null)
            {
                catalogItem.SetAvailable(false);
            }
        }
   }

   public void ResetModifications()
    {
        foreach (Modification modification in currentModifications) 
        {
            var catalogItem = allModifications.FirstOrDefault(m => m.GetId() == modification.GetId());
            if (catalogItem != null)
            {
                catalogItem.ResetLevels();
            }
        }
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
