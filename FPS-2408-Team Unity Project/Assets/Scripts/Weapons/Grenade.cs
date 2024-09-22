using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] int Explosiondamage;
    [SerializeField] float radius;
    Collider[] colliderref;
    private void Explosion()
    {
        IHealth healthref;
        colliderref = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider coll in colliderref) {
            if (coll.gameObject.TryGetComponent(out healthref)) {
                healthref.SetHealth(-Explosiondamage);
            }
        }
        Destroy(gameObject);
    } 
    
}
