using System.Collections;
using UnityEngine;
/// <summary>
/// Represents a base class for projectiles fired by weapons, providing common functionality for projectile behavior and
/// lifecycle management.
/// </summary>
/// <remarks>This abstract class is intended to be inherited by specific projectile types to implement custom
/// behavior, such as movement, collision handling, and lifetime control. The class maintains references to the weapon
/// that fired the projectile and its damage value, and provides a mechanism for managing the projectile's active
/// duration. Inheriting classes must implement the <see cref="LifeTimer"/> coroutine to define how the projectile's
/// lifetime is handled.</remarks>
public abstract class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("The weapon that fired this projectile")]
    protected Weapon parentWeapon;
    [SerializeField, Tooltip("How much damage this projectile will deal")]
    protected float damage;
    [Header("Lifetime Settings")]
    [SerializeField, Tooltip("How long before the projectile deactivates itself")]
    protected float lifeTime = 5f;
    [SerializeField, Tooltip("The coroutine instance that will deactivate the projectile")]
    protected Coroutine lifeCoroutine;

    public string PoolKey { get; set; }
    public abstract void OnCreatedPool();
    public abstract void OnReturnToPool();
    public abstract void OnSpawnFromPool();
    public IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        if (gameObject.activeInHierarchy)
        {
            WeaponManager.Instance.ProjectilePool.ReturnToPool(gameObject, PoolKey);
        }
    }
}
