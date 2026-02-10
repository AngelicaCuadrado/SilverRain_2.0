using UnityEngine;

public interface ITemporary : IUpgradeable
{
    UITemporary UIData {  get; }
    void SetAvailable(bool isAvailable);
}
