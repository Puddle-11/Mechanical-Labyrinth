using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class StickyProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rbRef;

    #region MonoBehavior Methods
    private void OnCollisionEnter(Collision collision)
    {

        Destroy(rbRef);
        transform.parent = collision.transform;

    }
    #endregion

}
