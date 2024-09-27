using System.Collections.Generic;

using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    private float warningTimer = 0;
    [SerializeField] private float warningHangTime;
    [SerializeField] private GameObject insuficientFunds;
    [SerializeField] private List<GameObject> allBuyButtons;

    [SerializeField] private shopItem[] shopItemList;
    [SerializeField] private shopAmmo[] shopAmmoList;

    [Space]
    [Header("Pistols")]
    [Space]
    [SerializeField] private ItemType deagle;
    [SerializeField] private ItemType glock;
    [SerializeField] private ItemType shotgunPistol;
    [SerializeField] private ItemType classic;
    [SerializeField] private ItemType uzi;
    [Space]
    [Header("Assault Rifles")]
    [Space]
    [SerializeField] private ItemType mp5;
    [SerializeField] private ItemType scar;
    [SerializeField] private ItemType m4a1;
    [SerializeField] private ItemType mp7;
    [Space]
    [Header("Snipers")]
    [Space]
    [SerializeField] private ItemType m16;
    [Space]
    [Header("Items")]
    [Space]
    [SerializeField] private ItemType healthPack;


    [System.Serializable]
    public struct shopItem
    {
        public string Name;
        public ItemType t;
        public Sprite CustomIcon;
        public int price;

        public Image Icon;
        public TMP_Text priceText;

    }
    [System.Serializable]
    public struct shopAmmo
    {
        public AmmoInventory.bulletType t;
        public int price;
        public int quantity;

        public Image Icon;
        public TMP_Text description;
    }
    private void Update()
    {
        if(warningTimer > 0)
        {
            warningTimer -= Time.unscaledDeltaTime;
            SetWarningState(true);

        }
        else
        {
            warningTimer = 0;
            SetWarningState(false);
        }
    }
    private void Start()
    {
        SetUpAmmoShop();
        SetUpItemShop();
    }
    public void SetUpAmmoShop()
    {
        for (int i = 0; i < shopAmmoList.Length; i++)
        {
           if(shopAmmoList[i].description != null) shopAmmoList[i].description.text = "Price: " + shopAmmoList[i].price.ToString()+ "\n\nQuantity: " + shopAmmoList[i].quantity.ToString();
            if (shopAmmoList[i].Icon != null) shopAmmoList[i].Icon.sprite =  AmmoInventory.instance.GetAmmoIcon(shopAmmoList[i].t);
        }
    }
    public void SetUpItemShop()
    {
        for (int i = 0; i < shopItemList.Length; i++)
        {
            if (shopItemList[i].priceText != null)  shopItemList[i].priceText.text = "Price: " + shopItemList[i].price.ToString();
            if (shopItemList[i].Icon != null && shopItemList[i].t != null) shopItemList[i].Icon.sprite = shopItemList[i].CustomIcon == null ? shopItemList[i].t.Icon : shopItemList[i].CustomIcon;
        }
    }



    public void BuyItem(int index)
    {
        if (index < 0 || index >= shopItemList.Length) return;

        if (ScrapInventory.instance.currentScrap >= shopItemList[index].price)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(shopItemList[index].t, result);
                ScrapInventory.instance.RemoveScrap(shopItemList[index].price);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }
    public void BuyAmmo(int index)
    {
        if (index < 0 || index >= shopAmmoList.Length) return;
        if (ScrapInventory.instance.currentScrap >= shopAmmoList[index].price)
        {
            AmmoInventory.instance.UpdateAmmoInventory(shopAmmoList[index].t, 20);
            ScrapInventory.instance.RemoveScrap(shopAmmoList[index].price);
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void ResetWarningTimer()
    {
        SetWarningTimer( warningHangTime);
    }
    private void OnDisable()
    {
        warningTimer = 0;
    }
    public void SetWarningTimer(float _time)
    {
        warningTimer = _time;
    }
    public void SetWarningState(bool _val)
    {
        if(insuficientFunds != null) insuficientFunds.gameObject.SetActive(_val);
    }
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
        TurnOffBuyButtons();
        UIManager.instance.CloseShop();
    }

    public void GunShop()
    {
        TurnOffBuyButtons();
        UIManager.instance.GunShop();
    }

    public void AmmoShop()
    {
        TurnOffBuyButtons();
        UIManager.instance.AmmoShop();
    }

    public void ItemShop()
    {
        TurnOffBuyButtons();
        UIManager.instance.ItemShop();
    }

    public void PrimaryShop()
    {
        TurnOffBuyButtons();
        UIManager.instance.PrimaryShop();
    }

    public void SecondaryShop() 
    {
        TurnOffBuyButtons();
        UIManager.instance.SecondaryShop();
    }

    public void SniperShop()
    {
        TurnOffBuyButtons();
        UIManager.instance.SniperShop();
    }

    //Ammo
    public void BuyPistolAmmo()
    {
        if (ScrapInventory.instance.currentScrap >= 5)
        {
            AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Pistol, 20);
            ScrapInventory.instance.RemoveScrap(5);
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyAssaultAmmo()
    {
        if (ScrapInventory.instance.currentScrap >= 7)
        {
            AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Assualt, 20);
            ScrapInventory.instance.RemoveScrap(5);
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyShotgunAmmo()
    {
        if (ScrapInventory.instance.currentScrap >= 10)
        {
            AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Shotgun, 20);
            ScrapInventory.instance.RemoveScrap(5);
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuySniperAmmo()
    {
        if (ScrapInventory.instance.currentScrap >= 15)
        {
            AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Sniper, 20);
            ScrapInventory.instance.RemoveScrap(7);
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyExplosiveAmmo()
    {
        if (ScrapInventory.instance.currentScrap >= 20)
        {
            AmmoInventory.instance.UpdateAmmoInventory(AmmoInventory.bulletType.Explosive, 20);
            ScrapInventory.instance.RemoveScrap(5);
        }
        else
        {
            ResetWarningTimer();
        }
    }

    //Pistols

    public void BuyDeagle()
    {
        if (ScrapInventory.instance.currentScrap >= 75)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(deagle, result);
                ScrapInventory.instance.RemoveScrap(15);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyGlock()
    {
        if (ScrapInventory.instance.currentScrap >= 60)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(glock, result);
                ScrapInventory.instance.RemoveScrap(10);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyShotgunPistol()
    {
        if (ScrapInventory.instance.currentScrap >= 30)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(shotgunPistol, result);
                ScrapInventory.instance.RemoveScrap(15);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyClassic()
    {
        if (ScrapInventory.instance.currentScrap >= 15)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(classic, result);
                ScrapInventory.instance.RemoveScrap(5);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyUZI()
    {
        if (ScrapInventory.instance.currentScrap >= 45)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(uzi, result);
                ScrapInventory.instance.RemoveScrap(15);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    //Snipers

    public void BuyM16()
    {
        if (ScrapInventory.instance.currentScrap >= 120)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(m16, result);
                ScrapInventory.instance.RemoveScrap(30);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    //Assault Rifles

    public void BuyScar()
    {
        if (ScrapInventory.instance.currentScrap >= 60)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(scar, result);
                ScrapInventory.instance.RemoveScrap(25);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyM4A1()
    {
        if (ScrapInventory.instance.currentScrap >= 50)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(m4a1, result);
                ScrapInventory.instance.RemoveScrap(35);
                Debug.Log(result);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyMP5()
    {
        if (ScrapInventory.instance.currentScrap >= 80)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(mp5, result);
                ScrapInventory.instance.RemoveScrap(35);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyMP7()
    {
        if (ScrapInventory.instance.currentScrap >= 25)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(mp7, result);
                ScrapInventory.instance.RemoveScrap(25);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void BuyHealthPack()
    {
        if (ScrapInventory.instance.currentScrap >= 200)
        {
            if (GeneralInventory.instance.GetNextFreeIndex(out int result))
            {
                GeneralInventory.instance.AddItemToInventory(healthPack, result);
                ScrapInventory.instance.RemoveScrap(100);
            }
        }
        else
        {
            ResetWarningTimer();
        }
    }

    public void TurnOffBuyButtons()
    {
        foreach (GameObject button in allBuyButtons)
        {
            button.SetActive(false);
        }
    }
}
