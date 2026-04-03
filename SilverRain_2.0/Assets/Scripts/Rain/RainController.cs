using UnityEngine;
using UnityEngine.VFX;

public class RainController : MonoBehaviour
{
    public static RainController Instance { get; private set; }

    [SerializeField, Tooltip("The player's transform, used to position the rain system above them.")]
    private Transform player;

    [SerializeField, Tooltip("The VisualEffect component for the rain effect.")]
    private VisualEffect rainVFX;

    private void Awake()
    {
        if (Instance == null || Instance == this) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        if (PlayerFinder.Instance != null && PlayerFinder.Instance.Player != null)
        {
            player = PlayerFinder.Instance.Player.transform;
        }

        if (rainVFX == null)
            rainVFX = GetComponentInChildren<VisualEffect>();

        StopRain();
    }

    private void LateUpdate()
    {
        if (player == null) return;

        transform.position = player.position;
    }

    public void StartRain()
    {
        if (rainVFX == null) return;
        rainVFX.Play();
    }

    public void StopRain()
    {
        if (rainVFX == null) return;
        rainVFX.Stop();
    }
}