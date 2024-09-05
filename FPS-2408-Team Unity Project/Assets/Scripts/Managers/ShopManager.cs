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
}
