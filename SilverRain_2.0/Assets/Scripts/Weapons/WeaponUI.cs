using UnityEngine;
/// <summary>
/// Provides user interface functionality for displaying weapon information and updating weapon descriptions in the
/// game.
/// </summary>
/// <remarks>This component should be attached to a GameObject representing a weapon. It interacts with the
/// associated weapon controller to present relevant weapon data, such as icon, name, and description, to the
/// player.</remarks>
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
