using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterReveal : MonoBehaviour
{
    private ParticleSystem ps;
    //private List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();

    //private List<GameObject> enemies = new List<GameObject>();

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        Destroy(this.gameObject, ps.main.duration);
    }

    private void OnParticleCollision(GameObject other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !GlobalInvisibilityManager.Instance.isActive) 
        {
           enemy.RevealTimed(5f);
        }
    }

    //private void OnParticleTrigger()
    //{
    //    Debug.Log("Trigger");
    //    int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

    //    for (int i = 0; i < numEnter; i++)
    //    {
    //        ParticleSystem.Particle p = particles[i];

    //        Collider[] colliders = Physics.OverlapSphere(p.position, 1.0f);
    //        foreach (var col in colliders)
    //        {
    //            Enemy enemy = col.GetComponent<Enemy>();
    //            if (enemy != null)
    //            {
    //                enemy.Reveal();
    //            }
    //        }
    //    }
    //}
}
