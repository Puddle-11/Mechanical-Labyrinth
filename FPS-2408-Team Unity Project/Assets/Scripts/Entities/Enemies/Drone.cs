using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //====================================
    //REWORKED
    //====================================
public class Drone : BaseEnemy
{
    [SerializeField] private int explosionDamage;
    #region Monobehavior Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            GameManager.instance.playerControllerRef.UpdateHealth(-explosionDamage);
            base.Death();
        }
    }
    #endregion
}