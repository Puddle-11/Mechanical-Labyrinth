using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
[ExecuteInEditMode]
public class IKSolver : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform target;
    [SerializeField] private int pointCount;
    [SerializeField] private float segmentDist;
    [SerializeField] private float margin;
    [SerializeField] private Vector3[] points;
    private void Awake()
    {
        CreateArm();
    }
    private void Update()
    {
        DrawArmGizmos();
        Solve();
    }


    public void CreateArm()
    {
        points = new Vector3[pointCount];

        Solve();
    }
    public void Solve()
    {
        if (Vector3.Distance(target.position, start.position) > (pointCount - 1) * segmentDist)
        {
            points = OutofRangeSolve(start.position, target.position, points, segmentDist);
            return;
        }


        int i = 0;
        while (i < 100)
        {

            points = Iterate(start.position, points, segmentDist);
            Array.Reverse(points);
            points = Iterate(target.position, points, segmentDist);
            Array.Reverse(points);
            



            i++;
        }
    }
    private Vector3[] Iterate(Vector3 _target, Vector3[] _points, float _length)

    {
        Vector3[] result = new Vector3[_points.Length];
        Array.Copy(_points, result, _points.Length);

        result[0] = _target;
        for (int i = 1; i < result.Length; i++)
        {
            Vector3 point = (result[i] - result[i - 1]).normalized * _length;
            result[i] = result[i - 1] + point;
        }
        return result;
    }
    public Vector3[] OutofRangeSolve(Vector3 _start, Vector3 _target, Vector3[] _points, float _length)
    {
        Vector3[] result = new Vector3[_points.Length];
        Array.Copy(_points, result, _points.Length);
        result[0] = _start;
        Vector3 dir = (_target - _start).normalized;

        for (int i = 1; i < result.Length; i++)
        {
            result[i] = result[i - 1] + dir * _length;
        }
        return result;
    }
  
    public void DrawArmGizmos()
    {
        for (int i = 1; i < points.Length; i++)
        {
            Vector3 point = (points[i] - points[i - 1]).normalized * segmentDist;

            Debug.DrawRay(points[i - 1], point, Color.red);

        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(points[0], 0.1f);
        Gizmos.color =  Color.blue;
        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.DrawWireSphere(points[i], 0.1f);
        }
    }
}
