using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public LayerMask projectileIgnore;
    public static GameManager instance;


    [HideInInspector] public GameObject playerRef;
    [HideInInspector] public PlayerController playerControllerRef;
    [SerializeField] private TMP_Text enemyCountField;
    [SerializeField] GameObject nextLevelText;
    [SerializeField] GameObject enemyCountObject;
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
        enemyCountObject.SetActive(true);
        enemyCount += _amount;
        enemyCountField.text = enemyCount.ToString("F0");

        if (enemyCount <= 0)
        {
            enemyCountObject.SetActive(false);
            nextLevelText.SetActive(true);
        }
    }
}
