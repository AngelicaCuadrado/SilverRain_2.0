using UnityEngine;

public class ModificationFrenzy : Modification, IStatModifier
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetModifyValue(StatType type)
    {
        throw new System.NotImplementedException();
    }

}
