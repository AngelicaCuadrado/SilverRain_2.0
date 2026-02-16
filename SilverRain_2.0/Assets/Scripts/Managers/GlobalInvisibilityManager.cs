using System;
using UnityEngine;
using UnityEngine.Events;

public class GlobalInvisibilityManager : MonoBehaviour
{
    public static GlobalInvisibilityManager Instance { get; private set; }

    [SerializeField, Tooltip("The duration for which the global invisibility effect will last when activated")]
    private float invisibilityTimer = 0f;
    [SerializeField, Tooltip("Whether the global invisibility effect is currently active")]
    private bool isActive = false;

    // Properties
    public float InvisibilityTimer => invisibilityTimer;
    public bool IsActive => isActive;

    // Events
    public UnityEvent<float> OnGlobalReveal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (isActive)
        {
            invisibilityTimer -= Time.deltaTime;
            if (invisibilityTimer <= 0f)
            {
                isActive = false;
            }
        }
    }

    public void SetTimer(float seconds)
    {
        invisibilityTimer = seconds;
        isActive = true;
    }
}
