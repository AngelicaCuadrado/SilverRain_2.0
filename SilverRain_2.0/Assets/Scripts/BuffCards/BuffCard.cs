using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BuffCard : MonoBehaviour
{
    TMP_Text buffName;
    TMP_Text buffLevel;
    TMP_Text buffDescription;
    ITemporary assignedBuff;
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
