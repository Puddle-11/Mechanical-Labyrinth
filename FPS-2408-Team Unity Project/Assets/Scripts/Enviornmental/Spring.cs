using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] Vector3 SpringDirection;
    [SerializeField] float Springforce;

    #region MonoBehavior Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {   
             GameManager.instance.playerControllerRef.SetPlayervel(SpringDirection.normalized * Springforce);
             GameManager.instance.playerControllerRef.UpdateJumpAmount(1);
        }
    }
    #endregion
}
