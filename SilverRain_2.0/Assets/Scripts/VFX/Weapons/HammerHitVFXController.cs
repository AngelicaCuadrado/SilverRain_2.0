using System.Collections;
using UnityEngine;

public class HammerHitVFXController : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("The sparks particle system")]
    private ParticleSystem sparks;
    [SerializeField, Tooltip("The smoke particle system")]
    private ParticleSystem smoke;
    [SerializeField, Tooltip("The scale factor for the VFX size")]
    private float sizeScaler = 0f;
    [SerializeField, Tooltip("The duration of the VFX, set to the max duration of the particle systems")]
    private float duration;

    public string PoolKey { get; set; }
    public ObjectPooler PoolOwner { get; set; }

    public void OnCreatedPool() { }

    public void OnReturnToPool() { }

    public void OnSpawnFromPool() { }

    public void Init(string poolKey, float size)
    {
        if (sparks == null) { Debug.LogWarning("HammerHitVFXController is missing sparks ParticleSystem reference"); return; }
        if (smoke == null) { Debug.LogWarning("HammerHitVFXController is missing smoke ParticleSystem reference"); return; }

        PoolKey = poolKey;

        // Sparks
        var sparksShape = sparks.shape;
        sparksShape.radius = size * sizeScaler;
        sparks.Play();

        // Smoke
        var smokeMain = smoke.main;
        smokeMain.startSize = new(size * sizeScaler);
        smoke.Play();

        // Duration is the max of the two particle systems' durations + max start lifetimes
        var sparksDuration = sparks.main.duration + sparks.main.startLifetime.constantMax;
        var smokeDuration = smoke.main.duration + smoke.main.startLifetime.constantMax;
        duration = Mathf.Max(sparksDuration, smokeDuration);
        StartCoroutine(ReturnWhenDone());
    }
    private IEnumerator ReturnWhenDone()
    {
        yield return new WaitForSeconds(duration);

        // Use the PoolOwner to return to the correct pool
        PoolOwner.ReturnToPool(gameObject, PoolKey);
    }
}