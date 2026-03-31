using System.Collections;
using UnityEngine;

public class GrenadeExplosionVFX : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("")]
    private ParticleSystem ps;
    [SerializeField, Tooltip("")]
    private float sizeScaler = 3f;

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
        // Ido Isaac - I multiplied by 3 to offset the animation we have being small
        main.startSize = new ParticleSystem.MinMaxCurve(size * sizeScaler);
        ps.Play();
        StartCoroutine(ReturnWhenDone());
    }
    private IEnumerator ReturnWhenDone()
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);

        // Use the PoolOwner to return to the correct pool
        PoolOwner.ReturnToPool(gameObject, PoolKey);
        yield break;
    }
}