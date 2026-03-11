using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private float currentScore;
    public static UnityEvent OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("More than one ScoreManager found in the scene");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddScore(float amount)
    {

    }

    public void ResetScore()
    {

    }
}