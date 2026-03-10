using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GlobalInvisibilityManager : MonoBehaviour
{
    public static GlobalInvisibilityManager Instance { get; private set; }
    [SerializeField, Tooltip("")]
    private float invisibilityTimer;
    [SerializeField, Tooltip("Whether the global invisibility effect is currently active")]
    private bool isActive = false;

    // Properties
    public float InvisibilityTimer => invisibilityTimer;
    public bool IsActive => isActive;

    // Events
    public UnityEvent<float> OnGlobalReveal;

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