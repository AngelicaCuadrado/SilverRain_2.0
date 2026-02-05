using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    Vector3 direction;
    float damage;
    float speed = 10f;
    PlayerHealth targetPlayerHealth;
    float deathTimer = 10f;

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void Initialize(Vector3 direction, float damage, PlayerHealth target)
    {
        this.direction = direction.normalized;
        this.damage = damage;
        targetPlayerHealth = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPlayerHealth.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
    private void Start()
    {
        Destroy(gameObject, deathTimer);
    }
}
