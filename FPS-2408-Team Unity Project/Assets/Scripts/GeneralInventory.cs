using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class GeneralInventory : MonoBehaviour
{
    //make list of ItemSlot names Hotbar.
    public int selectedSlot;

    //List of ItemSlots

    //array of hotbar UI elements


    public static GeneralInventory instance;

    [Header("Inventory Variables")]
    [Space]
    [SerializeField] private int numOfslots;


    public HotbarSlot[] Hotbar;


    public struct HotbarSlot
    {
        public ItemType t;
        public Image UIImage;
        public GameObject obj;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Hotbar = new HotbarSlot[numOfslots];
    }
    private void Start()
    {
        //Getting save if there is one
        if (GameManager.instance != null)
        {
            ItemType[] temp = GameManager.instance.GetGeneralInventory();
            if (temp != null && temp.Length > 0)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (i >= Hotbar.Length) break;
                    Hotbar[i].t = temp[i];
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
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            selectedSlot = (selectedSlot + 1) % Hotbar.Length;
            GameManager.instance.playerControllerRef.GetPlayerHand().ToggleADS(false);
            changeItem();
        }
        //going down
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            selectedSlot = selectedSlot <= 0 ? Hotbar.Length - 1 : selectedSlot - 1;
            GameManager.instance.playerControllerRef.GetPlayerHand().ToggleADS(false);
            changeItem();
        }
    }
    void changeItem()
    {
        //
        ItemType item = Hotbar[selectedSlot].t;

        if (item == null)
        {
            GameManager.instance.playerControllerRef.GetPlayerHand().DropItem();
            return;
        }
        //guns
        if ((int)item.type != 0)
        {
           GameManager.instance.playerControllerRef.GetPlayerHand().PickupItem(item, null, true);
        }
    }
    public void SetSlot(int _index, ItemType t)
    {
        Hotbar[_index].t = t;
    }
    public void SetSlot(ItemType t)
    {
        SetSlot(selectedSlot, t);
    }
}
