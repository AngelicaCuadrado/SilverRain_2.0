using UnityEngine;

public abstract class StatModification : Modification
{
    public abstract float GetModifyValue(StatType type);
}
