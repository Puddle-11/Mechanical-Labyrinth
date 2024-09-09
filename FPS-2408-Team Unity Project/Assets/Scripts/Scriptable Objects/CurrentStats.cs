using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(menuName = "Custom Assets/SaveFile")]
public class CurrentStats : ScriptableObject
{
    public ItemType[] S_GeneralInventory;
    public int[] S_AmmoInventory;
    public int S_Level;
    [SerializeField] private ItemType S_DefaultItem;
    public ItemType S_Item;
    public UInt64 S_TotalDamage;
    public int S_TotallEnemiesKilled;
    public int S_TotalDeaths;
    [SerializeField] private int S_DefaultHealth;
    public int S_CurrentHealth;
    public float S_currentTime;

    public void ResetStats()
    {
        S_GeneralInventory = new ItemType[0];
        S_AmmoInventory = new int[0];
        S_Level = 0;
        S_Item = S_DefaultItem;
        S_TotalDamage = 0;
        S_TotallEnemiesKilled = 0;
        S_TotalDeaths = 0;
        S_CurrentHealth = S_DefaultHealth;
        S_currentTime = 0;
    }
    public void ResetPerRunStats()
    {
        S_GeneralInventory = new ItemType[0];
        S_AmmoInventory = new int[0];
        S_Level = 0;
        S_Item = S_DefaultItem;
        S_TotalDamage = 0;
        S_TotallEnemiesKilled = 0;
        S_CurrentHealth = S_DefaultHealth;
        S_currentTime = 0;
    }
}
