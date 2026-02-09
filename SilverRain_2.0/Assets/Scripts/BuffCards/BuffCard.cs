using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuffCard : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private TMP_Text buffName;
    [SerializeField, Tooltip("")]
    private TMP_Text buffLevel;
    [SerializeField, Tooltip("")]
    private TMP_Text buffDescription;
    [SerializeField, Tooltip("")]
    private Image buffIcon;

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
    }
}
