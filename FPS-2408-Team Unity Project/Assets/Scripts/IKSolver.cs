using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform target;
    [SerializeField] private int pointCount;
    [SerializeField] private float segmentDist;
    [SerializeField] private Vector3[] points;
    public void CreateArm()
    {
        points = new Vector3[pointCount];
    }
    public void Solve()
    {

    }
    public void OnDrawGizmo()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawWireSphere(points[i], 0.5f);
        }
    }
}
