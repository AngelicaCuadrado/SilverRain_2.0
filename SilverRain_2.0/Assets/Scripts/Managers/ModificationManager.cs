using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ModificationManager : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private List<Modification> allModifications;
    [Tooltip("")]
    private List<Modification> currentModifications;

    // Events
    public static ModificationManager Instance { get; private set; }
    public UnityEvent<Modification, bool> OnModificationAvailabilityChange;
    public UnityEvent<WeaponType, StatType> OnWeaponStatModificationChange;
    public UnityEvent<ModificationID> OnModificationAquired;

    // Properties
    public List<Modification> AllModifications => allModifications;

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
        if (!currentModifications.Any(m => m.Id == modification.Id))
        {
            currentModifications.Add(modification);
            // Activate the modification's effects
            modification.Activate();

            var catalogItem = allModifications.FirstOrDefault(m => m.Id == modification.Id);
            if (catalogItem != null)
            {
                catalogItem.SetAvailable(false);
            }
            OnModificationAquired.Invoke(modification.Id);
        }
   }

   public void ResetModifications()
    {
        foreach (Modification modification in currentModifications) 
        {
            var catalogItem = allModifications.FirstOrDefault(m => m.Id == modification.Id);
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

    public void HandleAvailabilityChange(Modification modification, bool isAvailable)
    {
        OnModificationAvailabilityChange.Invoke(modification, isAvailable);
    }

    public void HandleWeaponStatModificationChange(WeaponType weapon, StatType stat)
    {
        OnWeaponStatModificationChange.Invoke(weapon, stat);
    }

    private void Start()
    {
        //foreach (var weapon in WeaponManager.Instance.allWeaponsList) 
        //{
        //    weapon.OnWeaponHit.AddListener();
        //}
    }
}
