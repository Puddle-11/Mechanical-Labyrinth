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
    [SerializeField] private float knockbackmod;
    #region Monobehavior Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        { 
            knockback = GameManager.instance.playerRef.transform.position - transform.position;
            GameManager.instance.playerControllerRef.UpdateHealth(-explosionDamage);
            GameManager.instance.playerControllerRef.SetForce(knockback.normalized * knockbackmod);
            base.Death();
        }
    }
    #endregion
}