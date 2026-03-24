using UnityEngine;

public class BeamSword : Modification
{
    [SerializeField, Tooltip("")]
    private string beamPoolKey;
    [SerializeField, Tooltip("")]
    private float forwardOffset;

    public override void Start()
    {
        base.Start();
        WeaponManager.Instance.OnWeaponAquired.AddListener(OnRequirementMet);

        // Check initial weapon
        if (WeaponManager.Instance.InitialWeapon == WeaponType.Sword)
        {
            OnRequirementMet(WeaponType.Sword);
        }
    }

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponProjectileSpawn.AddListener(OnProjectileSpawn);
    }

    private void OnProjectileSpawn(WeaponType type, Weapon weapon, GameObject projObj)
    {
        if (type != WeaponType.Sword) return;

        // Spawn beam as child of the sword projectile
        //var beam = Instantiate(beamPrefab, projObj.transform);

        var beam = WeaponManager.Instance.ProjectilePool.Spawn(
            beamPoolKey,
            new Vector3(projObj.transform.position.x, forwardOffset, projObj.transform.position.y),
            projObj.transform.rotation
        );
        

        // Position at sword tip (local offset)
        beam.transform.localPosition = new Vector3(0f, 0f, 1f); // adjust as needed

        // Initialize beam controller
        var controller = beam.GetComponent<BeamController>();
        controller.Init(
            weapon.WeaponStats.Damage,
            weapon.WeaponStats.Duration,
            weapon.WeaponStats.Cooldown,
            weapon.WeaponStats.Size
        );
    }

    public void OnRequirementMet(WeaponType type)
    {
        if (type == WeaponType.Sword)
        {
            SetAvailable(true);
            WeaponManager.Instance.OnWeaponAquired.RemoveListener(OnRequirementMet);
        }
    }

    private void OnDestroy()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnWeaponAquired.RemoveListener(OnRequirementMet);
            WeaponManager.Instance.OnWeaponProjectileSpawn.RemoveListener(OnProjectileSpawn);
        }
    }
}