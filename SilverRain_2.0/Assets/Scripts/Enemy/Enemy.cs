using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    EnemyHealth health;
    EnemyController controller;
    [SerializeField] private int scoreValue;
    [SerializeField] private float xpValue;
    [SerializeField] public float damage;
    //[SerializeField] private float goldValue;
    private Renderer[] renderers;

    // ObjectPool References
    private ObjectPooler pooler;
    public string PoolKey { get; set; }

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        controller = GetComponent<EnemyController>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Hide();
        if (GlobalInvisibilityManager.Instance.isActive)
        {
            float remaining = GlobalInvisibilityManager.Instance.invisibilityTimer;
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
    }

    public void OnReturnToPool()
    {
        StopAllCoroutines();
    }
    
    #endregion

    public void SetPooler(ObjectPooler pooler)
    {
        this.pooler = pooler;
    }

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
        EnemyEvents.OnGlobalReveal += RevealTimed;
    }

    private void OnDisable()
    {
        //Unsubscribe to reveal all event
        EnemyEvents.OnGlobalReveal -= RevealTimed;
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
}
