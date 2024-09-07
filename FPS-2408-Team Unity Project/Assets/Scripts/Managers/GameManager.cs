using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public LayerMask projectileIgnore;
    public LayerMask penetratingIgnore;
    public static GameManager instance;
    public CurrentStats currentStats;
    public GameObject playerRef;
    public PlayerController playerControllerRef;
    private int enemyCount;
    private bool isPause = false;
    public delegate void OnWin();
    public OnWin levelWon;
    [SerializeField] private ItemType defaultItem;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (BootLoadManager.instance != null) currentStats = BootLoadManager.instance.GetSave();
        }
        else
        {
            Debug.LogWarning("Found multiple game managers in scene\nDestroyed Game Manager at " + gameObject.name);
            Destroy(this);
        }
    }
    public void respawn()
    {

    }
    public void ResetCurrentHealth()
    {
        currentStats.S_CurrentHealth = playerControllerRef.GetMaxHealth();
    }
    public void MoveToRespawn()
    {
        playerControllerRef.SpawnPlayer();
    }
    private void OnEnable()
    {

        if (BootLoadManager.instance != null)
        {
            BootLoadManager.instance.stopLoadEvent += MoveToRespawn;
            BootLoadManager.instance.startLoadEvent += ResetGame;

        }
    }
    private void OnDisable()
    {
        if (BootLoadManager.instance != null)
        {
            BootLoadManager.instance.stopLoadEvent -= MoveToRespawn;
            BootLoadManager.instance.startLoadEvent -= ResetGame;

        }
    }
   
    private void Start()
    {

        if(currentStats != null) currentStats.isActive = true;
        if (TryFindPlayer(out playerRef))
        {
            playerRef.TryGetComponent<PlayerController>(out playerControllerRef);
            if (playerControllerRef != null) respawn();
        }
    }
    public void ResetAllStats()
    {
        //GeneralInventory.ItemSlot[]
        currentStats.S_GeneralInventory = new GeneralInventory.ItemSlot[0];
        currentStats.S_AmmoInventory = new int[0];
        currentStats.S_TotalDamage = 0;
        currentStats.S_TotallEnemiesKilled = 0;
        currentStats.S_Item = defaultItem;
        currentStats.isActive = false;
        currentStats.S_Level = 0;
    }
    private void Update()
    {
        if (BootLoadManager.instance != null && BootLoadManager.instance.IsLoading())
        {
            if(playerRef.activeInHierarchy == true)
            {
                playerRef.SetActive(false);
            }
            //THIS SHOULD BE THE ONLY SPOT OUTSIDE OF BOOTLOADER THAT STOPLOADEVENT IS INVOKED
            //=====================================================
            //Add more conditions to this if you want to wait until after a function is complete to load the level
            if (GetChunkGrid() == null) BootLoadManager.instance.stopLoadEvent.Invoke();
            else
            {
                if (GetChunkGrid().GetProgress() == 1)BootLoadManager.instance.stopLoadEvent.Invoke();
                else BootLoadManager.instance.UpdateLoadingBar(GetChunkGrid().GetProgress());
            }
            //=====================================================
        }
        else
        {
            if (playerRef.activeInHierarchy == false)
            {
                playerRef.SetActive(true);
            }
           if(currentStats != null) currentStats.S_currentTime += Time.deltaTime;
        }
    }
    public bool GetStatePaused(){return isPause;}
    public void SetPause(bool _val)
    {
        isPause = _val;
        Time.timeScale = _val ? 0 : 1;
        if (_val != true) return;
        for (int i = 0; i < UIManager.instance.ConstUI.Length; i++)
        {
            if (UIManager.instance.ConstUI[i].CUI_obj != null)
            {
                UIManager.instance.ConstUI[i].CUI_currentState = UIManager.instance.ConstUI[i].CUI_obj.activeInHierarchy;
                UIManager.instance.ConstUI[i].CUI_obj.SetActive(false);
            }
        }
    }
    public void UpdateDeathCounter(int _val)
    {
        currentStats.S_TotalDeaths += _val;
        UIManager.instance.SetAttemptNumber(currentStats.S_TotalDeaths);
    }
    public void UpdateKillCounter(int _val){SetKillCounter(currentStats.S_TotallEnemiesKilled + _val);}
    public void SetKillCounter(int _val)
    {
        currentStats.S_TotallEnemiesKilled = _val;
        UIManager.instance.SetEnemiesKilled(currentStats.S_TotallEnemiesKilled);
    }
    public void UpdateDamageDealt(int _val)
    {
       if(currentStats != null) currentStats.S_TotalDamage += (UInt64)_val;
       if(UIManager.instance != null) UIManager.instance.SetDamageDealt(currentStats != null ? currentStats.S_TotalDamage : 0);
    }
    public int GetCurrentLevel(){ return currentStats != null ? currentStats.S_Level : 0;}
    public void SetCurrentLevel(int _val) { if (currentStats != null) currentStats.S_Level = _val; }
    public void UpdateCurrentLevel(int _val){SetCurrentLevel(GetCurrentLevel() + _val);}
    public void SetCurrentItem(ItemType _item) {currentStats.S_Item = _item;}
    public void SetAmmoInventory(int[] _arr) {currentStats.S_AmmoInventory = _arr;}
    public int[] GetAmmoInventory() { return currentStats != null ? currentStats.S_AmmoInventory : new int[0]; }
    public ItemType GetCurrentItemType() {return currentStats != null ? currentStats.S_Item : null;}

    //in progress
    public void SetGeneralInventory(GeneralInventory.ItemSlot[] _arr) { currentStats.S_GeneralInventory = _arr; }
    public GeneralInventory.ItemSlot[] GetGeneralInventory() { return currentStats.S_GeneralInventory; }
    //not complete yet


    private ChunkGrid GetChunkGrid() {return ChunkGrid.instance;}
    public void SetCurrentHealth(int _val) { if (currentStats != null) currentStats.S_CurrentHealth = _val; }
    public int GetCurrentHealth()
    {
        return (currentStats == null || currentStats.S_CurrentHealth <= 0) ? int.MaxValue : currentStats.S_CurrentHealth;
    }

    public bool TryFindPlayer(out GameObject _ref)
    {
        GameObject tempref = GameObject.FindWithTag("Player");
        if (tempref != null)
        {
            _ref = tempref;
            return true;
        }
        _ref = null;
        return false;
    }

    public void ResetGame()
    {
        UIManager.instance.ResetTempUI();
        UIManager.instance.SetDamageDealt(currentStats != null? currentStats.S_TotalDamage:0);
        UIManager.instance.SetEnemiesKilled(currentStats != null ? currentStats.S_TotallEnemiesKilled: 0);
        UIManager.instance.SetAttemptNumber(currentStats != null ? currentStats.S_TotalDeaths: 0);
        if (currentStats != null && currentStats.isActive) SetCurrentItem(playerControllerRef.GetCurrentItemType());
        enemyCount = 0;
        UIManager.instance.ToggleWinMenu(false);
        UIManager.instance.SetEnemyCount(enemyCount);
    }
    public void updateGameGoal(int _amount)
    {
        enemyCount += _amount;
        UIManager.instance.SetEnemyCount(enemyCount);

        if (enemyCount <= 0)
        {
            levelWon?.Invoke();
            UIManager.instance.ToggleWinMenu(true);
        }
    }

}
