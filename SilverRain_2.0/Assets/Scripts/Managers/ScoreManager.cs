using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private float currentScore;
    public static UnityEvent OnScoreChanged;

    public void AddScore(float amount)
    {

    }

    public void ResetScore()
    {

    }
}
