using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmoCube : MonoBehaviour
{
    [SerializeField] private Vector3Int size;
    [SerializeField] private Color color;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, size);   
    }
}
