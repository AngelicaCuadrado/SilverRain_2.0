using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField, Tooltip("All pickup prefabs that can be spawned")]
    private List<GameObject> allPickupPrefabs = new();
    [SerializeField, Tooltip("Possible spawn locations for pickups")]
    private List<Transform> spawnLocations = new();
    [Tooltip("Tracks the availability of each spawn location (true = available, false = occupied)")]
    private Dictionary<Transform, bool> spawnLocationStatus = new();
    [SerializeField, Tooltip("The object pool for pickups")]
    private ObjectPooler pickupPool;
    [Space]

    [Header("Pickup Amounts")]
    [SerializeField, Tooltip("Maximum number of pickups that can be active at once")]
    private int maxPickups = 3;
    [SerializeField, Tooltip("Current number of active pickups in the scene")]
    private int currentPickups;
    [Space]

    [Header("Timer Settings")]
    [SerializeField, Tooltip("Time in seconds between pickup spawns")]
    private float timeToSpawn = 60f;
    [SerializeField, Tooltip("Time in seconds since the last pickup spawn")]
    private float spawnTimer;


    // Properties
    public ObjectPooler PickupPool => pickupPool;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize spawn location status
        foreach (var location in spawnLocations)
        {
            spawnLocationStatus[location] = true;
        }
    }

    private void Update()
    {
        // Only spawn pickups if we haven't reached the maximum amount
        if (currentPickups >= maxPickups)
            return;

        // Increment the spawn timer and check if it's time to spawn a new pickup
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= timeToSpawn)
        {
            SpawnPickup();
            spawnTimer = 0f;
        }
    }

    private void SpawnPickup()
    {
        // Validate that we have spawn locations and pickup prefabs assigned
        if (spawnLocations.Count == 0)
        {
            Debug.LogWarning("No spawn locations assigned");
            return;
        }
        if (allPickupPrefabs.Count == 0)
        {
            Debug.LogWarning("No pickup prefabs assigned");
            return;
        }

        // Pick a random available spawn point
        var availableSpawnPoints = new List<Transform>();
        foreach (var kvp in spawnLocationStatus)
        {
            if (kvp.Value) availableSpawnPoints.Add(kvp.Key);
        }
        if (availableSpawnPoints.Count == 0) { return; }
        var spawnIndex = Random.Range(0, availableSpawnPoints.Count);
        Transform spawnPoint = availableSpawnPoints[spawnIndex];

        // Get a random pickup from the pool
        var pickupIndex = Random.Range(0, allPickupPrefabs.Count);
        var pickup = allPickupPrefabs[pickupIndex].GetComponent<Pickup>();
        if (pickup == null)
        {
            Debug.LogWarning($"Prefab {allPickupPrefabs[pickupIndex].name} does not have a Pickup component");
            return;
        }

        // Spawn the pickup at the chosen location and update tracking
        pickupPool.Spawn(pickup.PoolKey, spawnPoint.position, spawnPoint.rotation);
        pickup.LocationIndex = spawnLocations.IndexOf(spawnPoint);
        spawnLocationStatus[spawnPoint] = false;
        currentPickups++;
    }

    public void ClearSpawnSpot(int index)
    {
        if (index < 0 || index >= spawnLocations.Count)
        {
            Debug.LogWarning($"Invalid spawn location index: {index}");
            return;
        }
        var location = spawnLocations[index];
        spawnLocationStatus[location] = true;
        currentPickups = Mathf.Max(0, currentPickups - 1);
    }
}