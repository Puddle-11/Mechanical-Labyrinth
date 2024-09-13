using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 axis;
    private float timer;
    //====================================
    //REWORKED
    //====================================
    private void LateUpdate()
    {
        timer += (Time.deltaTime * speed) % (Mathf.PI * 2);
        transform.position += axis * Mathf.Sin(timer) * speed * Time.deltaTime;
    }
}