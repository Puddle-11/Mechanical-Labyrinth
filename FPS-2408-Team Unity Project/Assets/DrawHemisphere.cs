using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHemisphere : MonoBehaviour
{
    [SerializeField] private Vector3 planeNormal;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, 10);
    }
}
