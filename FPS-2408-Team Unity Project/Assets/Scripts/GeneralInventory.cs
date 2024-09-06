using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralInventory : MonoBehaviour
{
    public struct Itemslot {
        public ItemType item;
        public int postion;
    }
    //class called itemtype gives type of item
    //item name, item id, pickup object, actual object, and max uses.

    //make list of ItemSlot names Hotbar.


    public static GeneralInventory instance;

    [Header("Inventory Variables")]
    [Space]
    [SerializeField] private int numOfslots;


    public Itemslot[] Hotbar = new Itemslot[10];

    // Start is called before the first frame update
    //void Awake()
    //{
    //    if (instance == null)
    //        instance = this;
    //    else
    //        Destroy(this);

    //}
    //private void Start()
    //{
    //    if (GameManager.instance != null)
    //    {
    //        int[] temp = GameManager.instance.GetAmmoInventory();
    //        if (temp != null && temp.Length > 0)
    //        {
    //            for (int i = 0; i < temp.Length; i++)
    //            {
    //                if (i >= ammoCounts.Length) break;
    //                ammoCounts[i] = temp[i];
    //            }
    //        }
    //    }

    //}
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
    //public void UpdateAmmoInventory(bulletType type, int amount)
    //{
    //    ammoCounts[(int)type] += amount;
    //    UIManager.instance.UpdateInternalAmmoInv(type);
    //    if (GameManager.instance != null) GameManager.instance.SetAmmoInventory(ammoCounts);
    //}
}
