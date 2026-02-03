/// <summary>
/// Defines methods that allow an object to participate in an object pool lifecycle.
/// </summary>
/// <remarks>Implement this interface to receive notifications when an object is created, retrieved from the pool,
/// or returned to the pool. This enables custom initialization, cleanup, or state management for pooled
/// objects.</remarks>
public interface IPoolable
{
    //Called once when the object is first created
    void OnCreatedPool();

    //Called whenever the pool hands out this instance
    void OnSpawnFromPool();

    //Called when returning to pool
    void OnReturnToPool();
}
