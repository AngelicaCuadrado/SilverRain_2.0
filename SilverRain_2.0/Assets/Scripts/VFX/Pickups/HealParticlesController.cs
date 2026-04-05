using UnityEngine;

public class HealParticlesController : MonoBehaviour
{
    [SerializeField, Tooltip("Particle system for healing effect")]
    private ParticleSystem healParticles;

    private void Awake()
    {
        healParticles = GetComponent<ParticleSystem>();
    }

    public void PlayHealParticles()
    {
        if (healParticles != null)
        {
            // Restart the particle system to ensure it plays from the beginning
            if (healParticles.isPlaying)
            {
                healParticles.Stop();
                healParticles.Clear();
            }

            // Play the healing particles
            healParticles.Play();
        }
    }
}