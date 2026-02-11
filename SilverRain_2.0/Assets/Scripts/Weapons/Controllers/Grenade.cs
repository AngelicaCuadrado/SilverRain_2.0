using System.Collections;
using UnityEngine;

public class Grenade : Weapon
{
    [Header("Throw Settings")]
    [SerializeField, Tooltip("How far forward the grenade will be thrown")]
    private float throwForce = 12f;
    [SerializeField, Tooltip("How far upward the grenade will be thrown")]
    private float upwardForce = 4f;
    [Space]

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

    #region TemporaryBuff Implementation
    public override void LevelUp()
    {
        base.LevelUp();
        // Recalculate stats
        weaponStats.CalculateStat(StatType.AttackDamage);
        weaponStats.CalculateStat(StatType.Cooldown);
        weaponStats.CalculateStat(StatType.Size);
    }

    public override void UpdateDescription()
    {
        uiData.UpdateDescription(level, maxLevel,
            "Damage", weaponStats.GetCurrentStatsForUI(StatType.AttackDamage), weaponStats.GetNextLevelStatsForUI(StatType.AttackDamage),
            "Cooldown", weaponStats.GetCurrentStatsForUI(StatType.Cooldown), weaponStats.GetNextLevelStatsForUI(StatType.Cooldown),
            "Size", weaponStats.GetCurrentStatsForUI(StatType.Size), weaponStats.GetNextLevelStatsForUI(StatType.Size));
    }
    #endregion

    #region Weapon Implementation
    public override IEnumerator OnDuration()
    {
        Attack();
        StartCoroutine(OnCooldown());
        yield break;
    }
    public override void Attack()
    {
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, firePoint.position, cam.rotation);

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
    #endregion
}