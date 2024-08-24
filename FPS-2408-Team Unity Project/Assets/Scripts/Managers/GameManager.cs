using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public LayerMask projectileIgnore;
    public LayerMask penetratingIgnore;
    public static GameManager instance;


    [HideInInspector] public GameObject playerRef;
    [HideInInspector] public PlayerController playerControllerRef;
    private int enemyCount;
    private ChunkGrid chunkGridRef;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Found multiple game managers in scene\nDestroyed Game Manager at " + gameObject.name);
            Destroy(this);
        }
    }

    public void respawn()
    {
        playerControllerRef.ResetHealth();
        playerControllerRef.spawnPlayer();
        
        UIManager.instance.StateUnpause();
    }
    private void OnEnable()
    {
        if (BootLoadManager.instance != null)
        {
            BootLoadManager.instance.stopLoadEvent += respawn;
            BootLoadManager.instance.startLoadEvent += ResetGameGoal;

        }
    }
    private void OnDisable()
    {
        if (BootLoadManager.instance != null)
        {
            BootLoadManager.instance.stopLoadEvent -= respawn;
            BootLoadManager.instance.startLoadEvent -= ResetGameGoal;

        }
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
                if (GetChunkGrid().progress == 1)BootLoadManager.instance.stopLoadEvent.Invoke();
                else BootLoadManager.instance.UpdateLoadingBar(GetChunkGrid().progress);
            }
            //=====================================================
        }
        else
        {
            if (playerRef.activeInHierarchy == false)
            {
                playerRef.SetActive(true);
            }
        }
    }
    private ChunkGrid GetChunkGrid()
    {
        if (ChunkGrid.instance == null) return null;


        return ChunkGrid.instance;
    }
    private void Start()
    {

        if (TryFindPlayer(out playerRef))
        {
            playerRef.TryGetComponent<PlayerController>(out playerControllerRef);
            if (playerControllerRef != null) respawn();
        }
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
    public void ResetGameGoal()
    {
        enemyCount = 0;
        UIManager.instance.SetEnemyCount(enemyCount);
    }
    public void updateGameGoal(int _amount)
    {
        enemyCount += _amount;
        UIManager.instance.SetEnemyCount(enemyCount);

        if (enemyCount <= 0)
        {
            UIManager.instance.ToggleWinMenu(true);
        }
    }

}
