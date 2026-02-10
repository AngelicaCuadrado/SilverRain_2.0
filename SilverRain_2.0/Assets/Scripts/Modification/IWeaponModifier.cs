using UnityEngine;

public interface IWeaponModifier
{
    public abstract float GetModifyValue(WeaponType weapon, StatType stat);
}
