using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    [Header("References")]
    [SerializeField, Tooltip("The EnemyHealth component attached to this GameObject")]
    private EnemyHealth health;
    [SerializeField, Tooltip("The EnemyController component attached to this GameObject")]
    private EnemyController controller;
    [SerializeField, Tooltip("Whether the enemy should start hidden")]
    private bool startHidden = true;

    [Header("Rewards")]
    [SerializeField, Tooltip("The score value awarded to the player for defeating this enemy")]
    private int scoreValue;
    [SerializeField, Tooltip("The experience points awarded to the player for defeating this enemy")]
    private float xpValue;

    [Header("Combat")]
    [SerializeField, Tooltip("The damage value dealt by the enemy")]
    private float damage;

    [Header("Pooling")]
    [SerializeField, Tooltip("The key used to identify this enemy in the object pool")]
    private string poolKey;

    [Header("Stage VFX")]
    [SerializeField, Tooltip("The particle system prefab for the stage VFX")]
    private ParticleSystem stageParticlePrefab;
    [SerializeField, Tooltip("The instance of the stage particle system")]
    private ParticleSystem _stageParticleInstance;
    [SerializeField, Tooltip("The material property block used for stage VFX")]
    private MaterialPropertyBlock _mpb;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private Renderer[] renderers;

    // Properties
    public string PoolKey { get => poolKey; set => poolKey = value; }
    public ObjectPooler PoolOwner { get; set; }
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
        if (GlobalInvisibilityManager.Instance != null) GlobalInvisibilityManager.Instance.OnGlobalReveal.AddListener(RevealTimed);
        // if (StageManager.Instance != null)
        //     StageManager.Instance.OnStageChanged.AddListener(ApplyStageVFX);
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        if (GlobalInvisibilityManager.Instance != null) GlobalInvisibilityManager.Instance.OnGlobalReveal.RemoveListener(RevealTimed);
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