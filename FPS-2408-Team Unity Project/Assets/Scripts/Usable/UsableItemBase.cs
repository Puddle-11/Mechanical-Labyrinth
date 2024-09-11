using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItemBase : MonoBehaviour, IUsable
{
    [SerializeField] protected ItemType item;
    protected GameObject pickUp;
    public Pickup.PStats stats;
    protected bool usingItem;

    public virtual void Update()
    {
        if (usingItem)
        {
            UseItem();
        }
    }
    public virtual bool CanAim()
    {
        return false;
    }

    public virtual string GetItemName()
    {
        return item.name;
    }

    public  virtual string GetItemStats()
    {
        return "Nothing special here";
    }

    public virtual ItemType GetItemType()
    {
        return item;
    }

    public virtual GameObject GetPickup()
    {
        if (pickUp != null)
        {
            return pickUp;
        }
        else if (item != null && item.Pickup != null)
        {
            return item.Pickup;
        }
        else
        {
            Debug.LogWarning("No pickup available on " + gameObject.name);
            return null;
        }
    }

    public virtual Pickup.PStats GetPStats()
    {
        return stats;
    }

    public virtual bool GetUsingItem()
    {
        return usingItem;
    }

    public virtual void SetPickup(GameObject _val)
    {
        pickUp = _val;
    }

    public virtual void SetPStats(Pickup.PStats _val)
    {
        stats = _val;
    }

    public virtual void SetUsingItem(bool _val)
    {
        usingItem = _val;
    }

    public virtual void UseItem()
    {
        Debug.Log("Used Item " + gameObject.name);
    }
}
