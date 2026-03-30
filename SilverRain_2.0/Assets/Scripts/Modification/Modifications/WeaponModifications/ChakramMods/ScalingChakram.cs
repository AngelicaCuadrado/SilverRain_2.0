using UnityEngine;

public class ScalingChakram : Modification
{
    [SerializeField, Tooltip("The percentage increase in damage per hit")]
    //private float damageIncreasePerHit = 0.05f; // 5% increase per hit
    private float damageIncreasePerHit = 5f; // 5 flat increase per hit

    public override void Activate()
    {
        base.Activate();
        WeaponManager.Instance.OnWeaponHit.AddListener(OnProjectileHit);
    }

    private void OnProjectileHit(WeaponType type, GameObject[] hitObjects, Vector3 hitPoint, Projectile proj)
    {
        if (type != WeaponType.Chakram) return;

        ChakramProjectile chakramProj = proj as ChakramProjectile;
        if (chakramProj == null) return;

        // Increase damage
        //chakramProj.Damage *= 1 + damageIncreasePerHit;       // Percentage increase
        chakramProj.Damage += damageIncreasePerHit;             // Flat increase
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WeaponManager.Instance.OnWeaponHit.RemoveListener(OnProjectileHit);
    }
}