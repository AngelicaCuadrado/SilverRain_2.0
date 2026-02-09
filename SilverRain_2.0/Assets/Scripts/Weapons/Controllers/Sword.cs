using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Projectile Settings")]
    [SerializeField, Tooltip("The center of the sword's rotation")]
    private Transform playerTrans;
    [SerializeField, Tooltip("The rotation which the projectile will spawn in relative to the camera")]
    private float spawnAngleOffset = 90f;
    [Header("Spawn Position Offsets")]
    [SerializeField, Tooltip("How far forward the sword is positioned relative to the camera")]
    private float spawnOffsetForward = 1f;
    [SerializeField, Tooltip("How far above the sword is positioned relative to the camera")]
    private float spawnOffsetUp = -0.4f;
    [SerializeField, Tooltip("How far to the side the sword is positioned relative to the camera")]
    private float spawnOffsetSide = 0f;

    private void Update()
    {
        if (cam == null) return;
        //Make the pistol follow the camera's position and rotation
        Vector3 desiredPos = cam.position + (cam.forward * spawnOffsetForward) + (cam.up * spawnOffsetUp) + (cam.right * spawnOffsetSide);
        transform.position = desiredPos;
        transform.rotation = Quaternion.LookRotation(-cam.forward, Vector3.up);
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
        //Cache the player transform
        playerTrans = PlayerFinder.Instance.Player.transform;
        if (playerTrans == null)
        {
            Debug.LogError("Sword: Could not find player transform.");
            return;
        }
        //Cache the main camera transform
        if (Camera.main != null) { cam = Camera.main.transform; }
        else { Debug.LogWarning("Sword: no Camera.main found. Ensure a camera has the MainCamera tag."); }
        base.OnActivate();
    }

    public override IEnumerator OnDuration()
    {
        Attack();
        yield return new WaitForSeconds(weaponStats.Duration);
        StartCoroutine(OnCooldown());
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
        proj.Init(this, playerTrans, weaponStats.Damage, weaponStats.Duration, weaponStats.Size, weaponStats.ProjectileSpeed);
    }

    public override void UpdateDescription()
    {
        uiData.UpdateDescription(weaponLevel, 
            "Damage", weaponStats.GetCurrentStatsForUI(StatType.AttackDamage), weaponStats.GetNextLevelStatsForUI(StatType.AttackDamage),
            "Cooldown", weaponStats.GetCurrentStatsForUI(StatType.Cooldown), weaponStats.GetNextLevelStatsForUI(StatType.Cooldown),
            "Duration", weaponStats.GetCurrentStatsForUI(StatType.Duration), weaponStats.GetNextLevelStatsForUI(StatType.Duration),
            "Speed", weaponStats.GetCurrentStatsForUI(StatType.ProjectileSpeed), weaponStats.GetNextLevelStatsForUI(StatType.ProjectileSpeed),
            "Size", weaponStats.GetCurrentStatsForUI(StatType.Size), weaponStats.GetNextLevelStatsForUI(StatType.Size));
    }
}