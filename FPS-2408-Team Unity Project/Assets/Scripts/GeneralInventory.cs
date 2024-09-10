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
        SelectItem();
    }

    void SelectItem()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //if at top of list, loop to bottom
            selectedSlot = (selectedSlot + 1) % Hotbar.Length;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //if at bottom of list, loop to top
            selectedSlot = selectedSlot <= 0 ? Hotbar.Length - 1 : selectedSlot - 1;
        }
            GameManager.instance.playerControllerRef.GetPlayerHand().ToggleADS(false);
        ChangeItem();
    }
    void ChangeItem()
    {
        ItemType item = Hotbar[selectedSlot].t;

        if (item == null)
        {
            GameManager.instance.playerControllerRef.GetPlayerHand().DropItem();
            return;
        }
        //guns
        GameManager.instance.playerControllerRef.GetPlayerHand().PickupItem(item, null, true);
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
