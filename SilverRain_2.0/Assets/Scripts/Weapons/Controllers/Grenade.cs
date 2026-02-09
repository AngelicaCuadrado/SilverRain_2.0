using System.Collections;
using UnityEngine;

public class Grenade : Weapon
{
    [Header("Throw Settings")]
    [SerializeField, Tooltip("How far forward the grenade will be thrown")]
    private float throwForce = 12f;
    [SerializeField, Tooltip("How far upward the grenade will be thrown")]
    private float upwardForce = 4f;
    [Header("Spawn Position Offsets")]
    [SerializeField, Tooltip("How far forward the grenade will spawn relative to the camera")]
    private float spawnOffsetForward = 0.6f;
    [SerializeField, Tooltip("How far above the grenade will spawn relative to the camera")]
    private float spawnOffsetUp = 3.5f;
    [SerializeField, Tooltip("How far to the side the grenade will spawn relative to the camera")]
    private float spawnOffsetSide = 0.5f;
    [Header("References")]
    [SerializeField, Tooltip("The position from which grenades will spawn")]
    private Transform firePoint;

    private void Update()
    {
        if (cam == null) return;
        //Make the grenade launcher follow the camera's position and rotation
        Vector3 desiredPos = cam.position + (cam.forward * spawnOffsetForward) + (cam.up * spawnOffsetUp) + (cam.right * spawnOffsetSide);
        transform.position = desiredPos;
        transform.rotation = Quaternion.LookRotation(-cam.forward, Vector3.up);
    }

    public override void Attack()
    {
        Vector3 spawnPos = firePoint.position;
        //Vector3 spawnPos = cam.position + (cam.forward * spawnOffsetForward) + (cam.up * spawnOffsetUp) + (cam.right * spawnOffsetSide);
        Quaternion spawnRot = cam.rotation;

        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, spawnPos, spawnRot);

        Rigidbody rb = projObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Grenade: grenade projectile prefab has no Rigidbody.");
            return;
        }

        //Build throw direction
        Vector3 throwDirection =
            (cam.forward * throwForce) +
            (cam.up * upwardForce);

        rb.linearVelocity = throwDirection * 0.8f;
        rb.angularVelocity = Random.insideUnitSphere * 5f;

        //Initialize grenade behaviour
        var grenadeScript = projObj.GetComponent<GrenadeProjectile>();
        if (grenadeScript != null)
        {
            grenadeScript.Init(this, weaponStats.Damage, weaponStats.Size);
        }
    }

    public override void LevelUp()
    {
        //Ensure we don't exceed max level
        if (weaponLevel >= maxWeaponLevel) return;
        //Increase weapon level and recalculate stats
        weaponLevel++;
        weaponStats.CalculateStat(StatType.AttackDamage);
        weaponStats.CalculateStat(StatType.Cooldown);
        weaponStats.CalculateStat(StatType.Size);
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
        //Cache the main camera transform
        if (Camera.main != null) { cam = Camera.main.transform; }
        else { Debug.LogWarning("Grenade: no Camera.main found. Ensure a camera has the MainCamera tag."); }
        base.OnActivate();
    }

    public override IEnumerator OnDuration()
    {
        Attack();
        StartCoroutine(OnCooldown());
        yield break;
    }

    public override void UpdateDescription()
    {
        uiData.UpdateDescription(weaponLevel,
            "Damage", weaponStats.GetCurrentStatsForUI(StatType.AttackDamage), weaponStats.GetNextLevelStatsForUI(StatType.AttackDamage),
            "Cooldown", weaponStats.GetCurrentStatsForUI(StatType.Cooldown), weaponStats.GetNextLevelStatsForUI(StatType.Cooldown),
            "Size", weaponStats.GetCurrentStatsForUI(StatType.Size), weaponStats.GetNextLevelStatsForUI(StatType.Size));
    }
}