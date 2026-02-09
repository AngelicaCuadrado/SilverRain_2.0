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
    [Tooltip("The item (Weapon, Upgrade or Modification) assigned to this buff card")]
    private ITemporary assignedBuff;

    //Events
    public UnityEvent<ITemporary> OnBuffCardClicked;

    public void OnCardClicked()
    {
        OnBuffCardClicked?.Invoke(assignedBuff);
    }
    public void SetupCard(ITemporary buffToAssign)
    {
        assignedBuff = buffToAssign;
        UITemporary buffInfo = buffToAssign.UIData;
        buffName.text = buffInfo.BuffName;
        buffLevel.text = buffInfo.BuffLevel;
        buffDescription.text = buffInfo.FinalBuffDescription;
        buffIcon.sprite = buffInfo.BuffIcon;
    }
}
