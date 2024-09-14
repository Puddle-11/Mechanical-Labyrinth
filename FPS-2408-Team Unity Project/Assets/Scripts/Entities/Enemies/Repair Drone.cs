using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairDrone : BaseEnemy
{
    [SerializeField] private int healingAmount;
    #region Monobehavior Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            GameManager.instance.playerControllerRef.UpdateHealth(+healingAmount);
        }
    }
    #endregion
}
