using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Plane : MonoBehaviour
{
    //------------------------------------
    //These points will define the plane
    [SerializeField] private Transform p1;
    [SerializeField] private Transform p2;
    [SerializeField] private Transform p3;

    //------------------------------------

    private Vector3 planeNormal;
    private void Update()
    {
 
    }
    public Vector3 ProjectPoint(Vector3 originPoint)
    {
        Vector3 planeAnchorToOrigin = originPoint - p1.position;
        float scalarProj = Vector3.Dot(planeNormal, planeAnchorToOrigin) / Mathf.Pow(planeNormal.magnitude, 2);
        Vector3 vecProj = planeNormal * scalarProj;
        return originPoint - vecProj;
    }
    public Vector2[] WorldToPlane(Vector3[] _input)
    {
        Vector2[] result = new Vector2[_input.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = WorldToPlane(_input[i]);
        }
        return result;
    }
    public Vector3[] PlaneToWorld(Vector2[] _input)
    {
        Vector3[] result = new Vector3[_input.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = PlaneToWorld(_input[i]);
        }
        return result;
    }
    public Vector3 WorldToPlane(Vector3 _planePoint)
    {
        _planePoint -= p1.position;
        planeNormal = GetNormal(p1.position, p2.position, p3.position);

        //====================================
        //Set up plane orthogonal axis;
        Vector3 XAxis = (p2.position - p1.position).normalized;
        Vector3 YAxis = -Vector3.Cross(XAxis, planeNormal);
        Vector3 ZAxis = planeNormal;
        //====================================

        //Get X Y plane cords
        float x = Vector3.Dot(XAxis, _planePoint) / Mathf.Pow(XAxis.magnitude, 2);
        float y = Vector3.Dot(YAxis, _planePoint) / Mathf.Pow(YAxis.magnitude, 2);
        float z = Vector3.Dot(ZAxis, _planePoint) / Mathf.Pow(ZAxis.magnitude, 2);
        //====================================

        return new Vector3(x,y,z);
    }
    public Vector3 PlaneToWorld(Vector2 _planePos)
    {
        planeNormal = GetNormal(p1.position, p2.position, p3.position);
        //====================================
        //Set up plane orthogonal axis;
        Vector3 XAxis = (p2.position - p1.position).normalized;
        Vector3 YAxis = -Vector3.Cross(XAxis, planeNormal);
        Vector3 ZAxis = planeNormal;
        //====================================
        //Get x y vectors (in world position)
        Vector3 x = _planePos.x * XAxis;
        Vector3 y = _planePos.y * YAxis;
        //====================================
        return x +y + p1.position;
    }
    public Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = p2 - p1;
        Vector3 b = p3 - p1;
        Vector3 n = Vector3.Cross(a, b);
        n = n / n.magnitude;
        return n;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(p1.position, p2.position);
        Gizmos.DrawLine(p2.position, p3.position);
        Gizmos.DrawLine(p3.position, p1.position);

    }
}
