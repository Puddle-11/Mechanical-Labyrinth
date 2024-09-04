using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class RotateObj : MonoBehaviour
{
    public float speed;
    public Vector3 axis;
    private float timer;
    void Update()
    {
        timer += Time.deltaTime * speed;
        if(timer > 360)
        {
            timer = 0;
        }

        transform.rotation = Quaternion.AngleAxis(timer, axis);

    }
}
