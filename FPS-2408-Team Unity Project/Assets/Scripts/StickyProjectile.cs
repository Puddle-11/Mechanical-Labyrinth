using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rbRef;
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(rbRef);
        transform.parent = collision.transform;
    }
}
