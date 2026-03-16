using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    EnemyHealth health;
    EnemyController controller;
    [SerializeField] private int scoreValue;
    [SerializeField] private float xpValue;
    [SerializeField] public float damage;
    [SerializeField] private bool startHidden = true;
    //[SerializeField] private float goldValue;
    private Renderer[] renderers;

    // ObjectPool References
    public ObjectPooler pooler;
    public string PoolKey { get; set; }
    
    // Stage VFX
    [Header("Stage VFX")]
    [SerializeField] private ParticleSystem stageParticlePrefab;
    private ParticleSystem _stageParticleInstance;
    private MaterialPropertyBlock _mpb;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        controller = GetComponent<EnemyController>();
        renderers = GetComponentsInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();

        if (stageParticlePrefab != null)
        {
            _stageParticleInstance = Instantiate(stageParticlePrefab, transform);
            _stageParticleInstance.Stop();
        }
    }

    private void Start()
    {
        Initialize();
        if (StageManager.Instance != null)
            ApplyStageVFX(StageManager.Instance.CurrentStage);
    }

    private void Initialize()
    {
        if (startHidden)
        {
            Hide();
        }
        else
        {
            Reveal();
        }

        if (GlobalInvisibilityManager.Instance.IsActive)
        {
            float remaining = GlobalInvisibilityManager.Instance.InvisibilityTimer;
            RevealTimed(remaining);
        }
    }
    
    #region IPoolable Implementation

    public void OnCreatedPool()
    {
    }

    public void OnSpawnFromPool()
    {
        health?.ResetHealth();
        Initialize();
        if (StageManager.Instance != null)
            ApplyStageVFX(StageManager.Instance.CurrentStage);
    }

    public void OnReturnToPool()
    {
        StopAllCoroutines();
    }
    
    #endregion

    public void ReturnToPool()
    {
        if (pooler != null)
            pooler.ReturnToPool(gameObject, PoolKey);
        else
            Destroy(gameObject);
    }
    
    private void OnEnable()
    {
        //Subscribe to reveal all event
        GlobalInvisibilityManager.Instance.OnGlobalReveal.AddListener(RevealTimed);
        if (StageManager.Instance != null)
            StageManager.Instance.OnStageChanged.AddListener(ApplyStageVFX);
    }

    private void OnDisable()
    {
        //Unsubscribe to reveal all event
        GlobalInvisibilityManager.Instance.OnGlobalReveal.RemoveListener(RevealTimed);
        if (StageManager.Instance != null)
            StageManager.Instance.OnStageChanged.RemoveListener(ApplyStageVFX);
    }

    private void Update()
    {
        //Trigger reveal when reveal all event is called
        //health.DamageTest();
    }
    public void Reveal()
    {
        //Debug.Log("Enemy is Revealing");
        foreach (var r in renderers) 
        { 
            r.enabled = true;
        }
    }

    public void Hide()
    {
        foreach (var r in renderers)
        {
            r.enabled = false;
        }
    }

    public void RevealTimed(float seconds) 
    {
        //Debug.Log("Timed Reveal Start");
        StopAllCoroutines();
        StartCoroutine(RevealCorutine(seconds));
    }

    // private void Start()
    // {
    //     health = GetComponent<EnemyHealth>();
    //     controller = GetComponent<EnemyController>();
    //     renderers = GetComponentsInChildren<Renderer>();
    //
    //     Hide();
    //
    //     if (GlobalInvisibilityManager.Instance.isActive)
    //     {
    //         float remaining = GlobalInvisibilityManager.Instance.invisibilityTimer;
    //         RevealTimed(remaining);
    //     }
    // }

    private IEnumerator RevealCorutine(float duration) 
    {
        Reveal();
        yield return new WaitForSeconds(duration);
        Hide();
    }

    public float RewardXP() 
    {
        return xpValue;
    }
    public int RewardScore()
    {
        return scoreValue;
    }

    private void ApplyStageVFX(int stage)
    {
        if (StageManager.Instance == null) return;

        Color emissionColor = StageManager.Instance.GetStageEmissionColor();
        _mpb.SetColor(EmissionColorID, emissionColor);
        foreach (var r in renderers)
        {
            r.SetPropertyBlock(_mpb);
        }

        if (_stageParticleInstance != null)
        {
            float rate = StageManager.Instance.GetStageParticleRate();
            var emission =  _stageParticleInstance.emission;
            emission.rateOverTime = rate;

            if (rate > 0f)
            {
                if (!_stageParticleInstance.isPlaying) _stageParticleInstance.Play();
            }
            else
            {
                _stageParticleInstance.Stop();
            }
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Collider c = GetComponent<Collider>();
        Gizmos.DrawWireSphere(c.bounds.center, c.bounds.extents.x);
    }
    #endif
}
