using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    //Singleton instance
    public static WeaponManager Instance { get; private set; }

    //Serialized fields
    [SerializeField, Tooltip("List of all weapons currently implemented")]
    private List<WeaponEntry> allWeaponsList;
    //Dictionary of all weapons
    private Dictionary<WeaponType, Weapon> allWeapons;
    //Dictionary of weapons active in the current level
    private Dictionary<WeaponType, Weapon> currentWeapons;
    [SerializeField, Tooltip("Maximum amount of weapon allowed to be active in a level")]
    private int maxWeapons;
    [SerializeField, Tooltip("The weapon active at the start of a level")]
    private Weapon initialWeapon;

    [Header("Pools")]
    [SerializeField, Tooltip("ObjectPooler reference containing all the projectile pools")]
    private ObjectPooler projectilePool;
    [SerializeField, Tooltip("ObjectPooler reference containing all the VFX pools")]
    private ObjectPooler effectsPool;

    //Properties
    public Dictionary<WeaponType, Weapon> AllWeapons => allWeapons;
    public Dictionary<WeaponType, Weapon> CurrentWeapons => currentWeapons;
    public int MaxWeapons => maxWeapons;
    public Weapon InitialWeapon { get => initialWeapon; set => initialWeapon = value; }
    public ObjectPooler ProjectilePool => projectilePool;
    public ObjectPooler EffectsPool => effectsPool;

    //Events
    public UnityEvent<ITemporary, bool> OnWeaponAvailabilityChange;

    private void Awake()
    {
        //Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //Initialize all weapons dictionary
        allWeapons = new Dictionary<WeaponType, Weapon>();
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
        //Initialize current weapons dictionary
        currentWeapons = new Dictionary<WeaponType, Weapon>();
    }

    public void AddWeapon(WeaponType type)
    {
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

    public void HandleAvailabilityChange(ITemporary weapon, bool isAvailable)
    {
        OnWeaponAvailabilityChange.Invoke(weapon, isAvailable);
    }
}