using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Custom Assets/Item")]
public class ItemType : ScriptableObject
{
    public string itemName;
    public int itemID;
    public GameObject Object;
    public GameObject Pickup;
    public int maxUses;
    public Sprite Icon;
    public Pickup.PStats defaultStats;
}
