using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IPoolable
{
    Vector3 direction;
    float damage;
    float speed = 10f;
    PlayerHealth targetPlayerHealth;
    float deathTime = 10f;
    
    public ObjectPooler pooler;
    public string PoolKey { get; set; }
    
    #region IPoolable Implementation
    public void OnCreatedPool()
    {
    }

    public void OnSpawnFromPool()
    {
        if (deathTime > 0f)
        {
            StartCoroutine(LifeTimer());
        }
    }

    public void OnReturnToPool()
    {
    }
    
    #endregion

    public void ReturnToPool(GameObject obj)
    {
        if (pooler != null)
            pooler.ReturnToPool(obj, PoolKey);
        else
            Destroy(obj);
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
        //Destroy(gameObject);
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
