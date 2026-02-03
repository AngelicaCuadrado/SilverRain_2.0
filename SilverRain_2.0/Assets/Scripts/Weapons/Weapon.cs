using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class Weapon : MonoBehaviour, ITemporary
{
    [SerializeField,Tooltip("Current weapon level, 0 = unaquired")]
    protected int weaponLevel = 0;
    [SerializeField,Tooltip("Maximum weapon level")]
    protected int maxWeaponLevel = 5;
    [SerializeField,Tooltip("Key used for identification")]
    protected WeaponType weaponType;
    [SerializeField,Tooltip("GameObject of the weapon")]
    protected GameObject weaponPrefab;
    [SerializeField,Tooltip("Stats component, must be attached to the weapon's GameObject")]
    protected WeaponStats weaponStats;
    [SerializeField, Tooltip("UI handling component, must be attached to the weapon's GameObject")]
    protected WeaponUI weaponUI;
    [SerializeField, Tooltip("Represents whether the weapon can appear as an option when leveling up")]
    protected bool isAvailable;

    //Events
    public UnityEvent<IUpgradeable> OnUpgradeLevelChanged;
    public UnityEvent<ITemporary> OnAvailabilityChanged;

    //Properties
    public int WeaponLevel => weaponLevel;
    public int MaxWeaponLevel => maxWeaponLevel;
    public WeaponType WeaponType => weaponType;
    public GameObject WeaponPrefab => weaponPrefab;
    public WeaponStats WeaponStats => weaponStats;
    public WeaponUI WeaponUI => weaponUI;
    public bool IsAvailable => isAvailable;

    //Methods
    public abstract void LevelUp();
    public abstract void ResetLevels();
    public abstract void UpdateDescription();
    public abstract void SetAvailability(bool availability);
    public abstract void OnActivate();
    public abstract IEnumerator OnCooldown();
    public abstract IEnumerator OnDuration();
    public abstract void Attack();
}



//TEMPORARY to avoid errors ---DELETE THIS--- ☺
public interface IUpgradeable{}
public interface ITemporary{}