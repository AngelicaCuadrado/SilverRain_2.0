using UnityEngine;

public class BeamSword : Modification
{
    [SerializeField, Tooltip("The key used to access the pool containing the beam VFX")]
    private string beamPoolKey;
    [SerializeField, Tooltip("The offset of the beam from the sword's forward direction")]
    private float forwardOffset;

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponProjectileSpawn.AddListener(OnProjectileSpawn);
    }

    private void OnProjectileSpawn(WeaponType type, Weapon weapon, GameObject projObj)
    {
        if (type != WeaponType.Sword) return;

        // Spawn beam from pool
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

    public override void OnDestroy()
    {   
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponProjectileSpawn.RemoveListener(OnProjectileSpawn);       
    }
}