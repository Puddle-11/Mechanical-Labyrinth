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
    [SerializeField] private Plane armPlane;
    [SerializeField] private float margin;
    private Vector3[] points;
    [SerializeField] private Arm[] arms;
    private float totalLength;
    #region Custm Structs and Enums
    [System.Serializable]
    public struct Arm
    {
        public Transform worldObj;
        public float segmentLength;
    }
    #endregion
    private void Update() { Solve(); }
    public void CreateArm()
    {
        points = new Vector3[arms.Length + 1];
        for (int i = 1; i < points.Length; i++)
        {
            points[i] = transform.position + Vector3.up * i * arms[i - 1].segmentLength;
        }
        totalLength = GetTotalLength();
        Solve();
    }
    public float GetSegmentLength(int _index)
    {
        if (_index < 0 || _index >= arms.Length) return 0;
        return arms[_index].segmentLength;
    }
    public float GetTotalLength()
    {
        float res = 0;
        for (int i = 0; i < arms.Length; i++)
        {
            res += arms[i].segmentLength;
        }
        return res;
    }
    public void Solve()
    {
        if(points == null || arms == null || points.Length != arms.Length + 1)
        {
            CreateArm();
            return;
        }
        if (Vector3.Distance(target.position, start.position) > totalLength)
        {
            points = OutofRangeSolve(start.position, target.position, points);

        }
        else
        {
            for (int i = 0; i < 32; i++)
            {
                points = Iterate(start.position, points);
                Array.Reverse(points);
                Array.Reverse(arms);
                points = Iterate(target.position, points);
                Array.Reverse(points);
                Array.Reverse(arms);
                if (Vector3.Distance(points[0], start.position) < margin && Vector3.Distance(points[points.Length - 1], target.position) < margin)
                {
                    break;
                }
            }
        }
        for (int i = 0; i < points.Length - 1; i++)
        {
            if (arms == null || i >= arms.Length) break;

            if (arms[i].worldObj == null) continue;
            arms[i].worldObj.position = points[i];
            Quaternion rot = Quaternion.LookRotation(points[i + 1] - points[i]);

            arms[i].worldObj.rotation = rot;
        }
    }
    private Vector3[] Iterate(Vector3 _target, Vector3[] _points)
    {
        
        Vector2[] result = armPlane.WorldToPlane(_points);

        result[0] = armPlane.WorldToPlane(_target);
        for (int i = 1; i < result.Length; i++)
        {
            Vector2 point = (result[i] - result[i - 1]).normalized * GetSegmentLength(i - 1);
            result[i] = result[i - 1] + point;
        }

        return armPlane.PlaneToWorld(result);
    }
    public Vector3[] OutofRangeSolve(Vector3 _start, Vector3 _target, Vector3[] _points)
    {
        Vector3[] result = new Vector3[_points.Length];
        Array.Copy(_points, result, _points.Length);
        result[0] = _start;
        Vector3 dir = (_target - _start).normalized;

        for (int i = 1; i < result.Length; i++)
        {
            result[i] = result[i - 1] + dir * GetSegmentLength(i - 1);
        }
        return result;
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(points[0], 0.1f);
        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(points[i], 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(points[i], points[i - 1]);
        }
    }
}
