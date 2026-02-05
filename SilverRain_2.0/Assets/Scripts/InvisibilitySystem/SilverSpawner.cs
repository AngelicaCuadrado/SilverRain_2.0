using UnityEngine;

public class SilverSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject silverPrefab;
    [SerializeField]
    private float timeToSpawn = 60f;
    [SerializeField]
    private Transform[] spawnLocations;

    private GameObject currentSilver;

    private float spawnTimer;

    private void Start()
    {
        
    }

    void Update()
    {
        if (currentSilver == null) spawnTimer += Time.deltaTime;
        if (spawnTimer > timeToSpawn)
        {
            SpawnSilver();
            spawnTimer = 0f;
        }
    }

    private void SpawnSilver()
    {
        if (spawnLocations.Length == 0)
        {
            Debug.LogWarning("No spawn locations assigned");
            return;
        }

        // Pick a random spawn point
        int index = Random.Range(0, spawnLocations.Length);
        Transform spawnPoint = spawnLocations[index];

        // Instantiate at that location
        currentSilver = Instantiate(silverPrefab, spawnPoint.position, spawnPoint.rotation);
    }

}
