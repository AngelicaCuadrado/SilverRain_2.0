using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BuffCardManager : MonoBehaviour
{
    [Header("Choices settings")]
    [SerializeField, Tooltip("The choices currently offered for level up")]
    private List<BuffCard> currentChioces = new();
    [SerializeField, Tooltip("All available choices that may be offered on level up")]
    private List<ITemporary> availableChoices = new();
    [SerializeField, Tooltip("The amount of choices that will be offered on level up")]
    private int choiceAmount = 3;
    [Header("Buff Card Settings")]
    [SerializeField, Tooltip("Prefab for the buff card UI element")]
    private GameObject buffCardPrefab;
    [SerializeField, Tooltip("Parent where the buff card will spawn")]
    private Transform buffCardParent;
    [SerializeField, Tooltip("PlayerExperience component used to subscribe to LevelUp event")]
    private PlayerExperience playerExperience;

    private void Start()
    {
        if (playerExperience == null)
        {
            PlayerFinder.Instance.Player.TryGetComponent<PlayerExperience>(out PlayerExperience playerXP);
            if (playerXP != null)
            {
                playerExperience = playerXP;
                playerExperience.OnLevelUp.AddListener(DisplayBuffCards);
            }
        }

        WeaponManager.Instance.OnWeaponAvailabilityChange.AddListener(UpdateAvailableChoices);
        //StatManager.Instance.OnTempUpgradeAvailabilityChange.AddListener(UpdateAvailableChoices);
        //ModificationManager.Instance.OnModificationAvailabilityChange.AddListener(UpdateAvailableChoices);
    }

    private void DisplayBuffCards()
    {
        for (int i = 0; i < choiceAmount; i++)
        {
            //Create a new buff card
            BuffCard newCard = Instantiate(buffCardPrefab, buffCardParent).GetComponent<BuffCard>();
            //Randomly assign a buff to the card
            ITemporary buffToAssign = RandomiseChoices();
            newCard.SetupCard(buffToAssign);

            newCard.OnBuffCardClicked.AddListener(ChooseBuffCard);
            currentChioces.Add(newCard);
        }
    }

    private ITemporary RandomiseChoices()
    {
        return null;
    }

    private void ChooseBuffCard(ITemporary buffClicked)
    {
        switch (buffClicked)
        {
            case Weapon weapon:
                WeaponManager.Instance.AddWeapon(weapon.WeaponType);
                break;
            case TemporaryUpgrade upgrade:
                StatManager.Instance.AddTempUpgrade(upgrade.StatType);
                break;
            case Modification modification:
                ModificationManager.instance.AddModification(modification);
                break;
        }
    }

    private void UpdateAvailableChoices(ITemporary temp, bool isAvailable)
    {
        if (availableChoices.Contains(temp) && !isAvailable)
        {
            availableChoices.Remove(temp);
        }
        if (!availableChoices.Contains(temp) && isAvailable)
        {
            availableChoices.Add(temp);
        }
    }

    //Unsubscribe from events to prevent memory leaks
    private void OnDestroy()
    {
        if (playerExperience != null)
        {
            playerExperience.OnLevelUp.RemoveListener(DisplayBuffCards);
        }
        WeaponManager.Instance.OnWeaponAvailabilityChange.RemoveListener(UpdateAvailableChoices);
        //StatManager.Instance.OnTempUpgradeAvailabilityChange.RemoveListener(UpdateAvailableChoices);
        //ModificationManager.Instance.OnModificationAvailabilityChange.RemoveListener(UpdateAvailableChoices);
    }
    private void OnDisable()
    {
        if (playerExperience != null)
        {
            playerExperience.OnLevelUp.RemoveListener(DisplayBuffCards);
        }
        WeaponManager.Instance.OnWeaponAvailabilityChange.RemoveListener(UpdateAvailableChoices);
        //StatManager.Instance.OnTempUpgradeAvailabilityChange.RemoveListener(UpdateAvailableChoices);
        //ModificationManager.Instance.OnModificationAvailabilityChange.RemoveListener(UpdateAvailableChoices);
    }
}






//-------------------------------------------------------------DELETE THIS-------------------------------------------------------------


// public class TemporaryUpgrade : ITemporary
// {
//     public StatType StatType;
//     public void LevelUp()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public void ResetLevels()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public void SetAvailable(bool isAvailable)
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public void UpdateDescription()
//     {
//         throw new System.NotImplementedException();
//     }
// }
