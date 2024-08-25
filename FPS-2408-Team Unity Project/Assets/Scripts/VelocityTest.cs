using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTest : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float springConstant;
    private float currForce;
    public Vector3 inertia;
    [SerializeField] private float dampeningForce;
    [SerializeField] private float mass;

    // Update is called once per frame
    void Update()
    {
        currForce = Vector3.Distance(transform.position, target.position) * springConstant;
        Vector3 direction = (target.position - transform.position).normalized;
        inertia += direction * currForce * Time.deltaTime / mass;

        transform.Translate(inertia);
    }
}
