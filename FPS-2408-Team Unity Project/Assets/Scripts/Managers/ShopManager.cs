using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

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
}
