using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModificationManager : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private List<Modification> allModifications;
    [Tooltip("")]
    private List<Modification> currentModifications;

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
        if (!currentModifications.Any(m => m.Id == modification.Id))
        {
            currentModifications.Add(modification);

            var catalogItem = allModifications.FirstOrDefault(m => m.Id == modification.Id);
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

    private void Start()
    {
        //If all weapons is going to be provate, subscribing to events needs to be handled differently
        //foreach (var weapon in WeaponManager.Instance.allWeaponsList) 
        //{
        //    weapon.OnWeaponHit.AddListener();
        //}
    }
}
