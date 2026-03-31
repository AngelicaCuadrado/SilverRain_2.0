using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IPoolable
{
    [Header("Movement")]
    [SerializeField, Tooltip("")]
    private Vector3 direction;
    [SerializeField, Tooltip("")]
    private float speed = 10f;

    [Header("Damage")]
    [SerializeField, Tooltip("")]
    private float damage;
    [SerializeField, Tooltip("")]
    private PlayerHealth targetPlayerHealth;

    [Header("Pooling")]
    [SerializeField, Tooltip("")]
    private float deathTime = 10f;
    [SerializeField, Tooltip("")]
    private string poolKey;

    public string PoolKey { get => poolKey; set => poolKey = value; }
    public ObjectPooler PoolOwner { get; set; }

    #region IPoolable Implementation
    public void OnCreatedPool() { }

    public void OnSpawnFromPool()
    {
        if (deathTime > 0f)
        {
            StartCoroutine(LifeTimer());
        }
    }

    public void OnReturnToPool()
    {
        if (deathTime > 0f)
        {
            StopCoroutine(LifeTimer());
        }
    }
    #endregion

    public void ReturnToPool(GameObject obj)
    {
        PoolOwner.ReturnToPool(obj, PoolKey);
    }

    public void Initialize(Vector3 direction, float damage, PlayerHealth target)
    {
        this.direction = direction.normalized;
        this.damage = damage;
        targetPlayerHealth = target;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPlayerHealth.TakeDamage(damage);
        }
        ReturnToPool(gameObject);
    }

    public IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(deathTime);
        if (gameObject.activeInHierarchy)
        {
            ReturnToPool(gameObject);
        }
    }
}