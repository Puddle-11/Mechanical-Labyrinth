using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Plane : MonoBehaviour
{
    [SerializeField] private Vector3 p1;
    [SerializeField] private Vector3 p2;
    [SerializeField] private Vector3 p3;
    [SerializeField] private Vector3 planeNormal;
    [SerializeField] private MeshRenderer meshReneder;
    [SerializeField] private MeshFilter meshFilter;
    private Vector3[] points;
    private int[] tris;
    private void Update()
    {
        UpdatePlaneMesh();
        planeNormal = GetNormal(p1,p2,p3);
    }
    public void UpdatePlaneMesh()
    {
        points = new Vector3[]{p1, p2, p3};
        tris = new int[] { 0,1,2};
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = new Mesh();
        }

        meshFilter.sharedMesh.vertices = points;
        meshFilter.sharedMesh.triangles = tris;

        meshFilter.sharedMesh.RecalculateNormals();

    }
    public Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = p2 - p1;
        Vector3 b = p3 - p1;
        Vector3 n = Vector3.Cross(a, b);
        return n.normalized;


    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + p1, 0.2f);
        Gizmos.DrawSphere(transform.position + p2, 0.2f);
        Gizmos.DrawSphere(transform.position + p3, 0.2f);
        Vector3 avgPos = transform.position + (p1 + p2 + p3) / 3;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(avgPos, avgPos + planeNormal * 2);
    }
}
