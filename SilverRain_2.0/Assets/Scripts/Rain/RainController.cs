using UnityEngine;

public class RainController : MonoBehaviour
{
    public static RainController Instance { get; private set; }

    [SerializeField, Tooltip("The player's transform, used to position the rain system above them.")]
    private Transform player;
    [SerializeField, Tooltip("The ParticleSystem component for the rain effect.")]
    private ParticleSystem rainSystem;
    [SerializeField, Tooltip("Height offset for the rain system above the player.")]
    private float heightOffset = 20f;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        if (PlayerFinder.Instance != null && PlayerFinder.Instance.Player != null)
        {
            player = PlayerFinder.Instance.Player.transform;
        }
        rainSystem = GetComponent<ParticleSystem>();
        
    }

    private void LateUpdate()
    {
        if (player == null) return;
        Vector3 pos = player.position;
        pos.y += heightOffset;
        transform.position = pos;
    }

    public void StartRain() 
    {
        if (rainSystem == null) return;
        if (!rainSystem.isPlaying) rainSystem.Play();
    }

    public void StopRain() 
    {
        if (rainSystem == null) return;
        if (rainSystem.isPlaying) rainSystem.Stop();
    }
}
