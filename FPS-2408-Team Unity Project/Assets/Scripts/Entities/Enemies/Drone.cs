using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //====================================
    //REWORKED
    //====================================
public class Drone : BaseEnemy
{
    [SerializeField] private int explosionDamage;
    [SerializeField] private Vector3 knockback;
    #region Monobehavior Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            GameManager.instance.playerControllerRef.UpdateHealth(-explosionDamage);
            GameManager.instance.playerControllerRef.SetForce(knockback);
            base.Death();
        }
    }
    #endregion
}