using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPad : MonoBehaviour
{
    //[SerializeField] int HealAmount;
    private bool onPad;
    [SerializeField] private float healSpeed;
    private IEnumerator runningLoop;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {

            runningLoop = healLoop();
            StartCoroutine(runningLoop);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            StopCoroutine(runningLoop);
        }
    }
    public IEnumerator healLoop()
    {
        while (true)
        {
            Debug.Log("Incremented health");
            GameManager.instance.playerControllerRef.UpdateHealth(1);

            yield return new WaitForSeconds(healSpeed);
        }
    }

}
