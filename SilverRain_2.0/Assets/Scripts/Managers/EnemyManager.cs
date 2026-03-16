using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("Pooling")]
    [SerializeField, Tooltip("")]
    private ObjectPooler enemyPool;
    [SerializeField, Tooltip("")]
    private ObjectPooler enemyProjectilePool;
    [SerializeField, Tooltip("")]
    private ObjectPooler enemyEffectsPool;

    [Header("Enemy Count")]
    [SerializeField, Tooltip("")]
    private int enemiesDefeated;

    // Properties
    public ObjectPooler EnemyPool { get => enemyPool; }
    public ObjectPooler EnemyProjectilePool { get => enemyProjectilePool; }
    public ObjectPooler EnemyEffectsPool { get => enemyEffectsPool; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
