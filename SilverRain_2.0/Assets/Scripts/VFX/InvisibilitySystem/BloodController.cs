using System.Collections;
using UnityEngine;

public class BloodController : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("How long the blood splatter should last before being returned to the pool")]
    private float lifetime = 5f;
    [SerializeField, Tooltip("The key used to identify this blood splatter in the pool")]
    private string poolKey;
    [SerializeField, Tooltip("The pool owner that manages this blood splatter")]
    private ObjectPooler poolOwner;

    public string PoolKey { get => poolKey; set => poolKey = value; }
    public ObjectPooler PoolOwner { get => poolOwner; set => poolOwner = value; }

    public void OnCreatedPool() { }

    public void OnSpawnFromPool()
    {
        StartCoroutine(LifetimeCoroutine());
    }

    public void OnReturnToPool()
    {
        StopAllCoroutines();
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifetime);
        GlobalInvisibilityManager.Instance.BloodSplatterPool.ReturnToPool(gameObject, poolKey);
    }
}