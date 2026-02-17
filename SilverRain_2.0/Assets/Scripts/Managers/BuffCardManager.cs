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
    
    [Header("Reroll and Ban settings")]
    [SerializeField, Tooltip("The maximum amount of times the player can reroll choices on level up")]
    private int startingRerollAmount;
    [SerializeField, Tooltip("The amount of rerolls currently available to the player")]
    private int rerollsAvailable;
    [SerializeField, Tooltip("The maximum amount of times the player can ban choices on level up")]
    private int startingBanAmount;
    [SerializeField, Tooltip("The amount of bans currently available to the player")]
    private int bansAvailable;

    [Header("References")]
    [SerializeField, Tooltip("PlayerExperience component used to subscribe to LevelUp event")]
    private PlayerExperience playerExperience;

    [Header("UI")]
    [SerializeField, Tooltip("BuffCardsWindow prefab to push via UIManager")]
    private UIWindow buffCardsWindowPrefab;

    private int pendingLevelUp;
    private bool isProcessingLevelUp;

    //Properties
    public List<TemporaryBuff> CurrentChoices => currentChoices;
    public int ChoiceAmount
    {
        get { return choiceAmount; }
        set { choiceAmount = Mathf.Max(value, 1); }
    }
    public int RerollsAvailable
    {
        get { return rerollsAvailable; }
        set { rerollsAvailable = Mathf.Clamp(value, 0, startingRerollAmount); }
    }
    public int BansAvailable
    {
        get { return bansAvailable; }
        set { bansAvailable = Mathf.Clamp(value, 0, startingBanAmount); }
    }

    private void Awake()
    {
        //Singleton pattern implementation
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
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
        if (playerExperience != null) { playerExperience.OnLevelUp.AddListener(AddToLevelUpQueue); }

        //Subscribe to availability change events for weapons, temporary upgrades, and modifications
        StatManager.Instance.OnTempUpgradeAvailabilityChange.AddListener(UpdateAvailableChoices);
        WeaponManager.Instance.OnWeaponAvailabilityChange.AddListener(UpdateAvailableChoices);
        if (StatManager.Instance == null) { Debug.Log("BuffCardManager - StatManager instance is null"); }
        //ModificationManager.Instance.OnModificationAvailabilityChange.AddListener(UpdateAvailableChoices);

        InitializeAvailableChoices();
    }

    private void AddToLevelUpQueue()
    {
        pendingLevelUp++;

    }

    private void Update()
    {
        if (pendingLevelUp > 0 && !isProcessingLevelUp)
        {
            DisplayBuffCards();
            
        }
    }

    private void InitializeAvailableChoices()
    {
        foreach (var weapon in WeaponManager.Instance.AllWeapons)
        {
            if (weapon.Value.IsAvailable) { availableChoices.Add(weapon.Value); }
        }
        foreach (var upgrade in StatManager.Instance.AllTempUpgrades)
        {
            if (upgrade.Value.IsAvailable) { availableChoices.Add(upgrade.Value); }
        }
        //foreach (var modification in ModificationManager.Instance.AllModifications)
        //{
        //    if (modification.Value.IsAvailable) { availableChoices.Add(modification.Value); }
        //}
    }
    private void DisplayBuffCards()
    {
        isProcessingLevelUp = true;
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
        
        for (int i = 0; i < buffAmount; i++)
        {
            // Randomly choose an ITemporary
            TemporaryBuff choice = PickRandomChoice(choicePool);
            currentChoices.Add(choice);
        }

        UIManager.Instance.Push(buffCardsWindowPrefab);
    }
    
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

        UIManager.Instance.Pop();
        isProcessingLevelUp = false;
        pendingLevelUp--;
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