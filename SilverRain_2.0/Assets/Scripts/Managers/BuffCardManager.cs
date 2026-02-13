using System.Collections.Generic;
using UnityEngine;

public class BuffCardManager : MonoBehaviour
{
    public static BuffCardManager Instance { get; private set; }

    [Header("Choices settings")]
    [SerializeField, Tooltip("The choices currently offered for level up")]
    private List<TemporaryBuff> currentChoices = new();
    [SerializeField, Tooltip("All available choices that may be offered on level up")]
    private HashSet<TemporaryBuff> availableChoices = new();
    [SerializeField, Min(1), Tooltip("The amount of choices that will be offered on level up")]
    private int choiceAmount = 3;
    
    // [Header("Buff Card Settings")]
    // [SerializeField, Min(1), Tooltip("The maximum amount of buff cards, used to create them in 'Start'")]
    // private int maxBuffCards = 5;
    // [SerializeField, Tooltip("A list of all buff cards")]
    // private List<BuffCard> buffCards = new();
    // [SerializeField, Tooltip("Prefab for the buff card UI element")]
    // private GameObject buffCardPrefab;
    // [SerializeField, Tooltip("Parent where the buff card will spawn")]
    // private Transform buffCardParent;
    [SerializeField, Tooltip("PlayerExperience component used to subscribe to LevelUp event")]
    private PlayerExperience playerExperience;
    
    [Header("UI")]
    [SerializeField, Tooltip("BuffCardsWindow prefab to push via UIManager")]
    private UIWindow buffCardsWindowPrefab;

    //Properties
    public List<TemporaryBuff> CurrentChoices => currentChoices;
    public int ChoiceAmount
    {
        get { return choiceAmount; }
        set { choiceAmount = Mathf.Max(value, 1); }
    }

    private void Awake()
    {
        //Singleton pattern implementation
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        //Subscribe to availability change events for weapons, temporary upgrades, and modifications
        StatManager.Instance.OnTempUpgradeAvailabilityChange.AddListener(UpdateAvailableChoices);
        WeaponManager.Instance.OnWeaponAvailabilityChange.AddListener(UpdateAvailableChoices);
        //ModificationManager.Instance.OnModificationAvailabilityChange.AddListener(UpdateAvailableChoices);
    }
    private void Start()
    {
        //Find PlayerExperience
        if (playerExperience == null)
        {
            PlayerFinder.Instance.Player.TryGetComponent<PlayerExperience>(out PlayerExperience playerXP);
            if (playerXP != null)
            {
                //Cache the PlayerExperience reference
                playerExperience = playerXP;
            }
        }
        if (playerExperience != null) { playerExperience.OnLevelUp.AddListener(DisplayBuffCards); }

        //Create maximum amount of buff cards
        // for (int i = 0; i < maxBuffCards; i++)
        // {
        //     GameObject buffObj = Instantiate(buffCardPrefab, buffCardParent);
        //     BuffCard card = buffObj.GetComponent<BuffCard>();
        //
        //     buffCards.Add(card);
        //     buffObj.SetActive(false);
        // }
    }

    private void DisplayBuffCards()
    {
        // Initialize choices lists
        currentChoices.Clear();
        List<TemporaryBuff> choicePool = new(availableChoices);

        // Ensure that there are enough available choices for each choice amount
        int buffAmount = Mathf.Min(choiceAmount, choicePool.Count);

        //------------------------------------------------------------------------------------------------------------------
        // If there are no available choices left when you level up we may want to reward
        // the player with something else, like score, full heal, reveal all enemies, etc.
        if (buffAmount <= 0) { return; }
        //------------------------------------------------------------------------------------------------------------------

        // Pause the game
        //GameManager.Instance.PauseGame();
        // _pauseToken = PauseManager.Instance.Acquire("BuffCard");
        // _inputToken = InputManager.Instance.Acquire(InputMode.UI, "BuffCard");
        
        for (int i = 0; i < buffAmount; i++)
        {
            // Randomly choose an ITemporary
            TemporaryBuff choice = PickRandomChoice(choicePool);
            currentChoices.Add(choice);

            // Setup and activate the buff card
            // buffCards[i].SetupCard(choice);
            // buffCards[i].gameObject.SetActive(true);
        }

        // Hide unused cards
        // for (int i = choiceAmount; i < buffCards.Count; i++)
        // {
        //     buffCards[i].gameObject.SetActive(false);
        // }
        UIManager.Instance.Push(buffCardsWindowPrefab);
    }
    // private void HideBuffCards()
    // {
    //     // Unpause the game
    //     //GameManager.Instance.UnpauseGame();
    //     if (_pauseToken != null)
    //     {
    //         PauseManager.Instance.Release(_pauseToken);
    //         _pauseToken = null;
    //     }
    //
    //     if (_inputToken != null)
    //     {
    //         InputManager.Instance.Release(_inputToken);
    //         _inputToken = null;
    //     }
    //     
    //     foreach (var card in buffCards)
    //     {
    //         card.gameObject.SetActive(false);
    //     }
    // }
    
    private TemporaryBuff PickRandomChoice(List<TemporaryBuff> pool)
    {
        if (pool.Count == 0) { Debug.Log("BuffCardManager - Available pool is empty"); return null; }

        int index = Random.Range(0, pool.Count);
        TemporaryBuff chosen = pool[index];
        // Ensures uniqueness
        pool.RemoveAt(index);
        return chosen;
    }

    public void ChooseBuffCard(TemporaryBuff buffClicked)
    {
        switch (buffClicked)
        {
            case Weapon weapon:
                WeaponManager.Instance.AddWeapon(weapon.WeaponType);
                break;
            case TemporaryUpgrade upgrade:
                StatManager.Instance.AddTempUpgrade(upgrade.StatType);
                break;
            case Modification modification:
                ModificationManager.Instance.AddModification(modification);
                break;
            default:
                Debug.Log("BuffCardManager - ITemporary type not recognized");
                break;
        }

        //HideBuffCards();
        UIManager.Instance.Pop();
    }


    //This method is called whenever a weapon, temporary upgrade, or modification changes availability.
    private void UpdateAvailableChoices(TemporaryBuff temp, bool isAvailable)
    {
        if (isAvailable)
            availableChoices.Add(temp);
        else
            availableChoices.Remove(temp);
    }

    //Unsubscribe from events to prevent memory leaks
    private void OnDestroy()
    {
        if (playerExperience != null)
        {
            playerExperience.OnLevelUp.RemoveListener(DisplayBuffCards);
        }
        WeaponManager.Instance.OnWeaponAvailabilityChange.RemoveListener(UpdateAvailableChoices);
        StatManager.Instance.OnTempUpgradeAvailabilityChange.RemoveListener(UpdateAvailableChoices);
        //ModificationManager.Instance.OnModificationAvailabilityChange.RemoveListener(UpdateAvailableChoices);
    }
}