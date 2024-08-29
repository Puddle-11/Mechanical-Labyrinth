using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public Collider triggerCollider;
    private bool isRunning = false;
    [SerializeField] private float elevatorDelay;
    [SerializeField] private TMP_Text levelNum;
    [SerializeField] private GameObject elevatorCam;
    private bool doorsOpen = true;
    [SerializeField] private float doorSpeed;
    [SerializeField] private Door[] doors;
    [SerializeField] private float distToLockout;
    [System.Serializable]
    public struct Door
    {
        public GameObject doorObj;
        public Vector3 openLocalPos;
        public Vector3 closeLocalPos;

    }
    private void OnEnable()
    {
        GameManager.instance.levelWon += EnableTrigg;
    }
    public void OnDisable()
    {
        GameManager.instance.levelWon -= EnableTrigg;
    }
    private void Start()
    {
               levelNum.text = GameManager.instance.GetCurrentLevel().ToString();

        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].openLocalPos = doors[i].doorObj.transform.localPosition;
            doors[i].doorObj.transform.localPosition = doors[i].closeLocalPos;
        }
        
    }
    // Start is called before the first frame update
    public void EnableTrigg()
    {
        triggerCollider.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameManager.instance.playerRef)
        {
            StartCoroutine(MoveToNextLevel());
        }
    }
    private void Update()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].doorObj.transform.localPosition = Vector3.MoveTowards(doors[i].doorObj.transform.localPosition, doorsOpen ? doors[i].openLocalPos : doors[i].closeLocalPos, doorSpeed * Time.deltaTime);
        }
    }
    public IEnumerator MoveToNextLevel()
    {
        if (isRunning) yield break;
        isRunning = true;
        GameManager.instance.UpdateCurrentLevel(1);
        GameManager.instance.playerRef.SetActive(false);
        elevatorCam.SetActive(true);

        yield return new WaitForSeconds(elevatorDelay);
        levelNum.text = GameManager.instance.GetCurrentLevel().ToString();
        yield return new WaitForSeconds(elevatorDelay);
        doorsOpen = false;
        yield return new WaitForSeconds(elevatorDelay);
        BootLoadManager.instance.ReloadScene();
        isRunning = false;
    }

    public IEnumerator OpenDoorDelay()
    {
        yield return new WaitForSeconds(elevatorDelay);
        doorsOpen = true;
    }

}
