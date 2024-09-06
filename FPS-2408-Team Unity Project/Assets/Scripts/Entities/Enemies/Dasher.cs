using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dasher : BaseEnemy
{
    [SerializeField] int DashSpeed;
    [SerializeField] int RamDamage;
    [SerializeField] Vector3 Knockback;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("Hit Player");
            GameManager.instance.playerControllerRef.UpdateHealth(-RamDamage);
            GameManager.instance.playerControllerRef.GetForce();
            GameManager.instance.playerControllerRef.SetForce(Knockback.normalized);
        }
    }



}
