using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Minimal implementation
    public static GameManager Instance { get; private set; }
    public GameObject Player;
    void Awake() => Instance = this;
    public void AddScore(int score) {  }
}