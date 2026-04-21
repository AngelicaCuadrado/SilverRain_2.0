using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuffCard : MonoBehaviour
{
    [SerializeField, Tooltip("The text element that will display the name of the buff")]
    private TMP_Text buffName;
    [SerializeField, Tooltip("The text element that will display the level of the buff")]
    private TMP_Text buffLevel;
    [SerializeField, Tooltip("The text element that will display the description of the buff")]
    private TMP_Text buffDescription;
    [SerializeField, Tooltip("The image element that will display the icon of the buff")]
    private Image buffIcon;
    [Tooltip("The buff (Weapon, Upgrade or Modification) assigned to this buff card")]
    private TemporaryBuff assignedBuff;

    public void OnCardClicked()
    {
        if (assignedBuff == null)
        {
            Debug.LogWarning("BuffCard - Clicked card has no assigned buff.");
            return;
        }
        BuffCardManager.Instance.ChooseBuffCard(assignedBuff);
    }

    public void OnCardBanned()
    {
        if (assignedBuff == null)
        {
            Debug.LogWarning("BuffCard - Tried to ban a card with no assigned buff.");
            return;
        }
        BuffCardManager.Instance.BanChoice(assignedBuff);
    }

    public void SetupCard(TemporaryBuff buffToAssign)
    {
        if (buffToAssign == null)
        {
            Debug.LogWarning("BuffCard - SetupCard received a null buff.");
            assignedBuff = null;
            gameObject.SetActive(false);
            return;
        }

        //Assign the buff and get the UI data
        assignedBuff = buffToAssign;
        UITemporary buffInfo = buffToAssign.UIData;
        if (buffInfo == null)
        {
            Debug.LogWarning($"BuffCard - Buff '{buffToAssign.name}' is missing UIData.");
            assignedBuff = null;
            gameObject.SetActive(false);
            return;
        }

        // Put the data in the corresponding field
        buffName.text = buffInfo.BuffName;
        buffLevel.text = buffInfo.BuffLevel;
        buffDescription.text = buffInfo.FinalBuffDescription;
        buffIcon.sprite = buffInfo.BuffIcon;
    }
}
