using UnityEngine;
using UnityEngine.UI;

public class UITemporary : MonoBehaviour
{
    [SerializeField, Tooltip("The name of the buff")]
    private string buffName;
    [SerializeField, Tooltip("The current level of the buff")]
    private string buffLevel;
    [SerializeField, Tooltip("The description of whta the buff does, this doesn't change")]
    private string baseBuffDescription;
    [SerializeField, Tooltip("The description of how the buff improves per level")]
    private string finalBuffDescription;
    [SerializeField, Tooltip("The icon representing the buff")]
    private Sprite buffIcon;

    // Properties
    public string BuffName => buffName;
    public string BuffLevel => buffLevel;
    public string BaseBuffDescription => baseBuffDescription;
    public string FinalBuffDescription => finalBuffDescription;
    public Sprite BuffIcon => buffIcon;

    // Description update method + overloads
    #region Modification Descriptions
    public void UpdateDescription()
    {
        finalBuffDescription = baseBuffDescription;
    }
    #endregion

    #region Upgrade Descriptions
    public void UpdateDescription(int level, int maxLevel, float curStat, float nextStat)
    {
        // If the next level is the max, add (MAX) at the end
        if (level == maxLevel - 1)
        {
            buffLevel = $"{level} -> {maxLevel}(MAX)";
        }
        // Display the next level number
        else
        {
            buffLevel = $"{level} -> {level + 1}";
        }
        // Display the current stats and the next level stats
        finalBuffDescription = $"{baseBuffDescription}\n%{curStat} -> %{nextStat}";
    }
    #endregion

    #region Weapon Descriptions
    public void UpdateDescription(int level, int maxLevel, string statName,
        float curStat, float nextStat)
    {
        // If the next level is the max, add (MAX) at the end
        if (level == maxLevel - 1)
        {
            buffLevel = $"{level} -> {maxLevel}(MAX)";
        }
        // Display the next level number
        else
        {
            buffLevel = $"{level} -> {level + 1}";
        }
        // Display the current stats and the next level stats
        finalBuffDescription = $"{baseBuffDescription}\n{statName}: %{curStat} -> %{nextStat}";
    }

    public void UpdateDescription(int level, int maxLevel,
        string statName1, float curStat1, float nextStat1,
        string statName2, float curStat2, float nextStat2)
    {
        // If the next level is the max, add (MAX) at the end
        if (level == maxLevel - 1)
        {
            buffLevel = $"{level} -> {maxLevel}(MAX)";
        }
        // Display the next level number
        else
        {
            buffLevel = $"{level} -> {level + 1}";
        }
        // Display the current stats and the next level stats
        finalBuffDescription = $"{baseBuffDescription}" +
            $"\n{statName1}: %{curStat1} -> %{nextStat1}" +
            $"\n{statName2}: %{curStat2} -> %{nextStat2}";
    }

    public void UpdateDescription(int level, int maxLevel,
        string statName1, float curStat1, float nextStat1,
        string statName2, float curStat2, float nextStat2,
        string statName3, float curStat3, float nextStat3)
    {
        // If the next level is the max, add (MAX) at the end
        if (level == maxLevel - 1)
        {
            buffLevel = $"{level} -> {maxLevel}(MAX)";
        }
        // Display the next level number
        else
        {
            buffLevel = $"{level} -> {level + 1}";
        }
        // Display the current stats and the next level stats
        finalBuffDescription = $"{baseBuffDescription}" +
            $"\n{statName1}: %{curStat1} -> %{nextStat1}" +
            $"\n{statName2}: %{curStat2} -> %{nextStat2}" +
            $"\n{statName3}: %{curStat3} -> %{nextStat3}";
    }

    public void UpdateDescription(int level, int maxLevel,
        string statName1, float curStat1, float nextStat1,
        string statName2, float curStat2, float nextStat2,
        string statName3, float curStat3, float nextStat3,
        string statName4, float curStat4, float nextStat4)
    {
        // If the next level is the max, add (MAX) at the end
        if (level == maxLevel - 1)
        {
            buffLevel = $"{level} -> {maxLevel}(MAX)";
        }
        // Display the next level number
        else
        {
            buffLevel = $"{level} -> {level + 1}";
        }
        // Display the current stats and the next level stats
        finalBuffDescription = $"{baseBuffDescription}" +
            $"\n{statName1}: %{curStat1} -> %{nextStat1}" +
            $"\n{statName2}: %{curStat2} -> %{nextStat2}" +
            $"\n{statName3}: %{curStat3} -> %{nextStat3}" +
            $"\n{statName4}: %{curStat4} -> %{nextStat4}";
    }

    public void UpdateDescription(int level, int maxLevel,
        string statName1, float curStat1, float nextStat1,
        string statName2, float curStat2, float nextStat2,
        string statName3, float curStat3, float nextStat3,
        string statName4, float curStat4, float nextStat4,
        string statName5, float curStat5, float nextStat5)
    {
        // If the next level is the max, add (MAX) at the end
        if (level == maxLevel - 1)
        {
            buffLevel = $"{level} -> {maxLevel}(MAX)";
        }
        // Display the next level number
        else
        {
            buffLevel = $"{level} -> {level + 1}";
        }
        // Display the current stats and the next level stats
        finalBuffDescription = $"{baseBuffDescription}" +
            $"\n{statName1}: %{curStat1} -> %{nextStat1}" +
            $"\n{statName2}: %{curStat2} -> %{nextStat2}" +
            $"\n{statName3}: %{curStat3} -> %{nextStat3}" +
            $"\n{statName4}: %{curStat4} -> %{nextStat4}" +
            $"\n{statName5}: %{curStat5} -> %{nextStat5}";
    }
    #endregion
}
