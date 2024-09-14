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
        StartCoroutine(DashTowardsPlayer());
        base.Update();
    }
    IEnumerator DashTowardsPlayer() {
        Vector3 directiontoplayer = GameManager.instance.playerRef.transform.position - transform.position;
        Vector3 Direction = directiontoplayer / directiontoplayer.magnitude;
        while (currState == EnemyState.Attack)
        {
            yield return new WaitForSeconds(5);
            transform.Translate(directiontoplayer.normalized * DashSpeed * Time.deltaTime, Space.World);

        }
    }

}
