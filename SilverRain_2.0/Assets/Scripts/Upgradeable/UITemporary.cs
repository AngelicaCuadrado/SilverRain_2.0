using UnityEngine;
using UnityEngine.UI;

public class UITemporary : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    public string buffName;
    [SerializeField, Tooltip("")]
    public string buffLevel;
    [SerializeField, Tooltip("")]
    public string baseBuffDescription;
    [SerializeField, Tooltip("")]
    public string finalBuffDescription;
    [SerializeField, Tooltip("")]
    public Sprite buffIcon;

    #region Modification Descriptions
    public void UpdateDescription()
    {
        buffLevel = "Modification";
        finalBuffDescription = baseBuffDescription;
    }
    #endregion

    #region Upgrade Descriptions
    public void UpdateDescription(int level, float curStat, float nextStat)
    {
        buffLevel = level.ToString();
        finalBuffDescription = $"{baseBuffDescription}\n%{curStat} -> %{nextStat}";
    }
    #endregion

    #region Weapon Descriptions
    public void UpdateDescription(int level, string statName, float curStat, float nextStat)
    {
        buffLevel = level.ToString();
        finalBuffDescription = $"{baseBuffDescription}\n{statName}: %{curStat} -> %{nextStat}";
    }

    public void UpdateDescription(int level,
        string statName1, float curStat1, float nextStat1,
        string statName2, float curStat2, float nextStat2)
    {
        buffLevel = level.ToString();
        finalBuffDescription = $"{baseBuffDescription}" +
            $"\n{statName1}: %{curStat1} -> %{nextStat1}" +
            $"\n{statName2}: %{curStat2} -> %{nextStat2}";
    }

    public void UpdateDescription(int level, 
        string statName1, float curStat1, float nextStat1,
        string statName2, float curStat2, float nextStat2,
        string statName3, float curStat3, float nextStat3)
    {
        buffLevel = level.ToString();
        finalBuffDescription = $"{baseBuffDescription}" +
            $"\n{statName1}: %{curStat1} -> %{nextStat1}" +
            $"\n{statName2}: %{curStat2} -> %{nextStat2}" +
            $"\n{statName3}: %{curStat3} -> %{nextStat3}";
    }

    public void UpdateDescription(int level, 
        string statName1, float curStat1, float nextStat1,
        string statName2, float curStat2, float nextStat2, 
        string statName3, float curStat3, float nextStat3, 
        string statName4, float curStat4, float nextStat4)
    {
        buffLevel = level.ToString();
        finalBuffDescription = $"{baseBuffDescription}" +
            $"\n{statName1}: %{curStat1} -> %{nextStat1}" +
            $"\n{statName2}: %{curStat2} -> %{nextStat2}" +
            $"\n{statName3}: %{curStat3} -> %{nextStat3}" +
            $"\n{statName4}: %{curStat4} -> %{nextStat4}";
    }

    public void UpdateDescription(int level, 
        string statName1, float curStat1, float nextStat1,
        string statName2, float curStat2, float nextStat2, 
        string statName3, float curStat3, float nextStat3, 
        string statName4, float curStat4, float nextStat4, 
        string statName5, float curStat5, float nextStat5)
    {
        buffLevel = level.ToString();
        finalBuffDescription = $"{baseBuffDescription}" +
            $"\n{statName1}: %{curStat1} -> %{nextStat1}" +
            $"\n{statName2}: %{curStat2} -> %{nextStat2}" +
            $"\n{statName3}: %{curStat3} -> %{nextStat3}" +
            $"\n{statName4}: %{curStat4} -> %{nextStat4}" +
            $"\n{statName5}: %{curStat5} -> %{nextStat5}";
    }
    #endregion
}
