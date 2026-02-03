using UnityEngine;

public abstract class WeaponModification : Modification
{
    public abstract float GetModifyValue(WeaponType weapon, StatType stat);
}
