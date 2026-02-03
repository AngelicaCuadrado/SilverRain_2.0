using UnityEngine;

public abstract class WeaponModification : Modification
{
    public abstract float GetWeaponStatModification(WeaponType weapon, StatType stat);
}
