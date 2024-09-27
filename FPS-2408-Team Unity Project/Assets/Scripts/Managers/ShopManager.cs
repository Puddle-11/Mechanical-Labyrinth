using System.Collections.Generic;

using TMPro;
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
  

    public void TurnOffBuyButtons()
    {
        foreach (GameObject button in allBuyButtons)
        {
            button.SetActive(false);
        }
    }
}
