using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dasher : BaseEnemy
{
    [SerializeField] int DashSpeed;
    [SerializeField] int RamDamage;
    [SerializeField] Vector3 Knockback;
    [SerializeField] private float knockbackmod;
    [SerializeField] private float cooldown;
    [SerializeField] private bool Iscurrentlydashing;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef) {
            Knockback = GameManager.instance.playerRef.transform.position - transform.position;
            GameManager.instance.playerControllerRef.UpdateHealth(-RamDamage);
            GameManager.instance.playerControllerRef.SetForce(Knockback.normalized * knockbackmod);
        }
    }
    public override void Update()
    {
        if (currState == EnemyState.Attack)
        {
            StartCoroutine(DashTowardsPlayer());
        }
        base.Update();
    }
    IEnumerator DashTowardsPlayer() {
        if (Iscurrentlydashing == true) {
            yield break;
        }
        Iscurrentlydashing = true;
        Vector3 directiontoplayer = GameManager.instance.playerRef.transform.position - transform.position;
            
           float dashtimer = 0;
        while (dashtimer < cooldown) { 
            transform.position += directiontoplayer.normalized * DashSpeed * Time.deltaTime;
            dashtimer += Time.deltaTime;
            yield return null;
        }
        Iscurrentlydashing = false;
            Debug.Log("charging dash");
            
            //yield return new WaitForSeconds(1);
        //Debug.Log("dashing");
    }

}
