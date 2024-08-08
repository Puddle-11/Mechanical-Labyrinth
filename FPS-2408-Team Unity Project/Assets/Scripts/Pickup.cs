using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private ItemType Item;
    public ItemType GetItem()
    {
        if (Item == null) return null;
        return Item;
    }
    public ItemType PickupItem()
    {
        if (Item == null)
        {
            Debug.LogWarning("Failed to pickup\n ItemType variable on " + gameObject.name + " Unassigned");
            return null;
        }
        Destroy(gameObject);
        return Item;
    }

}
