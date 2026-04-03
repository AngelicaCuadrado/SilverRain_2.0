using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GlobalInvisibilityManager : MonoBehaviour
{
    public static GlobalInvisibilityManager Instance { get; private set; }

    [SerializeField, Tooltip("The timer for the global invisibility effect")]
    private float invisibilityTimer;
    [SerializeField, Tooltip("Whether the global invisibility effect is currently active")]
    private bool isActive = false;
    [SerializeField, Tooltip("The object pooler for blood splatter effects")]
    private ObjectPooler bloodSplatterPool;

    // Properties
    public float InvisibilityTimer => invisibilityTimer;
    public bool IsActive => isActive;
    public ObjectPooler BloodSplatterPool => bloodSplatterPool;

    // Events
    [HideInInspector] public UnityEvent<float> OnGlobalReveal;

    private void Awake()
    {
        if (Instance == null || Instance == this) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    public void ActivateInvisibility(float duration)
    {
        OnGlobalReveal?.Invoke(duration);
        isActive = true;
        RainController.Instance.StartRain();
        // Stop any existing countdown and start a new one
        StopAllCoroutines();
        StartCoroutine(InvisibilityCountdown(duration));
    }

    private IEnumerator InvisibilityCountdown(float duration)
    {
        invisibilityTimer = duration;
        while (invisibilityTimer > 0f)
        {
            invisibilityTimer -= Time.deltaTime;
            yield return null;
        }
        RainController.Instance.StopRain();
        isActive = false;
    }
}