using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFinder : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private static PlayerFinder instance;

    public GameObject Player => player;
    public static PlayerFinder Instance => instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        FindPlayer();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }
    public void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("PlayerFinder did not find a player in this scene.");
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
