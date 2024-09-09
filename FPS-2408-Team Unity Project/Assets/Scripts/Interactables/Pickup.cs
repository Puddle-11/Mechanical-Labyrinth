using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemType Item;
    [SerializeField] private string stats;
     public int uses = -1;
    public string GetStats()
    {
        IUsable temp;
        if(Item.Object.TryGetComponent<IUsable>(out temp))
        {
            return Item.itemName + "\n"+temp.GetItemStats();
        }
        return stats;
    }
    public void TriggerInteraction()
    {
        PlayerHand hand = GameManager.instance.playerControllerRef.GetPlayerHand();
        hand.PickupItem(Item, this);
    }

    public ItemType GetItem() { return Item;}
}
