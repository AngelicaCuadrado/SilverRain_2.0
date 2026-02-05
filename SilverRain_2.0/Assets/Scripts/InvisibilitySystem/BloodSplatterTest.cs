using UnityEngine;

public class BloodSplatterTest : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem bloodSplatterPrefab;
    [SerializeField]
    private EnemyHealth testEnemy;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestBloodSplatter();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TestEnemyDamage();
        }
    }

    private void TestBloodSplatter() 
    {
        Vector3 bloodSplatterSpawn = transform.position;
        bloodSplatterSpawn.y += 2;
        var bloodSplatter = Instantiate(bloodSplatterPrefab, bloodSplatterSpawn, Quaternion.identity);

        bloodSplatter.Play();
        Destroy(bloodSplatter, bloodSplatter.main.duration);
    }

    private void TestEnemyDamage() 
    {
        testEnemy.TakeDamage(1);
    }
}
