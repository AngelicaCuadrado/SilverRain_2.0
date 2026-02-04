using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    [SerializeField, Tooltip("The position from which bullets will spawn")]
    private Transform playerTrans;
    [SerializeField, Tooltip("The rotation which the projectile will spawn in relative to the camera")]
    private float spawnAngleOffset = 90f;

    private void Start()
    {
        //Deactivate visual if possible
        if (weaponVisual != null)
        {
            weaponVisual.enabled = false;
        }
    }
    public override void Attack()
    {
        //Calculate spawn rotation so the projectile faces forward
        Quaternion spawnRot = Quaternion.Euler(0f, playerTrans.eulerAngles.y + spawnAngleOffset, 0f);
        //Instantiate projectile
        var projObj = WeaponManager.Instance.ProjectilePool.Spawn(projectilePoolKey, playerTrans.position, spawnRot);
        //Initialize projectile
        var proj = projObj.GetComponent<SwordProjectile>();
        if (proj == null)
        {
            Debug.LogWarning("Sword: The instantiated projectile does not have a SwordProjectile component.");
            Destroy(projObj);
            return;
        }
        proj.Init(this, playerTrans, weaponStats.Damage, weaponStats.Duration, weaponStats.Size);
    }

    public override void LevelUp()
    {
        //Ensure we don't exceed max level
        if (weaponLevel >= maxWeaponLevel) return;
        //Increase weapon level and recalculate stats
        weaponLevel++;
        weaponStats.CalculateStat(StatType.AttackDamage);
        weaponStats.CalculateStat(StatType.Cooldown);
        weaponStats.CalculateStat(StatType.Duration);
        weaponStats.CalculateStat(StatType.ProjectileSpeed);
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
        playerTrans = PlayerFinder.Instance.Player.transform;
        if (playerTrans == null)
        {
            Debug.LogError("Sword: Could not find player transform.");
            return;
        }
        base.OnActivate();
    }

    public override IEnumerator OnDuration()
    {
        Attack();
        yield return new WaitForSeconds(weaponStats.Duration);
        StartCoroutine(OnCooldown());
    }
}
