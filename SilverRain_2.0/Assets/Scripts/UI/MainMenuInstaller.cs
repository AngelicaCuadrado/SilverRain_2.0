using UnityEngine;

public class MainMenuInstaller : MonoBehaviour
{
    [SerializeField] private MainMenuWindow mainMenuPrefab;
    void Start()
    {
        InputManager.Instance.Apply(InputMode.UI);
        
        // Install Main Menu
        UIManager.Instance.Push(mainMenuPrefab);
    }


}
