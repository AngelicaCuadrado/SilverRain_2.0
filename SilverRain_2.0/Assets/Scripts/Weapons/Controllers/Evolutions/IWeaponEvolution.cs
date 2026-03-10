using UnityEngine;

public interface IWeaponEvolution
{
    public void OnRequirementMet(WeaponType type);
    public void OnRequirementMet(StatType type);
    public void CheckRequirements();
}
