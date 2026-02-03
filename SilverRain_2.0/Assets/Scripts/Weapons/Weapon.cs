using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
/// <summary>
/// Represents an abstract base class for all weapon types, providing common properties, events, and behaviors for
/// weapons in the game.
/// </summary>
/// <remarks>Inherit from this class to implement specific weapon functionality, including leveling, activation,
/// cooldown, and attack logic. The Weapon class integrates with Unity's MonoBehaviour and supports temporary
/// availability through the ITemporary interface. It exposes events for upgrade and availability changes, and provides
/// access to associated components such as stats and UI. All weapon-specific logic should be implemented in derived
/// classes.</remarks>
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
    public UnityEvent<ITemporary> OnWeaponLevelChanged;
    public UnityEvent<ITemporary> OnAvailabilityChanged;
    public UnityEvent<WeaponType, Vector3> OnWeaponHit;

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
    public abstract void SetAvailable(bool availability);
    public abstract void OnActivate();
    public abstract IEnumerator OnCooldown();
    public abstract IEnumerator OnDuration();
    public abstract void Attack();
    public abstract void HandleWeaponHit(Vector3 pos);
}