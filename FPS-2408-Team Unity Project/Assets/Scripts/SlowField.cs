using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : MonoBehaviour
{
    [SerializeField] private float slowMod = 0.5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if(other.gameObject == GameManager.instance.playerRef)
        {
            GameManager.instance.playerControllerRef.UpdatePlayerSpeed(slowMod);

        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        if (other.gameObject == GameManager.instance.playerRef)
        {
            GameManager.instance.playerControllerRef.UpdatePlayerSpeed(1 / slowMod);
        }
    }
}
