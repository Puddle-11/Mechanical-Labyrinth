using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemType Item;
    [SerializeField] private string stats;
    public PStats currPStats;
    [System.Serializable]
    public struct PStats
    {
        public PStats(int _uses = -1, int _maxUses = 1)
        {
            uses = _uses;
            maxUses = _maxUses;
        }
        public int uses;
        public int maxUses;
    }
    public int GetUses()
    {
        return currPStats.uses;

    }
    public int GetMaxUses()
    {
        return currPStats.maxUses;
    }
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
