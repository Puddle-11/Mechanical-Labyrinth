using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    private void Awake()
    {
        if(respawnPoint == null)
        {
            respawnPoint = transform;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other == GameManager.instance.playerRef)
        {
            GameManager.instance.playerControllerRef.SetPlayerSpawnPos(respawnPoint.position);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(respawnPoint.position, new Vector3(1,2,1));
    }
}
