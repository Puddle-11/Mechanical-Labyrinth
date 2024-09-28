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
    public int S_currentScrap;
    public int S_Level;
    public UInt64 S_TotalDamage;
    public int S_TotallEnemiesKilled;
    public int S_TotalDeaths;
    [SerializeField] private int S_DefaultHealth;
    public int S_CurrentHealth;
    public float S_currentTime;

    public void ResetStats(CurrentStats _default)
    {
        ResetPerRunStats(_default);
        S_TotalDeaths = _default.S_TotalDeaths;
    
    }
    public void ResetPerRunStats(CurrentStats _default)
    {
        S_GeneralInventory = _default.S_GeneralInventory;
        S_AmmoInventory = _default.S_AmmoInventory;
        S_Level = _default.S_Level;
        S_currentScrap = _default.S_currentScrap;
        S_TotalDamage = _default.S_TotalDamage;
        S_TotallEnemiesKilled = _default.S_TotallEnemiesKilled;
        S_CurrentHealth = _default.S_CurrentHealth;
        S_currentTime = _default.S_currentTime;
    }
}
