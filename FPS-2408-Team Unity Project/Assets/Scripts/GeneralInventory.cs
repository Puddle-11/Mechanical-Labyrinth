using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class GeneralInventory : MonoBehaviour
{
    public struct ItemSlot {
        public ItemType item;
        public int postion;
    }
    //class called itemtype gives type of item
    //item name, item id, pickup object, actual object, and max uses.

    //make list of ItemSlot names Hotbar.
    public int selectedSlot;



    //List of ItemSlots

    //array of hotbar UI elements


    public static GeneralInventory instance;

    [Header("Inventory Variables")]
    [Space]
    [SerializeField] private int numOfslots;


    public ItemSlot[] Hotbar = new ItemSlot[10];

    public Image[] HotBarImages = new Image[10];

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        //Getting save if there is one
        if (GameManager.instance != null)
        {
            ItemSlot[] temp = GameManager.instance.GetGeneralInventory();
            if (temp != null && temp.Length > 0)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (i >= Hotbar.Length) break;
                    Hotbar[i] = temp[i];
                }
            }
        }

    }

    public void Update()
    {
        selectItem();
    }
    void selectItem()
    {
        //scrolling up
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedSlot < Hotbar.Length - 1)
        {
            selectedSlot++;
            changeItem();
        }
        //going down
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedSlot > 0)
        {
            selectedSlot--;
            changeItem();
        }
    }
    void changeItem()
    {
        ItemType item = Hotbar[selectedSlot].item;
        //guns
        if ((int)item.type == 0)
        {
           GameManager.instance.playerControllerRef.GetPlayerHand().PickupItem(item ,null);

        }


        //useable items


        //heal items

    }

    //public int GetAmmoTypeCount()
    //{
    //    return numOfAmmoTypes;
    //}
    //// Update is called once per frame

    //public int GetAmmoAmount(bulletType type)
    //{
    //    return GetAmmoAmount((int)type);
    //}
    //public int GetAmmoAmount(int _index)
    //{
    //    return ammoCounts[_index];

    //}
    //public Sprite GetAmmoIcon(bulletType type)
    //{
    //    return GetAmmoIcon((int)type);
    //}
    //public Sprite GetAmmoIcon(int _index)
    //{
    //    if (_index < typeIcons.Length)
    //    {
    //        return typeIcons[_index];
    //    }
    //    else
    //    {
    //        return null;
    //    }

    //}
    //public void UpdateGeneralInventory(ItemSlot item)
    //{
    //    //Hotbar[(int)type] += amount;
    //    //UIManager.instance.UpdateInternalAmmoInv(type);
    //    //if (GameManager.instance != null) GameManager.instance.SetAmmoInventory(ammoCounts);
    //}
}
