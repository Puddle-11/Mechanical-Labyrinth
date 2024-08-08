using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] GameObject menuActive;
    private bool isPause = false;

    public GameObject playerRef;
    public PlayerController playerControllerRef;
    [SerializeField] private TMP_Text enemyCountField;
    private int enemyCount;
    
    public bool GetStatePaused()
    {
        return isPause;
    }
    private void Awake()
    {
        if(instance == null)
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
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
            statePause();
            menuActive = menuPause;
            menuActive.SetActive(isPause);

            }
            else if(menuActive == menuPause)
            {

                stateUnpause();
                
            }
        }
    
    }
    public void statePause()
    {
        isPause = !isPause;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnpause()
    {
        isPause = !isPause;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPause);
        menuActive = null;
    }
    public void updateGameGoal(int _amount)
    {
        enemyCount += _amount;
        enemyCountField.text = enemyCount.ToString();

        if (enemyCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);

        }
    }
    public void YouLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);

    }
}
