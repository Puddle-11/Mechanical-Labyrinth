using System;
using UnityEngine;

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
    [SerializeField] protected float maxtime;
    [SerializeField] private Texture[] colorPalettes;
    [SerializeField] private int levelColorChangeFrequency;
    [SerializeField] private Material colorMat;
    private int currPaletteIndex;
    private int maxEnemyCount;
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
        if (TryFindPlayer(out playerRef))
        {
            playerRef.TryGetComponent(out playerControllerRef);
        }
        UIManager.instance.InitializeInventory();
        GeneralInventory.instance.ImportInventory();

    }

    public void ResetAllStats()
    {
        currentStats.ResetPerRunStats();
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
            if (GetChunkGrid() == null) BootLoadManager.instance.EndSceneAnimation();
            else
            {
                if (GetChunkGrid().GetProgress() == 1)BootLoadManager.instance.EndSceneAnimation();
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
    public int GetGameGoal()
    {
        return enemyCount;
    }
    public bool GetStatePaused(){return isPause;}
    public float Getmaxtime() {
        return maxtime;
    }
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
    public void SetAmmoInventory(int[] _arr) {currentStats.S_AmmoInventory = _arr;}
    public int[] GetAmmoInventory() { return currentStats != null ? currentStats.S_AmmoInventory : new int[0]; }

    //in progress
    public void SetGeneralInventory(ItemType[] _arr) { currentStats.S_GeneralInventory = _arr; }
    public ItemType[] GetGeneralInventory() { return currentStats.S_GeneralInventory; }
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

        enemyCount = 0;
        maxEnemyCount = 0;

        UIManager.instance.ToggleWinMenu(false);
        UIManager.instance.SetEnemyCount(enemyCount);

        if (GetCurrentLevel() % levelColorChangeFrequency == 0)
        {
            int randomPaletteIndex = currPaletteIndex;
            while (randomPaletteIndex == currPaletteIndex)
            {
                randomPaletteIndex = UnityEngine.Random.Range(0, colorPalettes.Length);
            }
            currPaletteIndex = randomPaletteIndex;
            colorMat.SetTexture("_Palette", colorPalettes[currPaletteIndex]);
        }

    }

    public void updateGameGoal(int _amount)
    {
        if(_amount > 0) maxEnemyCount += _amount;
        enemyCount += _amount;
        UIManager.instance.SetEnemyCount((float)enemyCount / maxEnemyCount);

        if (enemyCount <= 0)
        {
            levelWon?.Invoke();
            UIManager.instance.ToggleWinMenu(true);
        }
    }

}
