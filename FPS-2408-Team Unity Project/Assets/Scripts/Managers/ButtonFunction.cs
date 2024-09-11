using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{

    public void resume()
    {
        UIManager.instance?.StateUnpause();
    }
    public void restart()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetCurrentHealth();
            GameManager.instance.playerControllerRef.SetHealth(GameManager.instance.GetCurrentHealth());
        }
        BootLoadManager.instance?.LoadGameScene(SceneManager.GetActiveScene().name);
        UIManager.instance?.StateUnpause();
    }
    public void quit()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.GetCurrentHealth() == 0) GameManager.instance.ResetCurrentHealth();
        }
        Time.timeScale = 1;
        BootLoadManager.instance?.ExitGameMode();
    }

    public void respawn()
    {
        GameManager.instance?.respawn();
        GameManager.instance?.MoveToRespawn();
    }

    public void closeShop()
    {
        ShopManager.instance?.CloseShop();
    }

    public void openGunShop()
    {
        ShopManager.instance?.GunShop();
    }

    public void openAmmoShop()
    {
        ShopManager.instance?.AmmoShop();
    }

    public void openItemShop()
    {
        ShopManager.instance?.ItemShop();
    }

    public void back(GameObject menu)
    {
        UIManager.instance?.Back(menu);
    }

    public void openPrimaryShop()
    {
        ShopManager.instance?.PrimaryShop();
    }

    public void openSecondaryShop() 
    {
        ShopManager.instance?.SecondaryShop();
    }

    public void buyPistolAmmo(GameObject text)
    {
        ShopManager.instance.ResetWarningTimer();
    }
  

    public void buyAssaultAmmo()
    {
        AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Assualt, 30);
    }

    public void buyShotgunAmmo()
    {
        AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Shotgun, 30);
    }

    public void buySniperAmmo()
    {
        AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Sniper, 30);
    }

    public void buyExplosiveAmmo()
    {
        AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Explosive, 30);
    }

    public void viewButton(GameObject view)
    {
        view.SetActive(true);
    }

}
