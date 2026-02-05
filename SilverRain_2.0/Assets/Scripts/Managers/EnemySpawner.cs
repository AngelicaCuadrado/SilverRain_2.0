using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public string playerTag = "Player";

    [Header("Spawn area")]
    public GameObject spawnArea;

    [Header("Distance constraints")]
    public float minDistance = 5f;
    public float maxDistance = 30f;

    [Header("Spawn timing")]
    public float spawnDelay = 1.0f;
    public int maxAttemptsPerSpawn = 20;

    // [Header("Enemy prefabs")]
    // public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Collision checks")]
    public float spawnCheckRadius = 0.5f;
    public LayerMask blockingLayers = ~0;

    [Header("Optional")]
    public Transform spawnParent;
    
    [Header("Object Pooling")]
    public ObjectPooler objectPooler;
    public List<string> enemyPoolKeys = new List<string>();
    
    bool _spawning = false;
    Bounds _areaBounds;
    bool _haveAreaBounds = false;

    void Start()
    {
        //Find playerTrans if not assigned
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p;
        }

        UpdateSpawnAreaBounds();

        if (spawnParent == null) spawnParent = transform;
        
        StartSpawning();
    }

    void OnValidate()
    {
        // keep sensible values in inspector
        minDistance = Mathf.Max(0f, minDistance);
        maxDistance = Mathf.Max(minDistance + 0.01f, maxDistance);
        spawnDelay = Mathf.Max(0.01f, spawnDelay);
        maxAttemptsPerSpawn = Mathf.Max(1, maxAttemptsPerSpawn);
        spawnCheckRadius = Mathf.Max(0.01f, spawnCheckRadius);
    }

    public void UpdateSpawnAreaBounds()
    {
        _haveAreaBounds = false;
        if (spawnArea == null) return;

        // Try renderers first
        var rends = spawnArea.GetComponentsInChildren<Renderer>();
        if (rends != null && rends.Length > 0)
        {
            _areaBounds = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) _areaBounds.Encapsulate(rends[i].bounds);
            _haveAreaBounds = true;
            return;
        }

        // Fallback to colliders
        var cols = spawnArea.GetComponentsInChildren<Collider>();
        if (cols != null && cols.Length > 0)
        {
            _areaBounds = cols[0].bounds;
            for (int i = 1; i < cols.Length; i++) _areaBounds.Encapsulate(cols[i].bounds);
            _haveAreaBounds = true;
            return;
        }
    }

    public void StartSpawning()
    {
        if (_spawning) return;
        _spawning = true;
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (!_spawning) return;
        _spawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnLoop()
    {
        while (_spawning)
        {
            yield return new WaitForSeconds(spawnDelay);
            TrySpawnOnce();
        }
    }

    void TrySpawnOnce()
    {
        //Check that we have enemies to spawn
        // if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        // {
        //     Debug.LogWarning("EnemySpawner: No enemy prefabs assigned.");
        //     return;
        // }

        //Ensure we have a playerTrans reference
        if (player == null)
        {
            var p = GameManager.Instance.Player;
            if (p != null) player = p;
        }

        if (spawnArea != null && !_haveAreaBounds) UpdateSpawnAreaBounds();

        for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
        {
            Vector3 candidate = SamplePositionAroundPlayer();

            if (!IsInsideSpawnArea(candidate)) continue;
            if (!IsDistanceValid(candidate)) continue;
            if (IsOverlappingBlocking(candidate)) continue;

            SpawnEnemyAt(candidate);
            return;
        }

        // Failed to find a valid position this cycle; skip.
    }

    Vector3 SamplePositionAroundPlayer()
    {
        // Sample a point in an annulus around the playerTrans in XZ plane
        float r = Random.Range(minDistance, maxDistance);
        float ang = Random.Range(0f, Mathf.PI * 2f);
        Vector3 offset = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * r;
        Vector3 basePos = player != null ? player.transform.position : transform.position;
        Vector3 candidate = basePos + offset;

        // Keep candidate Y inside spawnArea vertical range if possible
        if (_haveAreaBounds)
        {
            candidate.y = Mathf.Clamp(candidate.y, _areaBounds.min.y, _areaBounds.max.y);
        }

        return candidate;
    }

    bool IsInsideSpawnArea(Vector3 point)
    {
        if (spawnArea == null) return true; // no restriction
        if (!_haveAreaBounds) UpdateSpawnAreaBounds();
        if (!_haveAreaBounds) return true;

        // Quick AABB check
        if (!_areaBounds.Contains(point)) return false;

        // More precise check: if spawnArea has a collider, test point inside colliders using ClosestPoint
        // We'll consider the point inside if it's within a collider volume (for MeshCollider/BoxCollider)
        Collider[] cols = spawnArea.GetComponentsInChildren<Collider>();
        if (cols == null || cols.Length == 0) return true;

        foreach (var c in cols)
        {
            // If point is inside collider.bounds, check whether closest point equals point (inside)
            if (c.bounds.Contains(point))
            {
                Vector3 closest = c.ClosestPoint(point);
                // If the closest point is exactly the sample, it's on or inside; allow it.
                // If it's different, then the point is outside this collider � we continue searching.
                if (Vector3.Distance(closest, point) < 0.001f) return true;
            }
        }

        // If none of the colliders reported contain, reject
        return false;
    }

    bool IsDistanceValid(Vector3 point)
    {
        if (player == null) return false;
        float d = Vector3.Distance(new Vector3(point.x, 0f, point.z), new Vector3(player.transform.position.x, 0f, player.transform.position.z));
        return d >= minDistance && d <= maxDistance;
    }

    bool IsOverlappingBlocking(Vector3 point)
    {
        // Check overlap in a small sphere at the candidate to avoid spawning inside geometry or other actors
        Collider[] hits = Physics.OverlapSphere(point, spawnCheckRadius, blockingLayers);
        if (hits == null || hits.Length == 0) return false;

        // If hits include the spawnArea's own colliders (e.g., floor), we should allow those depending on your setup.
        // This implementation treats any hit as blocking. If your spawnArea uses a ground collider and you want to allow it,
        // you can refine the mask or check tags here.
        return true;
    }

    void SpawnEnemyAt(Vector3 pos)
    {
        if (enemyPoolKeys.Count == 0) return;

        int index = Random.Range(0, enemyPoolKeys.Count);
        string poolKey = enemyPoolKeys[index];
        
        // GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        // if (prefab == null) return;

        //Check if spawning on navmesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 10.0f, NavMesh.AllAreas))
        { 
            //GameObject go = Instantiate(prefab, hit.position, Quaternion.identity, spawnParent);
            GameObject go = objectPooler.Spawn(poolKey, hit.position, Quaternion.identity);
            if (go == null) return;
            
            // Set Pooler Reference
            var enemy = go.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetPooler(objectPooler);
                enemy.PoolKey = poolKey;
            }
            
            // re-enable NavMeshAgent
            var agent = go.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(hit.position);
            }
            
            // Optional initial look at player
            if (player != null)
            {
              Vector3 lookDir = (player.transform.position - go.transform.position);
              lookDir.y = 0f;
              if (lookDir.sqrMagnitude > 0.001f) 
                  go.transform.rotation = Quaternion.LookRotation(lookDir.normalized);
            }
        }
    }

    // Debug visualization in editor
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = new Color(1f, 0f, 0.25f, 0.15f);
            Gizmos.DrawWireSphere(player.transform.position, minDistance);
            Gizmos.color = new Color(0.25f, 0.8f, 1f, 0.15f);
            Gizmos.DrawWireSphere(player.transform.position, maxDistance);
        }

        if (spawnArea != null)
        {
            if (!_haveAreaBounds) UpdateSpawnAreaBounds();
            if (_haveAreaBounds)
            {
                Gizmos.color = new Color(0.1f, 1f, 0.1f, 0.12f);
                Gizmos.DrawCube(_areaBounds.center, _areaBounds.size);
            }
        }
    }
}