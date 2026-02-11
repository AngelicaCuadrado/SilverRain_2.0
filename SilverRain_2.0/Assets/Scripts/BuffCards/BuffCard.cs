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
        BuffCardManager.Instance.ChooseBuffCard(assignedBuff);
    }
    public void SetupCard(TemporaryBuff buffToAssign)
    {
        //Assign the buff and get the UI data
        assignedBuff = buffToAssign;
        UITemporary buffInfo = buffToAssign.UIData;

        // Put the data in the corresponding field
        buffName.text = buffInfo.BuffName;
        buffLevel.text = buffInfo.BuffLevel;
        buffDescription.text = buffInfo.FinalBuffDescription;
        buffIcon.sprite = buffInfo.BuffIcon;
    }
}
