using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LazerPointer : MonoBehaviour
{
    [SerializeField] private float dist;
    [SerializeField] private LayerMask ignore;
    private LineRenderer lnRef;
    private Vector3[] Positions = new Vector3[2];
    private void Start()
    {
        
        if(!TryGetComponent<LineRenderer>(out lnRef))
        {
            Debug.LogWarning("No line renderer assigned to lazer pointer " + gameObject.name);
        }
    }
    private void Update()
    {
        if (lnRef != null)
        {
            Positions[0] = transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, dist, ~ignore))
                Positions[1] = hit.point;
            else
                Positions[1] = transform.position + transform.forward * dist;
            lnRef.SetPositions(Positions);
        }
    }

}
