using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public ObjectPooler enemyPooler;
    public ObjectPooler enemyProjectilePool;

    private int enemiesDefeated;

    private void Awake()
    {
        Instance = this;
    }
}
