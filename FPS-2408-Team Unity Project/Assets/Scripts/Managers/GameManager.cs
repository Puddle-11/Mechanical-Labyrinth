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
    private void Start()
    {
        playerRef = GameObject.FindWithTag("Player");
        playerControllerRef = playerRef.GetComponent<PlayerController>();
    }

    public void updateGameGoal(int _amount)
    {
        enemyCount += _amount;
        UIManager.instance.SetEnemyCount(enemyCount);

        if (enemyCount <= 0)
        {
            //UIManager.instance.ToggleEnemyCount(false);
            UIManager.instance.ToggleWinMenu(true);
        }
    }
    public void RespawnPlayer()
    {
        //playerControllerRef.ResetHealth();
        //playerControllerRef.SpawnPlayer();
        //UIManager.instance.StateUnpause();
    }
}
