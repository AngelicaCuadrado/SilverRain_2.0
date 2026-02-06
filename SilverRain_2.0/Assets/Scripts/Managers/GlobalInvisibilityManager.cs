using UnityEngine;

public class GlobalInvisibilityManager : MonoBehaviour
{
    public float invisibilityTimer = 0f;
    public bool isActive = false;

    public static GlobalInvisibilityManager Instance { get; private set; }

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
