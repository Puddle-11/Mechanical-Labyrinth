using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    private TMP_Text insuficientFunds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void OpenShop()
    {
        UIManager.instance.OpenShop();
    }

    public void CloseShop() 
    {
        UIManager.instance.CloseShop();
    }

    public void GunShop()
    {
        UIManager.instance.GunShop();
    }

    public void AmmoShop()
    {
        UIManager.instance.AmmoShop();
    }

    public void ItemShop()
    {
        UIManager.instance.ItemShop();
    }

    public void PrimaryShop()
    {
        UIManager.instance.PrimaryShop();
    }

    public void SecondaryShop() 
    {
        UIManager.instance.SecondaryShop();
    }

    public void YouArePoor(GameObject text)
    {
        if (ScrapInventory.instance.currentScrap >= 5)
        {
            AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Pistol, 30);
        }
        else
        {
            StartCoroutine(UIManager.instance.YouArePoor(text));
        }
    }
}
