using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    [SerializeField, Tooltip("The weapon controller that is attached to the same weapon object")]
    private Weapon weapon;
    [SerializeField, Tooltip("Icon representing the weapon")]
    private Sprite weaponIcon;
    [SerializeField, Tooltip("Name of the weapon")]
    private string weaponName;
    [SerializeField, Tooltip("Description of the weapon stats at current level and at the next level")]
    private string weaponDescription;

    public void UpdateDescription()
    {

    }
}
