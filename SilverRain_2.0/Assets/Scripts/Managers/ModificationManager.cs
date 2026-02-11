using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModificationManager : MonoBehaviour
{
    public List<Modification> allModifications;
    List<Modification> currentModifications;

    public static ModificationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
            if(modification is IStatModifier statModification)
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
            if(modification is IWeaponModifier weaponModification)
            {
                value += weaponModification.GetModifyValue(weapon, stat);
            }
        }
        return value;
    }

    private void Start()
    {
        foreach (var weapon in WeaponManager.Instance.AllWeapons.Values) 
        {
            weapon.OnWeaponHit.AddListener();
        }
    }
}
