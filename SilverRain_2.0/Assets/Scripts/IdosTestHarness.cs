using UnityEngine;

public class IdosTestHarness : MonoBehaviour
{
    //This test harness is used to test weapon functionality during development.
    //It needs to get key presses and call AddWeapon on the WeaponManager.
    public PlayerExperience playerExp;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponManager.Instance.AddWeapon(WeaponType.Pistol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponManager.Instance.AddWeapon(WeaponType.Sword);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WeaponManager.Instance.AddWeapon(WeaponType.Grenade);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //playerExp.OnLevelUp?.Invoke();
            playerExp.GainExp(playerExp.RequiredExp -  playerExp.CurrentExp);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            WeaponManager.Instance.AddWeapon(WeaponType.Hammer);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            WeaponManager.Instance.AddWeapon(WeaponType.Chakram);
        }
    }
}
