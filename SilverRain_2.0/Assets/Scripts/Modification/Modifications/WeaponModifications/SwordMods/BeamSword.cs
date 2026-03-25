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

        var beam = ModificationManager.Instance.PrefabPool.Spawn(
            beamPoolKey,
            projObj.transform.position,
            projObj.transform.rotation
        );
        
        // Set projectile as parent
        beam.transform.SetParent(projObj.transform, worldPositionStays: true);

        
        // Position at sword tip and rotate beam to point forward
        beam.transform.SetLocalPositionAndRotation(new Vector3(0f, forwardOffset, 0f), Quaternion.Euler(-90f, 0f, 0f));

        // Initialize beam controller
        var controller = beam.GetComponent<BeamController>();
        controller.Init(
            weapon.WeaponStats.Damage,
            Mathf.Abs(weapon.WeaponStats.Duration)/6,
            weapon.WeaponStats.Cooldown/6,
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