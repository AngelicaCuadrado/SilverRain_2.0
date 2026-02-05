using System.Collections;
using UnityEngine;

public class Grenade : Weapon
{
    [SerializeField, Tooltip("The main camera transform that the grenade follows")]
    private Transform cam;
    [Header("Throw Settings")]
    [SerializeField, Tooltip("How far forward the grenade will be thrown")]
    private float throwForce = 12f;
    [SerializeField, Tooltip("How far upward the grenade will be thrown")]
    private float upwardForce = 4f;
    [SerializeField, Tooltip("How far forward the grenade will spawn relative to the camera")]
    private float spawnOffsetForward = 1.2f;
    [SerializeField, Tooltip("How far above the grenade will spawn relative to the camera")]
    private float spawnOffsetUp = 0f;

    private void Start()
    {
        //Deactivate visual if possible
        if (weaponVisual != null)
        {
            weaponVisual.SetActive(false);
        }
    }

    public override void Attack()
    {
        Vector3 spawnPos = cam.position + (cam.forward * spawnOffsetForward) + (cam.up * spawnOffsetUp);
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
}