using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : BaseGun
{
    [SerializeField] private GameObject grenadeobject;
    [SerializeField] private float Projectilespeed;
    [SerializeField] private Transform shootpos;
    
    public override IEnumerator AttackDelay() {
        isAttacking = true;
        GameObject grenaderef = Instantiate(grenadeobject, shootpos.position, Quaternion.identity);
        Rigidbody rb = grenaderef.GetComponent<Rigidbody>();
        Vector3 Addedforce = shootpos.position.normalized * Projectilespeed;
        rb.AddForce(Addedforce, ForceMode.Impulse);
        yield return new WaitForSeconds(coolDown);
        isAttacking = false;

    }
}
