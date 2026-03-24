using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    [Header("References")]
    [SerializeField, Tooltip("")]
    private EnemyHealth health;
    [SerializeField, Tooltip("")]
    private EnemyController controller;
    [SerializeField] private bool startHidden = true;

    [Header("Rewards")]
    [SerializeField, Tooltip("")]
    private int scoreValue;
    [SerializeField, Tooltip("")]
    private float xpValue;

    [Header("Combat")]
    [SerializeField, Tooltip("")]
    private float damage;

    [Header("Pooling")]
    [SerializeField, Tooltip("")]
    private string poolKey;

    [Header("Stage VFX")]
    [SerializeField, Tooltip("")]
    private ParticleSystem stageParticlePrefab;
    [SerializeField, Tooltip("")]
    private ParticleSystem _stageParticleInstance;
    [SerializeField, Tooltip("")]
    private MaterialPropertyBlock _mpb;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private Renderer[] renderers;

    // Properties
    public string PoolKey { get => poolKey; set => poolKey = value; }
    public float Damage { get => damage; }
    public int ScoreValue { get => scoreValue; }
    public float XPValue { get => xpValue; }

    private void Awake()
    {
        if (health == null) health = GetComponent<EnemyHealth>();
        if (controller == null) controller = GetComponent<EnemyController>();
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
        if (health != null) { health.ResetHealth(); }
        Initialize();
        if (StageManager.Instance != null)
            ApplyStageVFX(StageManager.Instance.CurrentStage);
    }

    public void OnReturnToPool()
    {
        StopAllCoroutines();
    }
    #endregion

    private void OnEnable()
    {
        //Subscribe to events
        GlobalInvisibilityManager.Instance.OnGlobalReveal.AddListener(RevealTimed);
        // if (StageManager.Instance != null)
        //     StageManager.Instance.OnStageChanged.AddListener(ApplyStageVFX);
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        GlobalInvisibilityManager.Instance.OnGlobalReveal.RemoveListener(RevealTimed);
        // if (StageManager.Instance != null)
        //     StageManager.Instance.OnStageChanged.RemoveListener(ApplyStageVFX);
    }

    public void Reveal()
    {
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
        StopAllCoroutines();
        StartCoroutine(RevealCorutine(seconds));
    }

    private IEnumerator RevealCorutine(float duration)
    {
        Reveal();
        yield return new WaitForSeconds(duration);
        Hide();
    }

    private void ApplyStageVFX(int stage)
    {
        if (StageManager.Instance == null) return;
    
        EnemyEdgeGlow glow = GetComponent<EnemyEdgeGlow>();
        if (glow != null)
            glow.ApplyBuffVisual();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Collider c = GetComponent<Collider>();
        Gizmos.DrawWireSphere(c.bounds.center, c.bounds.extents.x);
    }
}