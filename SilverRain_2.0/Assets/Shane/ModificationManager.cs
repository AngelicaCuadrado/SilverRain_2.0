using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ModificationManager : MonoBehaviour
{
    List<Modification> allModifications;
    List<Modification> currentModifications;

    public static ModificationManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

   public void AddModification(Modification modification) 
    {
        if (!currentModifications.Contains(modification)) 
        {
            currentModifications.Add(modification);
        }
    }

   private void ResetModifications()
    {
        currentModifications.Clear();
    }

    float GetStatModifications(StatType type) 
    {

    }
}
