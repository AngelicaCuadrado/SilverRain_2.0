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
public abstract class Weapon : TemporaryBuff
{
    [Header("Identification")]
    [SerializeField,Tooltip("Key used for identification")]
    protected WeaponType weaponType;
    [SerializeField, Tooltip("The key used to spawn projectiles from the pool")]
    protected string projectilePoolKey;
    [Space]

    [Header("Components")]
    [SerializeField,Tooltip("Optional GameObject for the weapon visuals")]
    protected GameObject weaponVisual;
    [SerializeField,Tooltip("Stats component, must be attached to the weapon's GameObject")]
    protected WeaponStats weaponStats;
    [SerializeField, Tooltip("The main camera transform that the weapon follows")]
    protected Transform cam;
    [Space]

    [Header("Spawn Position Offsets")]
    [SerializeField, Tooltip("How far forward the weapon will spawn relative to the camera")]
    protected float spawnOffsetForward;
    [SerializeField, Tooltip("How far above the weapon will spawn relative to the camera")]
    protected float spawnOffsetUp;
    [SerializeField, Tooltip("How far to the side the weapon will spawn relative to the camera")]
    protected float spawnOffsetSide;
    [Space]

    [Header("Events")]
    public UnityEvent<WeaponType, Weapon> OnWeaponProjectileSpawn;
    public UnityEvent<WeaponType, GameObject[], Vector3> OnWeaponHit;

    //Properties
    public WeaponType WeaponType => weaponType;

    //Methods
    #region TemporaryBuff Implementation
    public override void Start()
    {
        base.Start();
        // Deactivate visual if possible
        if (weaponVisual != null)
        {
            weaponVisual.SetActive(false);
        }
    }
    public override void SetAvailable(bool availability)
    {
        isAvailable = availability;
        WeaponManager.Instance.HandleAvailabilityChange(this, availability);
    }
    public override void LevelUp()
    {
        base.LevelUp();
    }
    public override void ResetLevels()
    {
        base.ResetLevels();
        weaponStats.ResetWeaponStats();
    }
    public override void UpdateDescription() { }
    #endregion

    #region Weapon Attack Lifecycle
    public virtual void OnActivate()
    {
        //Cache the main camera transform
        if (Camera.main != null) { cam = Camera.main.transform; }
        else { Debug.LogWarning("Weapon: no Camera.main found. Ensure a camera has the MainCamera tag."); }

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
    #endregion

    #region Event Handling
    public virtual void HandleProjectileSpawn()
    {
        OnWeaponProjectileSpawn?.Invoke(weaponType, this);
    }

    public virtual void HandleWeaponHit(GameObject[] obj, Vector3 pos)
    {
        OnWeaponHit?.Invoke(weaponType, obj, pos);
    }
    #endregion
}