using System.Collections;
using UnityEngine;

public class BeamController : MonoBehaviour
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
    private bool firing;

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
            // Fire beam
            firing = true;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                FireBeam();
                yield return null;
            }

            firing = false;

            // Cooldown
            yield return new WaitForSeconds(cooldown);
        }
    }

    private void FireBeam()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        float maxDistance = size * 5f; // tweak multiplier

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, LayerMask.GetMask("Default", "Ground")))
        {
            maxDistance = hit.distance;
        }

        // Damage enemies along the beam
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance, LayerMask.GetMask("Enemy"));
        foreach (var h in hits)
        {
            var enemy = h.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(Mathf.RoundToInt(damage));
        }

        // Update visuals (line renderer, mesh, etc.)
        UpdateBeamVisual(maxDistance);
    }

    private void UpdateBeamVisual(float length)
    {
        // Example for LineRenderer
        var lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, Vector3.forward * length);
        }
    }
}