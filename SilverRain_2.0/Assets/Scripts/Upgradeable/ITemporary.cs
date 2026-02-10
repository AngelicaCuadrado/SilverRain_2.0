using UnityEngine;

public interface ITemporary : IUpgradeable
{
    public void SetAvailable(bool isAvailable);
    public UITemporary UIData {  get; }
}
