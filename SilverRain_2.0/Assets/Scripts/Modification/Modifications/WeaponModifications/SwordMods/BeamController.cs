using System.Collections;
using UnityEngine;

public class BeamController : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("The damage dealt by the beam")]
    private float damage;
    [SerializeField, Tooltip("The duration the beam stays active")]
    private float duration;
    [SerializeField, Tooltip("The cooldown time between beam activations")]
    private float cooldown;
    [SerializeField, Tooltip("The maximum length of the beam")]
    private float size;
    [SerializeField, Tooltip("The key used to access the pool containing the beam VFX")]
    private string poolKey = "BeamSword";
    [SerializeField, Tooltip("The visuals for the beam")]
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

    public void OnCreatedPool() { }

    public void OnSpawnFromPool() { }

    public void OnReturnToPool() { }
}