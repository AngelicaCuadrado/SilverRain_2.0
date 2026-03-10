using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    //Singleton instance
    public static WeaponManager Instance { get; private set; }

    [Header("Weapon Lists")]
    [SerializeField, Tooltip("List of all weapons currently implemented")]
    private List<WeaponEntry> allWeaponsList;
    [Tooltip("Dictionary of all weapon and their WeaponType as a key")]
    private Dictionary<WeaponType, Weapon> allWeapons;
    [Tooltip("Dictionary of weapons active in the current level")]
    private Dictionary<WeaponType, Weapon> currentWeapons;

    [Header("Evolution Lists")]
    [SerializeField, Tooltip("List of all weapon evolutions currently implemented")]
    private List<WeaponEntry> allEvolutionsList;
    [SerializeField, Tooltip("Dictionary of all weapon evolutions and their WeaponType as a key")]
    private Dictionary<WeaponType, Weapon> allEvolutions;

    [Header("Amount")]
    [SerializeField, Tooltip("Maximum amount of weapon allowed to be active in a level")]
    private int maxWeapons;
    [SerializeField, Tooltip("The weapon active at the start of a level")]
    private WeaponType initialWeapon;

    [Header("Pools")]
    [SerializeField, Tooltip("ObjectPooler reference containing all the projectile pools")]
    private ObjectPooler projectilePool;
    [SerializeField, Tooltip("ObjectPooler reference containing all the VFX pools")]
    private ObjectPooler effectsPool;

    [Header("Events")]
    public UnityEvent<TemporaryBuff, bool> OnWeaponAvailabilityChange;
    public UnityEvent<WeaponType> OnWeaponMaxLevelReached;
    public UnityEvent<WeaponType> OnWeaponAquired;
    public UnityEvent<WeaponType, Weapon> OnWeaponProjectileSpawn;
    public UnityEvent<WeaponType, GameObject[], Vector3> OnWeaponHit;
    
    //Properties
    public Dictionary<WeaponType, Weapon> AllWeapons => allWeapons;
    public WeaponType InitialWeapon { get => initialWeapon; set => initialWeapon = value; }
    public ObjectPooler ProjectilePool => projectilePool;
    public ObjectPooler EffectsPool => effectsPool;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize all weapons dictionary
        allWeapons = new();
        foreach (var entry in allWeaponsList)
        {
            if (!allWeapons.ContainsKey(entry.type))
            {
                allWeapons.Add(entry.type, entry.weapon);
            }
            else
            {
                Debug.LogWarning($"Duplicate weapon type {entry.type} found in allWeaponsList.");
            }
        }

        // Initialize all evolutions dictionary
        allEvolutions = new();
        foreach (var entry in allEvolutionsList)
        {
            if (!allEvolutions.ContainsKey(entry.type))
            {
                allEvolutions.Add(entry.type, entry.weapon);
            }
            else
            {
                Debug.LogWarning($"Duplicate weapon type {entry.type} found in allEvolutionsList.");
            }
        }

        // Initialize current weapons dictionary
        currentWeapons = new Dictionary<WeaponType, Weapon>();

    }

    private void Start()
    {
        // Add the initial weapon
        if (initialWeapon != WeaponType.None)
        {
            AddWeapon(initialWeapon);
        }

        // Subscribe to modification events
        ModificationManager.Instance.OnWeaponStatModificationChange.AddListener(RecalculateStats);
    }
    public void AddWeapon(WeaponType type)
    {
        if (type == WeaponType.None) { return; }

        //Level up weapon if already present
        if (currentWeapons.ContainsKey(type))
        {
            currentWeapons[type].LevelUp();
        }
        //Add new weapon if not present
        else
        {
            if (!allWeapons.ContainsKey(type))
            {
                Debug.LogError($"Weapon of type {type} not found in allWeapons dictionary.");
                return;
            }
            currentWeapons.Add(type, allWeapons[type]);
            //Increase level to 1
            currentWeapons[type].LevelUp();
            //Activate the weapon
            currentWeapons[type].OnActivate();
            //Invoke weapon acquired event
            OnWeaponAquired.Invoke(type);   
            //Check if max weapon amount reached
            if (currentWeapons.Count >= maxWeapons)
            {
                //Make all other weapons unavailable
                foreach (var weaponType in allWeapons.Keys)
                {
                    if (!currentWeapons.ContainsKey(weaponType))
                    {
                        allWeapons[weaponType].SetAvailable(false);
                    }
                }
            }
        }
    }

    public void AddEvolution(WeaponType type)
    {
        if (type == WeaponType.None) { return; }
        if (!allEvolutions.ContainsKey(type))
        {
            Debug.LogError($"Weapon evolution of type {type} not found in allEvolutions dictionary.");
            return;
        }
        if (!currentWeapons.ContainsKey(type))
        {
            Debug.LogWarning($"Weapon of type {type} not present in currentWeapons. Evolution cannot be added.");
            return;
        }

        // Deactivate and remove pre-evolved weapon
        currentWeapons[type].DeactivateWeapon();
        currentWeapons.Remove(type);

        currentWeapons.Add(type, allEvolutions[type]);
        //Increase level to 1
        currentWeapons[type].LevelUp();
        //Activate the weapon
        currentWeapons[type].OnActivate();
        //Invoke weapon acquired event
        OnWeaponAquired.Invoke(type);
    }
    public void ResetWeapons()
    {
        //Reset all current weapons
        foreach (var weapon in currentWeapons.Values)
        {
            weapon.ResetLevels();
        }
        //Reset current weapons list
        currentWeapons.Clear();
    }

    #region Event Handling
    public void HandleAvailabilityChange(TemporaryBuff weapon, bool isAvailable)
    {
        OnWeaponAvailabilityChange.Invoke(weapon, isAvailable);
    }

    public void HandleMaxLevelReached(WeaponType type)
    {
        OnWeaponMaxLevelReached.Invoke(type);
    }

    public void RecalculateStats(WeaponType weaponType, StatType statType)
    {
        if (currentWeapons.ContainsKey(weaponType))
        {
            currentWeapons[weaponType].RecalculateStats(statType);
        }
    }

    public void HandleWeaponHit(WeaponType weaponType, GameObject[] hitObjects, Vector3 hitPoint)
    {
        OnWeaponHit.Invoke(weaponType, hitObjects, hitPoint);
    }

    public void HandleProjectileSpawn(WeaponType weaponType, Weapon weapon)
    {
        OnWeaponProjectileSpawn.Invoke(weaponType, weapon);
    }
    #endregion
}