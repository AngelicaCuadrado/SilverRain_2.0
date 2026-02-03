using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //Singleton instance
    public static WeaponManager Instance { get; private set; }

    //Serialized fields
    [SerializeField, Tooltip("List of all weapons currently implemented")]
    private Dictionary<WeaponType, Weapon> allWeapons;
    [SerializeField, Tooltip("List of weapons active in the current level")]
    private Dictionary<WeaponType, Weapon> currentWeapons;
    [SerializeField, Tooltip("Maximum amount of weapon allowed to be active in a level")]
    private int maxWeapons;
    [SerializeField, Tooltip("The weapon active at the start of a level")]
    private Weapon initialWeapon;
    [SerializeField, Tooltip("ObjectPooler reference containing all the projectile pools")]
    private ObjectPooler projectilePool;

    //Properties
    public Dictionary<WeaponType, Weapon> AllWeapons => allWeapons;
    public Dictionary<WeaponType, Weapon> CurrentWeapons => currentWeapons;
    public int MaxWeapons => maxWeapons;
    public Weapon InitialWeapon { get => initialWeapon; set => initialWeapon = value; }
    public ObjectPooler ProjectilePool => projectilePool;

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
        //Initialize current weapons dictionary
        currentWeapons = new Dictionary<WeaponType, Weapon>();
    }

    private void AddWeapon(WeaponType type)
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
                        allWeapons[weaponType].SetAvailability(false);
                    }
                }
            }
        }
    }
    
    private void ResetWeapons()
    {
        //Reset all current weapons
        foreach (var weapon in currentWeapons.Values)
        {
            weapon.ResetLevels();
        }
        //Reset current weapons list
        currentWeapons.Clear();
    }
    
    //Ido - I'm not sure if we need to find a specific weapon from
    //the current weapons,if this has 0 references we should delete it
    public Weapon GetOneCurrentWeapon(WeaponType type)
    {
        if (currentWeapons.ContainsKey(type))
        {
            return currentWeapons[type];
        }
        return null;
    }
}
