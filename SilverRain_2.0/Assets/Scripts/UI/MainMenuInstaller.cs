using UnityEngine;

public class MainMenuInstaller : MonoBehaviour
{
    [SerializeField] private MainMenuWindow mainMenuPrefab;
    void Start()
    {
        // Input mode set as UI
        // TODO: InputManager.Instance....
        
        // Install Main Menu
        UIManager.Instance.Push(mainMenuPrefab);
    }


}
