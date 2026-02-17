using UnityEngine;

public class SilverIodide : Pickup
{
    [Header("Silver Iodide Settings")]
    [SerializeField, Tooltip("The duration for which the silver iodide effect reveals enemies")]
    private float revealTime = 30f;

    public override void OnPickup()
    {
        GlobalInvisibilityManager.Instance.OnGlobalReveal?.Invoke(revealTime);
        GlobalInvisibilityManager.Instance.SetTimer(revealTime);
        base.OnPickup();
    }
}