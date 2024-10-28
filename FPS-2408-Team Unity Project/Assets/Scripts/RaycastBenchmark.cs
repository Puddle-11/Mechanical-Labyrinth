using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBenchmark : MonoBehaviour
{
    public int itterations;
    public float distance;
    public bool infinity;
    public bool randomDir;
    public bool runRaycast;
    public Vector3 dir;
    public LayerMask mask;
    void Update()
    {
        for (int i = 0; i < itterations; i++)
        {
            RaycastHit hit;
            Vector3 _dir = randomDir ? Random.insideUnitSphere : dir;
            float dist = (infinity ? Mathf.Infinity : distance);
            if(runRaycast)Physics.Raycast(transform.position, _dir, out hit, dist, mask);
        }
    }
}
