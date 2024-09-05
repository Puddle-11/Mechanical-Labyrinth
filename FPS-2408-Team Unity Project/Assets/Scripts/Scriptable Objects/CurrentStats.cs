using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(menuName ="Custom Assets/SaveFile")]
public class CurrentStats : ScriptableObject
{
    public int[] S_AmmoInventory;
    public int S_Level;
    public ItemType S_Item;
    public UInt64 S_TotalDamage;
    public int S_TotallEnemiesKilled;
    public int S_TotalDeaths;
    public bool isActive = false;
    public int S_CurrentHealth;
    public float S_currentTime;
}
