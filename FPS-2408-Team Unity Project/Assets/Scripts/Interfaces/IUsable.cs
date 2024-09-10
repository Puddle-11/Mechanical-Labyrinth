using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsable
{
    public bool CanAim();
    public Pickup.PStats GetPStats();
    public void SetPStats(Pickup.PStats _val);
    public GameObject GetPickup();
    public void SetPickup(GameObject _val);
    public bool GetUsingItem();
    public void SetUsingItem(bool _val);
    public void UseItem();
    public string GetItemName();
    public string GetItemStats();
    public ItemType GetItemType();

}
