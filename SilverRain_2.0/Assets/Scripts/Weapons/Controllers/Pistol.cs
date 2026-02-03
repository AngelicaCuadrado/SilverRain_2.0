using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Pistol : Weapon
{
    [SerializeField, Tooltip("The position from which bullets will spawn")]
    private Transform firePoint;
    [SerializeField, Tooltip("The key used to spawn projectiles from the pool")]
    private string projectilePoolKey;
    [SerializeField, Tooltip("The main camera transform that the pistol follows")]
    private Transform cam;
    [SerializeField, Tooltip("How far forward the pistol is positioned relative to the camera")]
    private float spawnOffsetForward = 1f;
    [SerializeField, Tooltip("How far above the pistol is positioned relative to the camera")]
    private float spawnOffsetUp = -0.4f;

    //Properties
    public string ProjectilePoolKey => projectilePoolKey;

    private void Start()
    {
        //Cache the main camera transform
        if (Camera.main != null) { cam = Camera.main.transform; }
        else { Debug.LogWarning("Pistol: no Camera.main found. Ensure a camera has the MainCamera tag."); }
        //Ensure firePoint is assigned
        if (firePoint == null) { firePoint = transform; Debug.LogWarning("Pistol: no firePoint was assigned, defaulting to pistol trsnform"); }
    }

    private void Update()
    {
        if (cam == null) return;
        //Make the pistol follow the camera's position and rotation
        Vector3 desiredPos = cam.position + cam.forward * spawnOffsetForward + cam.up * spawnOffsetUp;
        transform.position = desiredPos;
        transform.rotation = Quaternion.LookRotation(-cam.forward, Vector3.up);
    }
    public override void Attack()
    {
        //Calculate spawn rotation so the projectile faces forward
        Quaternion spawnRot = Quaternion.LookRotation(firePoint.forward, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
        //Instantiate projectile
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, firePoint.position, spawnRot);
        //Initialize projectile
        var proj = projObj.GetComponent<PistolProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("Pistol: The instantiated projectile does not have a PistolProjectile component.");
            Destroy(projObj);
            return;
        }
        proj.Init(this, weaponStats.Damage, firePoint.forward, weaponStats.ProjectileSpeed);
    }

    public override void LevelUp()
    {
        //Ensure we don't exceed max level
        if (weaponLevel >= maxWeaponLevel) return;
        //Increase weapon level and recalculate stats
        weaponLevel++;
        weaponStats.CalculateStat(StatType.AttackDamage);
        weaponStats.CalculateStat(StatType.Cooldown);
        weaponStats.CalculateStat(StatType.ProjectileSpeed);
        //Update UI
        UpdateDescription();
        OnWeaponLevelChanged?.Invoke(this);
        //Check if we've reached max level
        if (weaponLevel >= maxWeaponLevel)
        {
            SetAvailable(false);
        }

    }

    public override void OnActivate()
    {
        gameObject.SetActive(true);
        StartCoroutine(OnCooldown());
    }

    public override IEnumerator OnCooldown()
    {
        yield return new WaitForSeconds(weaponStats.Cooldown);
        StartCoroutine(OnDuration());
    }

    public override IEnumerator OnDuration()
    {
        Attack();
        StartCoroutine(OnCooldown());
        yield break;
    }

    public override void ResetLevels()
    {
        //Reset level and stats
        weaponLevel = 0;
        weaponStats.ResetWeaponStats();
        //Update UI
        OnWeaponLevelChanged?.Invoke(this);
        weaponUI.UpdateDescription();
        //Reset availability
        SetAvailable(true);
    }

    public override void SetAvailable(bool availability)
    {
        isAvailable = availability;
        OnAvailabilityChanged?.Invoke(this);
    }

    public override void UpdateDescription()
    {
        weaponUI.UpdateDescription();
    }

    public override void HandleWeaponHit(GameObject obj)
    {
        OnWeaponHit?.Invoke(weaponType, obj);
    }
}