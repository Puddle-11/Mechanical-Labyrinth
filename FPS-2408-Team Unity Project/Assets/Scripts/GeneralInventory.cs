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


    public ItemType[] Hotbar;

    public Image[] HotBarImages;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Hotbar = new ItemType[numOfslots];

        HotBarImages = new Image[numOfslots];
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
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            selectedSlot = (selectedSlot + 1) % Hotbar.Length;
            changeItem();
        }
        //going down
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            selectedSlot = (selectedSlot + 1) % Hotbar.Length;
            changeItem();
        }
    }
    void changeItem()
    {
        //
        ItemType item = Hotbar[selectedSlot];

        if (item == null)
        {
            return;
        }
        //guns
        if ((int)item.type == 0)
        {
           GameManager.instance.playerControllerRef.GetPlayerHand().PickupItem(item, null);
        }
    }
}
