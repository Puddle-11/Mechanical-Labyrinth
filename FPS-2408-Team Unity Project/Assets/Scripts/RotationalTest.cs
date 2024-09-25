using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
[ExecuteInEditMode]
public class RotationalTest : MonoBehaviour
{
    public Vector3 origin;
    public Vector3 rot;
    public Vector3 offset;
    public float radius;
    private void Update()
    {
    }
    public Vector3 Rotate(Vector3 _pos, Vector3 _pivot)
    {
        Vector3 tempPos = _pos - _pivot;
        //Initialize
        float x = tempPos.x;
        float y = tempPos.y;
        float z = tempPos.z;
        //X Rotation
        float x1 = x;
        float y1 = y * Mathf.Cos(rot.x) - z * Mathf.Sin(rot.x);
        float z1 = y * Mathf.Sin(rot.x) + z * Mathf.Cos(rot.x);
        //Y Rotation
        float x2 = x1 * Mathf.Cos(rot.y) + z1 * Mathf.Sin(rot.y);
        float y2 = y1;
        float z2 = -1 * x1 * Mathf.Sin(rot.y) + z1 * Mathf.Cos(rot.y);
        //Z Rotation
        float x3 = x2 * Mathf.Cos(rot.z) - y2 * Mathf.Sin(rot.z);
        float y3 = x2 * Mathf.Sin(rot.z) + y2 * Mathf.Cos(rot.z);
        float z3 = z2;
        tempPos = new Vector3(x3, y3, z3) + offset;
        return tempPos;
    }

    private void OnDrawGizmos()
    {
        DrawCircle();
    }
    private void DrawCircle()
    {
        float step = 0;
        Vector3 prevPos = new Vector2(Mathf.Sin(step), Mathf.Cos(step));
        while (step < Mathf.PI * 2)
        {
            step += 0.1f;
            Vector2 dcirc = new Vector2(Mathf.Sin(step), Mathf.Cos(step));
            Gizmos.DrawLine(Rotate( offset + (Vector3)dcirc, offset) + transform.position, Rotate(offset + (Vector3)prevPos, offset) + transform.position);
            prevPos = dcirc;
        }

    }
}
 