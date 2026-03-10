using UnityEngine;

public class UIPermanent : MonoBehaviour
{
    [SerializeField, Tooltip("The name of the upgrade")]
    private string upgradeName;
    [SerializeField, Tooltip("The description of what the upgrade does, this doesn't change")]
    private string baseUpgradeDescription;
    [SerializeField, Tooltip("The description of how the upgrade improves per level")]
    private string finalUpgradeDescription;
    [SerializeField, Tooltip("The icon representing the upgrade")]
    private Sprite upgradeIcon;

    // Properties
    public string UpgradeName => upgradeName;
    public string BaseUpgradeDescription => baseUpgradeDescription;
    public string FinalUpgradeDescription => finalUpgradeDescription;
    public Sprite UpgradeIcon => upgradeIcon;

    public void UpdateDescription(int level, float curStat, float nextStat)
    {
        // If level is 0 display curStat as 0
        if (level == 0)
        {
            curStat = 0;
        }
        // Display the current stats and the next level stats
        finalUpgradeDescription = $"{baseUpgradeDescription}\n%{curStat} -> %{nextStat}";
    }
}
