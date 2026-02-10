using UnityEngine;

public interface IStatModifier
{
    public abstract float GetModifyValue(StatType type);
}
