using System.Collections;
using UnityEngine;

public class ExplosionVFXController : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("The particle system for the explosion VFX")]
    private ParticleSystem ps;
    [SerializeField, Tooltip("The scale factor for the explosion size")]
    private float sizeScaler = 0f;

    public string PoolKey { get; set; }
    public ObjectPooler PoolOwner { get; set; }

    public void OnCreatedPool() { }

    public void OnReturnToPool() { }

    public void OnSpawnFromPool() { }

    public void Init(string poolKey, float size)
    {
        PoolKey = poolKey;
        if (ps == null) { ps = GetComponent<ParticleSystem>(); }
        // Scale the explosion VFX
        var main = ps.main;
        main.startSize = new(size * sizeScaler);
        ps.Play();
        StartCoroutine(ReturnWhenDone());
    }
    private IEnumerator ReturnWhenDone()
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);

        // Use the PoolOwner to return to the correct pool
        PoolOwner.ReturnToPool(gameObject, PoolKey);
    }
}