using System.Collections;
using UnityEngine;
/// <summary>
/// Represents a base class for projectiles fired by weapons, providing common functionality for projectile behavior and
/// lifecycle management.
/// </summary>
/// <remarks>This abstract class is intended to be inherited by specific projectile types to implement custom
/// behavior, such as movement, collision handling, and lifetime control. The class maintains references to the weapon
/// that fired the projectile and its damage value, and provides a mechanism for managing the projectile's active
/// duration. Inheriting classes may override the pool lifecycle hooks but should call base implementations when appropriate
/// so shared cleanup (like clearing SpawnMetadata) runs consistently.</remarks>
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

    // Properties
    public Weapon ParentWeapon => parentWeapon;
    public string PoolKey { get; set; }
    public ObjectPooler PoolOwner { get; set; }
    public float Damage { get => damage; set => damage = value; }

    // Called once when the pool initially creates the instance
    public virtual void OnCreatedPool() { }

    // Called whenever the pool spawns this instance
    public virtual void OnSpawnFromPool() { }

    // Called before the pool deactivates this instance
    // Performs shared cleanup: stops lifetime coroutine and clears SpawnMetadata so pooled instances don't carry stale state.
    public virtual void OnReturnToPool()
    {
        // Stop any running lifetime coroutine
        if (lifeCoroutine != null)
        {
            try
            {
                StopCoroutine(lifeCoroutine);
            }
            catch { }
            lifeCoroutine = null;
        }

        // Reset spawn metadata if present so pooled objects are clean when reused
        var meta = GetComponent<SpawnMetadata>();
        if (meta != null)
        {
            meta.Generation = 0;
            meta.ProcessedModIds?.Clear();
        }
    }

    public IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        if (gameObject.activeInHierarchy)
        {
            PoolOwner.ReturnToPool(gameObject, PoolKey);
        }
    }
}
