using System.Collections;
using UnityEngine;

public class GrenadeExplosionVFX : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("")]
    private ParticleSystem ps;

    public string PoolKey { get; set; }

    public void OnCreatedPool() { }

    public void OnReturnToPool() { }

    public void OnSpawnFromPool() { }

    public void Init(string poolKey, float size)
    {
        PoolKey = poolKey;
        if (ps ==  null) { ps = GetComponent<ParticleSystem>(); }
        // Scale the explosion VFX
        var main = ps.main;
        // Ido Isaac - I multiplied by 3 to offset the animation we have being small
        main.startSize = new ParticleSystem.MinMaxCurve(size * 3);
        ps.Play();
        StartCoroutine(ReturnWhenDone());
    }
    private IEnumerator ReturnWhenDone()
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);
        WeaponManager.Instance.EffectsPool.ReturnToPool(gameObject, PoolKey);
    }
}
