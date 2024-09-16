using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class ExitDoor : MonoBehaviour
{
    [SerializeField] private bool returnToMenu;
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private TMP_Text levelNumDisplay;
    [SerializeField] private GameObject elevatorCam;
    [SerializeField] private Door[] doors;
    [Space]
    [SerializeField] private float elevatorDelay;
    [SerializeField] private float doorSpeed;
    [SerializeField] private float distToLockout;
    [SerializeField] private NextSceneStats[] nextScenes;
    private bool primed = false;
    private bool isRunning = false;
    private bool doorsOpen = true;
    #region Custom Structs and Enums


    [System.Serializable]
    public struct NextSceneStats 
    {
        public string name;
        public int interval;
    }


    [System.Serializable]


    public struct Door
    {
        public GameObject doorObj;
        public Vector3 openLocalPos;
        public Vector3 closeLocalPos;

    }
    #endregion

    #region MonoBehavior Methods

    private void Start()
    {
        levelNumDisplay.text = GameManager.instance.GetCurrentLevel().ToString();

        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].openLocalPos = doors[i].doorObj.transform.localPosition;
            doors[i].doorObj.transform.localPosition = doors[i].closeLocalPos;
        }
    }
    private void Update()    {
        if (Vector3.Distance(GameManager.instance.playerRef.transform.position, transform.position) > distToLockout && primed == false)
        {
            doorsOpen = false;
            if (GameManager.instance.GetGameGoal() == 0)
            {
                EnableTrigg();
            }
        }




        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].doorObj.transform.localPosition = Vector3.MoveTowards(doors[i].doorObj.transform.localPosition, doorsOpen ? doors[i].openLocalPos : doors[i].closeLocalPos, doorSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameManager.instance.playerRef)
        {
            StartCoroutine(MoveToNextLevel());
        }
    }
    #endregion

    #region Getters and Setters
    public void EnableTrigg()
    {
        primed = true;
        doorsOpen = true;

        triggerCollider.enabled = true;
    }
    #endregion

    public IEnumerator MoveToNextLevel()
    {
        if (isRunning) yield break;
        isRunning = true;



        GameManager.instance.UpdateCurrentLevel(1);
        GameManager.instance.playerRef.SetActive(false);


        elevatorCam.SetActive(true);

        yield return new WaitForSeconds(elevatorDelay);
        levelNumDisplay.text = GameManager.instance.GetCurrentLevel().ToString();
        yield return new WaitForSeconds(elevatorDelay);
        doorsOpen = false;
        yield return new WaitForSeconds(elevatorDelay);
        int index = 0;
        for (int i = 0; i < nextScenes.Length; i++)
        {
            if(GameManager.instance == null)
            {
                break;
            }
            if (nextScenes[i].interval % GameManager.instance.GetCurrentLevel() == 0)
            {
                index = i;
                break;
            }
        }

        if (returnToMenu)
        {
            BootLoadManager.instance.ExitGameMode();
        }
        else if (nextScenes[index].name == null || nextScenes[index].name == "")
        {
            BootLoadManager.instance.ReloadScene();

        }
        else
        {
            BootLoadManager.instance.LoadScene(nextScenes[index].name);
        }

        isRunning = false;
    }
}
