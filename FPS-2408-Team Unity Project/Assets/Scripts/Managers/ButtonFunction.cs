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

    // Start is called before the first frame update

}
