using UnityEngine;

public abstract class Pickup : MonoBehaviour, IPoolable
{
    [Header("Identification")]
    [SerializeField, Tooltip("The key used to identify this pickup in the object pool")]
    protected string poolKey;
    [SerializeField, Tooltip("The index of the spawn location for this pickup (used for tracking in the PickupManager)")]
    protected int locationIndex;
    [Space]

    [Header("Visual Effect Settings")]
    [SerializeField, Tooltip("The speed at which the pickup bounces up and down for visual effect")]
    protected float bounceSpeed = 2f;
    [SerializeField, Tooltip("The minimum height for the pickup's bounce effect")]
    protected float minHeight = 0.8f;
    [SerializeField, Tooltip("The maximum height for the pickup's bounce effect")]
    protected float maxHeight = 1.2f;
    [SerializeField, Tooltip("The speed at which the pickup spins and bounces for visual effect")]
    protected float spinSpeed = 150f;

    public string PoolKey { get { return poolKey; } set { poolKey = value; } }
    public int LocationIndex { get { return locationIndex; } set { locationIndex = value; } }

    public virtual void Update()
    {
        // Rotate the pickup for visual effect
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        // Bounce the pickup up and down for visual effect using lerp between min and max height based on a sine wave
        float newY = Mathf.Lerp(minHeight, maxHeight, (Mathf.Sin(Time.time * bounceSpeed) + 1f) / 2f);
        Vector3 newPosition = transform.position;
        newPosition.y = newY;
        transform.position = newPosition;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup();
            PickupManager.Instance.PickupPool.ReturnToPool(gameObject, poolKey);
        }
    }

    public virtual void OnPickup()
    {
        PickupManager.Instance.ClearSpawnSpot(locationIndex);
    }

    public virtual void OnCreatedPool() { }

    public virtual void OnReturnToPool() { }

    public virtual void OnSpawnFromPool() { }
}