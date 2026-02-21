using System.Collections.Generic;
using UnityEngine;

public class BuffCardManager : MonoBehaviour
{
    public static BuffCardManager Instance { get; private set; }

    [Header("Choices settings")]
    [SerializeField, Tooltip("The choices currently offered for level up")]
    private List<TemporaryBuff> currentChoices = new();
    [Tooltip("All available choices that may be offered on level up")]
    private HashSet<TemporaryBuff> availableChoices = new();
    [SerializeField, Min(1), Tooltip("The amount of choices that will be offered on level up")]
    private int choiceAmount = 3;
    [Tooltip("A temporary list used to generate choices on level up without modifying the availableChoices list")]
    private List<TemporaryBuff> choicePool;

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
    [Tooltip("PlayerExperience component used to subscribe to LevelUp event")]
    private PlayerExperience playerExperience;

    [Header("UI")]
    [SerializeField, Tooltip("BuffCardsWindow prefab to push via UIManager")]
    private UIWindow buffCardsWindowPrefab;

    [Header("Level Up Queue")]
    [Tooltip("Tracks pending level ups to ensure they are processed one at a time")]
    private int pendingLevelUp;
    [Tooltip("Flag to indicate if we are currently processing a level up to prevent multiple simultaneous processes")]
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
        if (playerExperience != null) { playerExperience.OnLevelUp.AddListener(OnLevelUp); }

        //Subscribe to availability change events for weapons, temporary upgrades, and modifications
        StatManager.Instance.OnTempUpgradeAvailabilityChange.AddListener(UpdateAvailableChoices);
        WeaponManager.Instance.OnWeaponAvailabilityChange.AddListener(UpdateAvailableChoices);
        if (StatManager.Instance == null) { Debug.Log("BuffCardManager - StatManager instance is null"); }
        //ModificationManager.Instance.OnModificationAvailabilityChange.AddListener(UpdateAvailableChoices);

        InitializeAvailableChoices();

        // Initialize rerolls and bans
        rerollsAvailable = startingRerollAmount;
        bansAvailable = startingBanAmount;
    }

    private void Update()
    {
        if (pendingLevelUp > 0 && !isProcessingLevelUp)
        {
            DisplayBuffCards();
        }
    }

    private void OnLevelUp()
    {
        pendingLevelUp++;
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
        choicePool = new(availableChoices);

        // Ensure that there are enough available choices for each choice amount
        int buffAmount = Mathf.Min(choiceAmount, choicePool.Count);
        //------------------------------------------------------------------------------------------------------------------
        // If there are no available choices left when you level up we may want to reward
        // the player with something else, like score, full heal, reveal all enemies, etc.
        if (buffAmount <= 0) { Debug.Log("BuffCardManager - No available choices left"); return; }
        //------------------------------------------------------------------------------------------------------------------

        for (int i = 0; i < buffAmount; i++)
        {
            // Randomly choose an ITemporary
            TemporaryBuff choice = PickRandomChoice(choicePool);
            if (choice != null) currentChoices.Add(choice);
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

    public void RerollChoices()
    {
        if (rerollsAvailable <= 0) { Debug.Log("BuffCardManager - No rerolls available"); return; }
        UIManager.Instance.Pop();
        rerollsAvailable--;

        currentChoices.Clear();

        if (choicePool.Count < choiceAmount)
        {
            choicePool = new(availableChoices);
        }

        int buffAmount = Mathf.Min(choiceAmount, choicePool.Count);

        //------------------------------------------------------------------------------------------------------------------
        // If there are no available choices left when you level up we may want to reward
        // the player with something else, like score, full heal, reveal all enemies, etc.
        if (buffAmount <= 0) { Debug.Log("BuffCardManager - No available choices left"); return; }
        //------------------------------------------------------------------------------------------------------------------

        for (int i = 0; i < buffAmount; i++)
        {
            // Randomly choose an ITemporary
            TemporaryBuff choice = PickRandomChoice(choicePool);
            if (choice != null)  currentChoices.Add(choice);
        }

        UIManager.Instance.Push(buffCardsWindowPrefab);

    }

    public void BanChoice(TemporaryBuff buffClicked)
    {
        if (bansAvailable <= 0) { Debug.Log("BuffCardManager - No bans available"); return; }
        UIManager.Instance.Pop();
        bansAvailable--;
        // Remove the banned choice from the available choices and current choices
        buffClicked.SetAvailable(false);
        currentChoices.Remove(buffClicked);
        // If there are no more choices to offer, just reopen the window with the remaining choices
        if (currentChoices.Count == 0)
        {
            UIManager.Instance.Push(buffCardsWindowPrefab);
            return;
        }
        // Otherwise, fill the empty slot with a new random choice
        if (choicePool.Count == 0)
        {
            choicePool = new(availableChoices);
        }
        TemporaryBuff choice = PickRandomChoice(choicePool);
        if (choice != null) currentChoices.Add(choice);
        UIManager.Instance.Push(buffCardsWindowPrefab);
    }

    //This method is called whenever a weapon, temporary upgrade, or modification changes availability.
    private void UpdateAvailableChoices(TemporaryBuff temp, bool isAvailable)
    {
        if (isAvailable)
            availableChoices.Add(temp);
        else
            availableChoices.Remove(temp);
    }

    public void AddRerolls(int amount)
    {
        rerollsAvailable += amount;
    }

    public void AddBans(int amount)
    {
        bansAvailable += amount;
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