using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameManager.instance.playerRef)
        {
            BootLoadManager.instance.LoadGameScene(nextSceneName);
        }
    }
}
