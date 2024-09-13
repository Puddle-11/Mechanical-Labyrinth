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
        ShopManager.instance?.BuyPistolAmmo();
    }
  

    public void buyAssaultAmmo(GameObject text)
    {
        ShopManager.instance?.BuyAssaultAmmo();
    }

    public void buyShotgunAmmo(GameObject text)
    {
        ShopManager.instance?.BuyShotgunAmmo();
    }

    public void buySniperAmmo(GameObject text)
    {
        ShopManager.instance?.BuySniperAmmo();
    }

    public void buyExplosiveAmmo(GameObject text)
    {
        ShopManager.instance?.BuyExplosiveAmmo();
    }

    public void buyDeagle(GameObject text) 
    {
        ShopManager.instance?.BuyDeagle();
    }

    public void buyGlock(GameObject text)
    {
        ShopManager.instance?.BuyGlock();
    }

    public void buyClassic(GameObject text)
    {
        ShopManager.instance?.BuyClassic();
    }

    public void buyM16(GameObject text)
    {
        ShopManager.instance?.BuyM16();
    }

    public void buyScar(GameObject text)
    {
        ShopManager.instance?.BuyScar();
    }

    public void buyM4A1(GameObject text)
    {
        ShopManager.instance?.BuyM4A1();
    }

    public void buyRespawnItem(GameObject text)
    {
        ShopManager.instance?.BuyRespawnItem();
    }

    public void viewButton(GameObject view)
    {
        view.SetActive(true);
    }

}
