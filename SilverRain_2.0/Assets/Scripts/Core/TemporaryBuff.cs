using UnityEngine;
using UnityEngine.Events;

public abstract class TemporaryBuff : MonoBehaviour, IUpgradeable
{
    [Header("Level")]
    [SerializeField,Tooltip("")]
    protected int level;
    [SerializeField, Tooltip("")]
    protected int maxLevel;
    [Space]

    [Header("Availability")]
    [SerializeField, Tooltip("")]
    protected bool isAvailable;
    [SerializeField, Tooltip("")]
    protected bool isAvailableAtStart;
    [Space]

    [Header("UI")]
    [SerializeField, Tooltip("")]
    protected UITemporary uiData;

    // Events
    public UnityEvent<TemporaryBuff, bool> OnAvailabilityChanged;

    // Properties
    public int Level => level;
    public int MaxLevel => maxLevel;
    public bool IsAvailable => isAvailable;
    public bool IsAvailableAtStart => isAvailableAtStart;
    public UITemporary UIData { get { return uiData; } }

    public virtual void Start()
    {
        // Make available in BuffCardManager
        SetAvailable(isAvailableAtStart);
        // Update UI for buff cards
        UpdateDescription();
    }
    public abstract void SetAvailable(bool availability);
    public virtual void LevelUp()
    {
        //Ensure we don't exceed max level
        if (level >= maxLevel) return;
        //Increase level
        level++;
        //Update UI
        UpdateDescription();
        //Check if we've reached max level
        if (level >= maxLevel)
        {
            SetAvailable(false);
        }
    }

    public virtual void ResetLevels()
    {
        // Reset level and stats
        level = 0;
        // Update UI
        UpdateDescription();
        // Reset availability
        SetAvailable(true);
    }

    public abstract void UpdateDescription();
}
