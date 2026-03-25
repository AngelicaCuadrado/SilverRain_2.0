using System.Collections;
using UnityEngine;

public class BeamController : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("")]
    private float damage;
    [SerializeField, Tooltip("")]
    private float duration;
    [SerializeField, Tooltip("")]
    private float cooldown;
    [SerializeField, Tooltip("")]
    private float size;
    [SerializeField, Tooltip("")]
    private string poolKey = "BeamSword";
    [SerializeField, Tooltip("")]
    private GameObject visuals;

    public string PoolKey { get => poolKey; set => poolKey = value; }

    public void Init(float dmg, float dur, float cd, float sz)
    {
        damage = dmg;
        duration = dur;
        cooldown = cd;
        size = sz;

        StartCoroutine(FireLoop());
    }

    private IEnumerator FireLoop()
    {
        while (true)
        {
            // Turn visuals on
            visuals.SetActive(true);

            // Fire beam
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                FireBeam();
                yield return null;
            }

            // Turn visuals off
            visuals.SetActive(false);

            // Cooldown
            yield return new WaitForSeconds(cooldown);
        }
    }

    private void FireBeam()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, size, LayerMask.GetMask("Default", "Ground")))
        {
            size = hit.distance;
        }

        // Damage enemies along the beam
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, size, LayerMask.GetMask("Enemy"));
        foreach (var h in hits)
        {
            var enemy = h.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(Mathf.RoundToInt(damage));
            }
        }

        // Update visuals (line renderer, mesh, etc.)
        UpdateBeamVisual(size);
    }

    private void UpdateBeamVisual(float length)
    {
        var lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, Vector3.forward * length);
        }
    }

    public void OnCreatedPool()
    {
        
    }

    public void OnSpawnFromPool()
    {
        
    }

    public void OnReturnToPool()
    {
        
    }
}