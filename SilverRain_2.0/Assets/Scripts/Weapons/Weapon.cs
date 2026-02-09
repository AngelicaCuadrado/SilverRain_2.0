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
    [Header("Level")]
    [SerializeField,Tooltip("Current weapon level, 0 = unaquired")]
    protected int weaponLevel = 0;
    [SerializeField,Tooltip("Maximum weapon level")]
    protected int maxWeaponLevel = 5;

    [Header("Identification")]
    [SerializeField,Tooltip("Key used for identification")]
    protected WeaponType weaponType;
    [SerializeField, Tooltip("The key used to spawn projectiles from the pool")]
    protected string projectilePoolKey;

    [Header("Components")]
    [SerializeField,Tooltip("Optional GameObject for the weapon visuals")]
    protected GameObject weaponVisual;
    [SerializeField,Tooltip("Stats component, must be attached to the weapon's GameObject")]
    protected WeaponStats weaponStats;
    [SerializeField, Tooltip("The main camera transform that the weapon follows")]
    protected Transform cam;
    [SerializeField, Tooltip("UI handling component, must be attached to the weapon's GameObject")]
    protected UITemporary uiData;

    [Header("Availability")]
    [SerializeField, Tooltip("Represents whether the weapon can appear as an option when leveling up")]
    protected bool isAvailable;
    [SerializeField, Tooltip("Represents whether the weapon is available when the level starts")]
    protected bool isAvailableAtStart;

    [Header("Events")]
    public UnityEvent<ITemporary> OnWeaponLevelChanged;
    public UnityEvent<ITemporary> OnAvailabilityChanged;
    public UnityEvent<WeaponType, Weapon> OnWeaponProjectileSpawn;
    public UnityEvent<WeaponType, GameObject[], Vector3> OnWeaponHit;


    //Properties
    public int WeaponLevel => weaponLevel;
    public int MaxWeaponLevel => maxWeaponLevel;
    public WeaponType WeaponType => weaponType;
    public string ProjectilePoolKey => projectilePoolKey;
    public GameObject WeaponVisual => weaponVisual;
    public WeaponStats WeaponStats => weaponStats;
    public Transform Cam => cam;
    public UITemporary UIData { get { return uiData; } }
    public bool IsAvailable => isAvailable;
    public bool IsAvailableAtStart => isAvailableAtStart;

    //Methods
    public virtual void Start()
    {
        // Make available in BuffCardManager
        SetAvailable(isAvailableAtStart);
        // Deactivate visual if possible
        if (weaponVisual != null)
        {
            weaponVisual.SetActive(false);
        }
        //Update UI for buff cards
        UpdateDescription();
    }
    public abstract void LevelUp();
    public virtual void OnActivate()
    {
        // Activate visual if possible
        if (weaponVisual != null)
        {
            weaponVisual.SetActive(true);
        }
        // Start duration coroutine
        StartCoroutine(OnDuration());
    }
    public virtual IEnumerator OnCooldown()
    {
        yield return new WaitForSeconds(weaponStats.Cooldown);
        StartCoroutine(OnDuration());
    }
    public abstract IEnumerator OnDuration();
    public abstract void Attack();
    public virtual void ResetLevels()
    {
        // Reset level and stats
        weaponLevel = 0;
        weaponStats.ResetWeaponStats();
        // Update UI
        OnWeaponLevelChanged?.Invoke(this);
        UpdateDescription();
        // Reset availability
        SetAvailable(true);
    }
    public virtual void SetAvailable(bool availability)
    {
        isAvailable = availability;
        WeaponManager.Instance.HandleAvailabilityChange(this, availability);
        print("Available");
    }
    public abstract void UpdateDescription();
    public virtual void HandleProjectileSpawn()
    {
        OnWeaponProjectileSpawn?.Invoke(weaponType, this);
    }

    public virtual void HandleWeaponHit(GameObject[] obj, Vector3 pos)
    {
        OnWeaponHit?.Invoke(weaponType, obj, pos);
    }
}